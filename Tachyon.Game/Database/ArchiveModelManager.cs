using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using osu.Framework.Extensions;
using osu.Framework.Extensions.IEnumerableExtensions;
using osu.Framework.Logging;
using osu.Framework.Platform;
using osu.Framework.Threading;
using SharpCompress.Common;
using Tachyon.Game.IO;
using Tachyon.Game.IO.Archives;
using Tachyon.Game.IPC;
using Tachyon.Game.Utils;

namespace Tachyon.Game.Database
{
    public abstract class ArchiveModelManager<TModel, TFileModel> : ICanAcceptFiles, IModelManager<TModel>
        where TModel : class, IHasFiles<TFileModel>, IHasPrimaryKey, ISoftDelete
        where TFileModel : class, INamedFileInfo, new()
    {
        private static readonly ThreadedTaskScheduler import_scheduler = new ThreadedTaskScheduler(1, nameof(ArchiveModelManager<TModel, TFileModel>));

        public event Action<TModel> ItemAdded;
        public event Action<TModel> ItemRemoved;
        
        public virtual string[] HandledExtensions => new[] { ".zip" };

        protected readonly FileStore Files;
        protected readonly IDatabaseContextFactory ContextFactory;
        protected readonly MutableDatabaseBackedStore<TModel> ModelStore;

        // ReSharper disable once NotAccessedField.Local (we should keep a reference to this so it is not finalised)
        private ArchiveImportIPCChannel ipc;

        protected ArchiveModelManager(Storage storage, IDatabaseContextFactory contextFactory, MutableDatabaseBackedStoreWithFileIncludes<TModel, TFileModel> modelStore, IIpcHost importHost = null)
        {
            ContextFactory = contextFactory;

            ModelStore = modelStore;
            ModelStore.ItemAdded += item => handleEvent(() => ItemAdded?.Invoke(item));
            ModelStore.ItemRemoved += s => handleEvent(() => ItemRemoved?.Invoke(s));

            Files = new FileStore(contextFactory, storage);

            if (importHost != null)
                ipc = new ArchiveImportIPCChannel(importHost, this);

            ModelStore.Cleanup();
        }

        public Task Import(params string[] paths)
        {
            return Imports(paths);
        }

        //TODO: rename this function
        protected async Task<IEnumerable<TModel>> Imports(params string[] paths)
        {
            var imported = new List<TModel>();

            await Task.WhenAll(paths.Select(async path =>
            {
                try
                {
                    var model = await Import(path);

                    lock (imported)
                    {
                        if (model != null)
                            imported.Add(model);
                    }
                }
                catch (TaskCanceledException)
                {
                    throw;
                }
                catch (Exception e)
                {
                    Logger.Error(e, $@"Could not import ({Path.GetFileName(path)})", LoggingTarget.Database);
                }
            }));

            return imported;
        }
        
        protected virtual bool ShouldDeleteArchive(string path) => false;

        public async Task<TModel> Import(string path, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            TModel import;
            using (ArchiveReader reader = getReaderFrom(path))
                import = await Import(reader, cancellationToken);

            try
            {
                if (import != null && File.Exists(path) && ShouldDeleteArchive(path))
                    File.Delete(path);
            }
            catch (Exception e)
            {
                LogForModel(import, $@"Could not delete original file after import ({Path.GetFileName(path)})", e);
            }

            return import;
        }

        public Task<TModel> Import(ArchiveReader archive, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            TModel model = null;

            try
            {
                model = CreateModel(archive);

                if (model == null)
                    return Task.FromResult<TModel>(null);
            }
            catch (TaskCanceledException)
            {
                throw;
            }
            catch (Exception e)
            {
                LogForModel(model, $"Model creation of {archive.Name} failed.", e);
                return null;
            }

            return Import(model, archive, cancellationToken);
        }

        protected abstract string[] HashableFileTypes { get; }

        protected static void LogForModel(TModel model, string message, Exception e = null)
        {
            string prefix = $"[{(model?.Hash ?? "?????").Substring(0, 5)}]";

            if (e != null)
                Logger.Error(e, $"{prefix} {message}", LoggingTarget.Database);
            else
                Logger.Log($"{prefix} {message}", LoggingTarget.Database);
        }

        private string computeHash(TModel item, ArchiveReader reader = null)
        {
            MemoryStream hashable = new MemoryStream();

            foreach (TFileModel file in item.Files.Where(f => HashableFileTypes.Any(f.Filename.EndsWith)))
            {
                using (Stream s = Files.Store.GetStream(file.FileInfo.StoragePath))
                    s.CopyTo(hashable);
            }

            if (hashable.Length > 0)
                return hashable.ComputeSHA2Hash();

            if (reader != null)
                return reader.Name.ComputeSHA2Hash();

            return item.Hash;
        }

        public async Task<TModel> Import(TModel item, ArchiveReader archive = null, CancellationToken cancellationToken = default) => await Task.Factory.StartNew(async () =>
        {
            cancellationToken.ThrowIfCancellationRequested();

            delayEvents();

            void rollback()
            {
                if (!Delete(item))
                {
                    LogForModel(item, "Dereferencing files for incomplete import.");
                    Files.Dereference(item.Files.Select(f => f.FileInfo).ToArray());
                }
            }

            try
            {
                LogForModel(item, "Beginning import...");

                item.Files = archive != null ? createFileInfos(archive, Files) : new List<TFileModel>();
                item.Hash = computeHash(item, archive);

                await Populate(item, archive, cancellationToken);

                using (var write = ContextFactory.GetForWrite())
                {
                    try
                    {
                        if (!write.IsTransactionLeader) throw new InvalidOperationException($"Ensure there is no parent transaction so errors can correctly be handled by {this}");

                        var existing = CheckForExisting(item);

                        if (existing != null)
                        {
                            if (CanUndelete(existing, item))
                            {
                                Undelete(existing);
                                LogForModel(item, $"Found existing {HumanisedModelName} for {item} (ID {existing.ID}) – skipping import.");
                                // existing item will be used; rollback new import and exit early.
                                rollback();
                                flushEvents(true);
                                return existing;
                            }

                            Delete(existing);
                            ModelStore.PurgeDeletable(s => s.ID == existing.ID);
                        }

                        PreImport(item);

                        // import to store
                        ModelStore.Add(item);
                    }
                    catch (Exception e)
                    {
                        write.Errors.Add(e);
                        throw;
                    }
                }

                LogForModel(item, "Import successfully completed!");
            }
            catch (Exception e)
            {
                if (!(e is TaskCanceledException))
                    LogForModel(item, "Database import or population failed and has been rolled back.", e);

                rollback();
                flushEvents(false);
                throw;
            }

            flushEvents(true);
            return item;
        }, cancellationToken, TaskCreationOptions.HideScheduler, import_scheduler).Unwrap();

        public void UpdateFile(TModel model, TFileModel file, Stream contents)
        {
            using (var usage = ContextFactory.GetForWrite())
            {
                Files.Dereference(file.FileInfo);

                usage.Context.Set<TFileModel>().Remove(file);

                model.Files.Remove(file);
                model.Files.Add(new TFileModel
                {
                    Filename = file.Filename,
                    FileInfo = Files.Add(contents)
                });

                Update(model);
            }
        }

        public void Update(TModel item)
        {
            using (ContextFactory.GetForWrite())
            {
                item.Hash = computeHash(item);

                ModelStore.Update(item);
            }
        }

        public bool Delete(TModel item)
        {
            using (ContextFactory.GetForWrite())
            {
                // re-fetch the model on the import context.
                var foundModel = queryModel().Include(s => s.Files).ThenInclude(f => f.FileInfo).FirstOrDefault(s => s.ID == item.ID);

                if (foundModel == null || foundModel.DeletePending) return false;

                if (ModelStore.Delete(foundModel))
                    Files.Dereference(foundModel.Files.Select(f => f.FileInfo).ToArray());
                return true;
            }
        }

        public void Delete(List<TModel> items, bool silent = false)
        {
            if (items.Count == 0) return;

            foreach (var b in items)
            {
                Delete(b);
            }
        }
        
        public void Undelete(List<TModel> items)
        {
            if (!items.Any()) return;

            foreach (var item in items)
            {
                Undelete(item);
            }
        }

        public void Undelete(TModel item)
        {
            using (var usage = ContextFactory.GetForWrite())
            {
                usage.Context.ChangeTracker.AutoDetectChangesEnabled = false;

                if (!ModelStore.Undelete(item)) return;

                Files.Reference(item.Files.Select(f => f.FileInfo).ToArray());

                usage.Context.ChangeTracker.AutoDetectChangesEnabled = true;
            }
        }

        private List<TFileModel> createFileInfos(ArchiveReader reader, FileStore files)
        {
            var fileInfos = new List<TFileModel>();

            string prefix = reader.Filenames.GetCommonPrefix();
            if (!(prefix.EndsWith("/") || prefix.EndsWith("\\")))
                prefix = string.Empty;

            foreach (string file in reader.Filenames)
            {
                using (Stream s = reader.GetStream(file))
                {
                    fileInfos.Add(new TFileModel
                    {
                        Filename = file.Substring(prefix.Length).ToStandardisedPath(),
                        FileInfo = files.Add(s)
                    });
                }
            }

            return fileInfos;
        }

        protected abstract TModel CreateModel(ArchiveReader archive);

        protected virtual Task Populate(TModel model, [CanBeNull] ArchiveReader archive, CancellationToken cancellationToken = default) => Task.CompletedTask;

        protected virtual void PreImport(TModel model)
        {
        }

        protected TModel CheckForExisting(TModel model) => model.Hash == null ? null : ModelStore.ConsumableItems.FirstOrDefault(b => b.Hash == model.Hash);

        protected virtual bool CanUndelete(TModel existing, TModel import) => true;

        private DbSet<TModel> queryModel() => ContextFactory.Get().Set<TModel>();

        protected virtual string HumanisedModelName => $"{typeof(TModel).Name.Replace("Info", "").ToLower()}";

        private ArchiveReader getReaderFrom(string path)
        {
            if (ZipUtils.IsZipArchive(path))
                return new ZipArchiveReader(File.Open(path, FileMode.Open, FileAccess.Read, FileShare.Read), Path.GetFileName(path));
            if (Directory.Exists(path))
                return new LegacyDirectoryArchiveReader(path);
            if (File.Exists(path))
                return new LegacyFileArchiveReader(path);

            throw new InvalidFormatException($"{path} is not a valid archive");
        }

        #region Event handling / delaying

        private readonly List<Action> queuedEvents = new List<Action>();

        private bool delayingEvents;
        
        private void delayEvents() => delayingEvents = true;

        private void flushEvents(bool perform)
        {
            Action[] events;

            lock (queuedEvents)
            {
                events = queuedEvents.ToArray();
                queuedEvents.Clear();
            }

            if (perform)
            {
                foreach (var a in events)
                    a.Invoke();
            }

            delayingEvents = false;
        }

        private void handleEvent(Action a)
        {
            if (delayingEvents)
            {
                lock (queuedEvents)
                    queuedEvents.Add(a);
            }
            else
                a.Invoke();
        }

        #endregion
    }
}
