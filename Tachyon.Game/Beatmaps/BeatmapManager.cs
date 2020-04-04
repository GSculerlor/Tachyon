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
using osu.Framework.Threading;
using Tachyon.Game.Beatmaps.Formats;
using Tachyon.Game.Database;
using Tachyon.Game.GameModes.Objects;
using Tachyon.Game.IO;
using Tachyon.Game.IO.Archives;

namespace Tachyon.Game.Beatmaps
{
    public partial class BeatmapManager : ArchiveModelManager<BeatmapSetInfo, BeatmapSetFileInfo>
    {
        public readonly WorkingBeatmap DefaultBeatmap;
        
        private readonly BeatmapStore beatmaps;
        private readonly BeatmapUpdateQueue updateQueue;
        private readonly AudioManager audioManager;
        private readonly GameHost gameHost;
        
        private readonly WeakList<WorkingBeatmap> workingCache = new WeakList<WorkingBeatmap>();
        
        public override string[] HandledExtensions => new[] { ".osz" };

        protected override string[] HashableFileTypes => new[] { ".osu" };

        protected override string HumanisedModelName => "beatmap";

        public BeatmapManager(Storage storage, IDatabaseContextFactory contextFactory, AudioManager audioManager, GameHost gameHost,  WorkingBeatmap defaultBeatmap = null) 
            : base(storage, contextFactory, new BeatmapStore(contextFactory), gameHost)
        {
            this.audioManager = audioManager;
            this.gameHost = gameHost;

            DefaultBeatmap = defaultBeatmap;
            beatmaps = (BeatmapStore) ModelStore;
            updateQueue = new BeatmapUpdateQueue();
        }

        protected override bool ShouldDeleteArchive(string path) => Path.GetExtension(path)?.ToLowerInvariant() == ".osz";

        protected override async Task Populate(BeatmapSetInfo beatmapSet, ArchiveReader archive, CancellationToken cancellationToken = default)
        {
            if (archive != null)
                beatmapSet.Beatmaps = createBeatmapDifficulties(beatmapSet.Files);

            foreach (BeatmapInfo b in beatmapSet.Beatmaps)
            {
                // remove metadata from difficulties where it matches the set
                if (beatmapSet.Metadata.Equals(b.Metadata))
                    b.Metadata = null;

                b.BeatmapSet = beatmapSet;
            }

            validateOnlineIds(beatmapSet);

            bool hadOnlineBeatmapIDs = beatmapSet.Beatmaps.Any(b => b.OnlineBeatmapID > 0);

            await updateQueue.UpdateAsync(beatmapSet, cancellationToken);

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

        protected override BeatmapSetInfo CreateModel(ArchiveReader archive)
        {
            string mapName = archive.Filenames.FirstOrDefault(f => f.EndsWith(".osu"));

            if (string.IsNullOrEmpty(mapName))
            {
                Logger.Log($"No beatmap files found in the beatmap archive ({archive.Name}).", LoggingTarget.Database);
                return null;
            }

            Beatmap beatmap;
            using (var stream = new LineBufferedReader(archive.GetStream(mapName)))
                beatmap = Decoder.GetDecoder<Beatmap>(stream).Decode(stream);

            return new BeatmapSetInfo
            {
                OnlineBeatmapSetID = beatmap.BeatmapInfo.BeatmapSet?.OnlineBeatmapSetID,
                Beatmaps = new List<BeatmapInfo>(),
                Metadata = beatmap.Metadata,
                DateAdded = DateTimeOffset.UtcNow
            };
        }
        
        public List<BeatmapSetInfo> GetAllUsableBeatmapSets() => GetAllUsableBeatmapSetsEnumerable().ToList();
        
        public IQueryable<BeatmapSetInfo> GetAllUsableBeatmapSetsEnumerable() => beatmaps.ConsumableItems.Where(s => !s.Protected);

        public IEnumerable<BeatmapSetInfo> QueryBeatmapSets(Expression<Func<BeatmapSetInfo, bool>> query) => beatmaps.ConsumableItems.AsNoTracking().Where(query);

        public BeatmapSetInfo QueryBeatmapSet(Expression<Func<BeatmapSetInfo, bool>> query) => beatmaps.ConsumableItems.AsNoTracking().FirstOrDefault(query);

        public IQueryable<BeatmapInfo> QueryBeatmaps(Expression<Func<BeatmapInfo, bool>> query) => beatmaps.Beatmaps.AsNoTracking().Where(query);

        public BeatmapInfo QueryBeatmap(Expression<Func<BeatmapInfo, bool>> query) => beatmaps.Beatmaps.AsNoTracking().FirstOrDefault(query);
        
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
                        new LargeTextureStore(gameHost?.CreateTextureLoaderStore(Files.Store)), beatmapInfo, audioManager));
                }

                previous?.TransferTo(working);
                return working;
            }
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

                    //beatmap.BeatmapInfo.StarDifficulty = ruleset?.CreateInstance().CreateDifficultyCalculator(new DummyConversionBeatmap(beatmap)).Calculate().StarRating ?? 0;
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

        private void validateOnlineIds(BeatmapSetInfo beatmapSet)
        {
            var beatmapIds = beatmapSet.Beatmaps.Where(b => b.OnlineBeatmapID.HasValue).Select(b => b.OnlineBeatmapID)
                .ToList();

            LogForModel(beatmapSet, "Validating online IDs...");

            // ensure all IDs are unique
            if (beatmapIds.GroupBy(b => b).Any(g => g.Count() > 1))
            {
                LogForModel(beatmapSet, "Found non-unique IDs, resetting...");
                resetIds();
                return;
            }

            // find any existing beatmaps in the database that have matching online ids
            var existingBeatmaps = QueryBeatmaps(b => beatmapIds.Contains(b.OnlineBeatmapID)).ToList();

            if (existingBeatmaps.Count > 0)
            {
                // reset the import ids (to force a re-fetch) *unless* they match the candidate CheckForExisting set.
                // we can ignore the case where the new ids are contained by the CheckForExisting set as it will either be used (import skipped) or deleted.
                var existing = CheckForExisting(beatmapSet);

                if (existing == null || existingBeatmaps.Any(b => !existing.Beatmaps.Contains(b)))
                {
                    LogForModel(beatmapSet, "Found existing import with IDs already, resetting...");
                    resetIds();
                }
            }

            void resetIds() => beatmapSet.Beatmaps.ForEach(b => b.OnlineBeatmapID = null);
        }
        
        private class BeatmapUpdateQueue
        {
            public Task UpdateAsync(BeatmapSetInfo beatmapSet, CancellationToken cancellationToken)
            {
                return Task.CompletedTask;
            }
        }
    }
}