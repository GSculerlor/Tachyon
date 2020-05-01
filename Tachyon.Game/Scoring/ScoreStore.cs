using System.Linq;
using Microsoft.EntityFrameworkCore;
using osu.Framework.Platform;
using Tachyon.Game.Database;

namespace Tachyon.Game.Scoring
{
    public class ScoreStore : MutableDatabaseBackedStoreWithFileIncludes<ScoreInfo, ScoreFileInfo>
    {
        public ScoreStore(IDatabaseContextFactory factory, Storage storage)
            : base(factory, storage)
        {
        }

        protected override IQueryable<ScoreInfo> AddIncludesForConsumption(IQueryable<ScoreInfo> query)
            => base.AddIncludesForConsumption(query)
                   .Include(s => s.Beatmap);
    }
}
