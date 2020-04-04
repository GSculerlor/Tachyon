using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using osu.Framework.Platform;
using Tachyon.Game.Database;

namespace Tachyon.Game.Beatmaps
{
    public class BeatmapStore : MutableDatabaseBackedStoreWithFileIncludes<BeatmapSetInfo, BeatmapSetFileInfo>
    {
        public BeatmapStore(IDatabaseContextFactory contextFactory) 
            : base(contextFactory)
        {
        } 
        
        protected override IQueryable<BeatmapSetInfo> AddIncludesForDeletion(IQueryable<BeatmapSetInfo> query) =>
            base.AddIncludesForDeletion(query)
                .Include(s => s.Metadata)
                .Include(s => s.Beatmaps).ThenInclude(b => b.BaseDifficulty)
                .Include(s => s.Beatmaps).ThenInclude(b => b.Metadata);

        protected override IQueryable<BeatmapSetInfo> AddIncludesForConsumption(IQueryable<BeatmapSetInfo> query) =>
            base.AddIncludesForConsumption(query)
                .Include(s => s.Metadata)
                .Include(s => s.Beatmaps).ThenInclude(b => b.BaseDifficulty)
                .Include(s => s.Beatmaps).ThenInclude(b => b.Metadata);

        protected override void Purge(List<BeatmapSetInfo> items, TachyonDbContext context)
        {
            context.BeatmapMetadata.RemoveRange(items.Select(s => s.Metadata));
            context.BeatmapMetadata.RemoveRange(items.SelectMany(s => s.Beatmaps.Select(b => b.Metadata).Where(m => m != null)));

            context.BeatmapDifficulty.RemoveRange(items.SelectMany(s => s.Beatmaps.Select(b => b.BaseDifficulty)));

            base.Purge(items, context);
        }

        public IQueryable<BeatmapInfo> Beatmaps =>
            ContextFactory.Get().BeatmapInfo
                .Include(b => b.BeatmapSet).ThenInclude(s => s.Metadata)
                .Include(b => b.BeatmapSet).ThenInclude(s => s.Files).ThenInclude(f => f.FileInfo)
                .Include(b => b.Metadata)
                .Include(b => b.BaseDifficulty);
    }
}