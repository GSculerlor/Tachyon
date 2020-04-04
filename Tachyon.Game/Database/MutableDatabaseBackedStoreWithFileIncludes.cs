using System.Linq;
using Microsoft.EntityFrameworkCore;
using osu.Framework.Platform;

namespace Tachyon.Game.Database
{
    public abstract class MutableDatabaseBackedStoreWithFileIncludes<T, TFileInfo> : MutableDatabaseBackedStore<T>
        where T : class, IHasPrimaryKey, IHasFiles<TFileInfo>
        where TFileInfo : INamedFileInfo
    {
        protected MutableDatabaseBackedStoreWithFileIncludes(IDatabaseContextFactory contextFactory, Storage storage = null)
            : base(contextFactory, storage)
        {
        }

        protected override IQueryable<T> AddIncludesForConsumption(IQueryable<T> query) =>
            base.AddIncludesForConsumption(query)
                .Include(s => s.Files).ThenInclude(f => f.FileInfo);

        protected override IQueryable<T> AddIncludesForDeletion(IQueryable<T> query) =>
            base.AddIncludesForDeletion(query)
                .Include(s => s.Files); // don't include FileInfo. these are handled by the FileStore itself.
    }
}
