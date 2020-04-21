using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using osu.Framework.Audio;
using osu.Framework.Extensions;
using osu.Framework.Graphics.Textures;
using osu.Framework.Lists;
using osu.Framework.Logging;
using osu.Framework.Platform;
using Tachyon.Game.Beatmaps.Formats;
using Tachyon.Game.Database;
using Tachyon.Game.IO;
using Tachyon.Game.IO.Archives;
using Tachyon.Game.Rulesets.Objects;

namespace Tachyon.Game.Beatmaps
{
    public partial class BeatmapManager : ArchiveModelManager<BeatmapSetInfo, BeatmapSetFileInfo>
    {
        public readonly WorkingBeatmap DefaultBeatmap;

        public override string[] HandledExtensions => new[] { ".osz" };

        protected override string[] HashableFileTypes => new[] { ".osu" };

        private readonly BeatmapStore beatmaps;
        private readonly AudioManager audioManager;
        private readonly GameHost host;

        public BeatmapManager(Storage storage, IDatabaseContextFactory contextFactory, AudioManager audioManager, GameHost host = null,
                              WorkingBeatmap defaultBeatmap = null)
            : base(storage, contextFactory, new BeatmapStore(contextFactory), host)
        {
            this.audioManager = audioManager;
            this.host = host;

            DefaultBeatmap = defaultBeatmap;
            beatmaps = (BeatmapStore) ModelStore;
        }

        protected override bool ShouldDeleteArchive(string path) => Path.GetExtension(path)?.ToLowerInvariant() == ".osz";

        protected override async Task Populate(BeatmapSetInfo beatmapSet, ArchiveReader archive, CancellationToken cancellationToken = default)
        {
            if (archive != null)
                beatmapSet.Beatmaps = createBeatmapDifficulties(beatmapSet.Files);

            foreach (BeatmapInfo b in beatmapSet.Beatmaps)
            {
                if (beatmapSet.Metadata.Equals(b.Metadata))
                    b.Metadata = null;

                b.BeatmapSet = beatmapSet;
            }

            validateOnlineIds(beatmapSet);

            bool hadOnlineBeatmapIDs = beatmapSet.Beatmaps.Any(b => b.OnlineBeatmapID > 0);

            await Task.CompletedTask;

            if (hadOnlineBeatmapIDs && !beatmapSet.Beatmaps.Any(b => b.OnlineBeatmapID > 0))
            {
                if (beatmapSet.OnlineBeatmapSetID != null)
                {
                    beatmapSet.OnlineBeatmapSetID = null;
                    LogForModel(beatmapSet, "Disassociating beatmap set ID due to loss of all beatmap IDs");
                }
            }
        }

        protected override void PreImport(BeatmapSetInfo beatmapSet)
        {
            if (beatmapSet.Beatmaps.Any(b => b.BaseDifficulty == null))
                throw new InvalidOperationException($"Cannot import {nameof(BeatmapInfo)} with null {nameof(BeatmapInfo.BaseDifficulty)}.");

            if (beatmapSet.OnlineBeatmapSetID != null)
            {
                var existingOnlineId = beatmaps.ConsumableItems.FirstOrDefault(b => b.OnlineBeatmapSetID == beatmapSet.OnlineBeatmapSetID);

                if (existingOnlineId != null)
                {
                    Delete(existingOnlineId);
                    beatmaps.PurgeDeletable(s => s.ID == existingOnlineId.ID);
                    LogForModel(beatmapSet, $"Found existing beatmap set with same OnlineBeatmapSetID ({beatmapSet.OnlineBeatmapSetID}). It has been purged.");
                }
            }
        }

        private void validateOnlineIds(BeatmapSetInfo beatmapSet)
        {
            var beatmapIds = beatmapSet.Beatmaps.Where(b => b.OnlineBeatmapID.HasValue).Select(b => b.OnlineBeatmapID).ToList();

            LogForModel(beatmapSet, "Validating online IDs...");

            if (beatmapIds.GroupBy(b => b).Any(g => g.Count() > 1))
            {
                LogForModel(beatmapSet, "Found non-unique IDs, resetting...");
                resetIds();
                return;
            }

            var existingBeatmaps = QueryBeatmaps(b => beatmapIds.Contains(b.OnlineBeatmapID)).ToList();

            foreach (var beatmapInfo in existingBeatmaps)
            {
                Logger.Log($"Existing Beatmaps {beatmapInfo}");
            }
            
            if (existingBeatmaps.Count > 0)
            {
                var existing = CheckForExisting(beatmapSet);

                if (existing == null || existingBeatmaps.Any(b => !existing.Beatmaps.Contains(b)))
                {
                    LogForModel(beatmapSet, "Found existing import with IDs already, resetting...");
                    resetIds();
                }
            }

            void resetIds() => beatmapSet.Beatmaps.ForEach(b => b.OnlineBeatmapID = null);
        }
        
        private readonly WeakList<WorkingBeatmap> workingCache = new WeakList<WorkingBeatmap>();

        public WorkingBeatmap GetWorkingBeatmap(BeatmapInfo beatmapInfo, WorkingBeatmap previous = null)
        {
            if (beatmapInfo?.ID > 0 && previous != null && previous.BeatmapInfo?.ID == beatmapInfo.ID)
                return previous;

            if (beatmapInfo?.BeatmapSet == null || beatmapInfo == DefaultBeatmap?.BeatmapInfo)
                return DefaultBeatmap;

            lock (workingCache)
            {
                var working = workingCache.FirstOrDefault(w => w.BeatmapInfo?.ID == beatmapInfo.ID);

                if (working == null)
                {
                    if (beatmapInfo.Metadata == null)
                        beatmapInfo.Metadata = beatmapInfo.BeatmapSet.Metadata;

                    workingCache.Add(working = new BeatmapManagerWorkingBeatmap(Files.Store,
                        new LargeTextureStore(host?.CreateTextureLoaderStore(Files.Store)), beatmapInfo, audioManager));
                }

                previous?.TransferTo(working);
                return working;
            }
        }
        
        protected override bool CanUndelete(BeatmapSetInfo existing, BeatmapSetInfo import)
        {
            if (!base.CanUndelete(existing, import))
                return false;

            var existingIds = existing.Beatmaps.Select(b => b.OnlineBeatmapID).OrderBy(i => i);
            var importIds = import.Beatmaps.Select(b => b.OnlineBeatmapID).OrderBy(i => i);

            return existing.OnlineBeatmapSetID == import.OnlineBeatmapSetID && existingIds.SequenceEqual(importIds);
        }

        public BeatmapSetInfo QueryBeatmapSet(Expression<Func<BeatmapSetInfo, bool>> query) => beatmaps.ConsumableItems.AsNoTracking().FirstOrDefault(query);

        public List<BeatmapSetInfo> GetAllUsableBeatmapSets() => GetAllUsableBeatmapSetsEnumerable().ToList();

        public IQueryable<BeatmapSetInfo> GetAllUsableBeatmapSetsEnumerable() => beatmaps.ConsumableItems.Where(s => !s.DeletePending);

        public IEnumerable<BeatmapSetInfo> QueryBeatmapSets(Expression<Func<BeatmapSetInfo, bool>> query) => beatmaps.ConsumableItems.AsNoTracking().Where(query);

        public BeatmapInfo QueryBeatmap(Expression<Func<BeatmapInfo, bool>> query) => beatmaps.Beatmaps.AsNoTracking().FirstOrDefault(query);

        public IQueryable<BeatmapInfo> QueryBeatmaps(Expression<Func<BeatmapInfo, bool>> query) => beatmaps.Beatmaps.AsNoTracking().Where(query);

        protected override string HumanisedModelName => "beatmap";

        protected override BeatmapSetInfo CreateModel(ArchiveReader reader)
        {
            string mapName = reader.Filenames.FirstOrDefault(f => f.EndsWith(".osu"));

            if (string.IsNullOrEmpty(mapName))
            {
                Logger.Log($"No beatmap files found in the beatmap archive ({reader.Name}).", LoggingTarget.Database);
                return null;
            }

            Beatmap beatmap;
            using (var stream = new LineBufferedReader(reader.GetStream(mapName)))
                beatmap = Decoder.GetDecoder<Beatmap>(stream).Decode(stream);

            return new BeatmapSetInfo
            {
                OnlineBeatmapSetID = beatmap.BeatmapInfo.BeatmapSet?.OnlineBeatmapSetID,
                Beatmaps = new List<BeatmapInfo>(),
                Metadata = beatmap.Metadata,
                DateAdded = DateTimeOffset.UtcNow
            };
        }

        private List<BeatmapInfo> createBeatmapDifficulties(List<BeatmapSetFileInfo> files)
        {
            var beatmapInfos = new List<BeatmapInfo>();

            foreach (var file in files.Where(f => f.Filename.EndsWith(".osu")))
            {
                using (var raw = Files.Store.GetStream(file.FileInfo.StoragePath))
                using (var ms = new MemoryStream())
                using (var sr = new LineBufferedReader(ms))
                {
                    raw.CopyTo(ms);
                    ms.Position = 0;

                    var decoder = Decoder.GetDecoder<Beatmap>(sr);
                    IBeatmap beatmap = decoder.Decode(sr);

                    string hash = ms.ComputeSHA2Hash();

                    if (beatmapInfos.Any(b => b.Hash == hash))
                        continue;

                    beatmap.BeatmapInfo.Path = file.Filename;
                    beatmap.BeatmapInfo.Hash = hash;
                    beatmap.BeatmapInfo.MD5Hash = ms.ComputeMD5Hash();

                    beatmap.BeatmapInfo.StarDifficulty = 0;
                    beatmap.BeatmapInfo.Length = calculateLength(beatmap);
                    beatmap.BeatmapInfo.BPM = beatmap.ControlPointInfo.BPMMode;

                    beatmapInfos.Add(beatmap.BeatmapInfo);
                }
            }

            return beatmapInfos;
        }

        private double calculateLength(IBeatmap b)
        {
            if (!b.HitObjects.Any())
                return 0;

            var lastObject = b.HitObjects.Last();

            double endTime = lastObject.GetEndTime();
            double startTime = b.HitObjects.First().StartTime;

            return endTime - startTime;
        }
    }
}