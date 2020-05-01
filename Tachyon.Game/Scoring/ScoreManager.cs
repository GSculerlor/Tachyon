using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using osu.Framework.Logging;
using osu.Framework.Platform;
using Tachyon.Game.Beatmaps;
using Tachyon.Game.Database;
using Tachyon.Game.IO.Archives;

namespace Tachyon.Game.Scoring
{
    public class ScoreManager : ArchiveModelManager<ScoreInfo, ScoreFileInfo>
    {
        public override string[] HandledExtensions => new[] { ".osr" };

        protected override string[] HashableFileTypes => new[] { ".osr" };
        
        private readonly Func<BeatmapManager> beatmaps;

        public ScoreManager(Func<BeatmapManager> beatmaps, Storage storage, IDatabaseContextFactory contextFactory, IIpcHost importHost = null)
            : base(storage, contextFactory, new ScoreStore(contextFactory, storage), importHost)
        {
            this.beatmaps = beatmaps;
        }
        protected override ScoreInfo CreateModel(ArchiveReader archive)
        {
            if (archive == null)
                return null;

            using (var stream = archive.GetStream(archive.Filenames.First(f => f.EndsWith(".osr"))))
            {
                try
                {
                    return new ScoreDecoder(beatmaps()).Parse(stream).ScoreInfo;
                }
                catch (ScoreDecoder.BeatmapNotFoundException e)
                {
                    Logger.Log(e.Message, LoggingTarget.Information, LogLevel.Error);
                    return null;
                }
            }
        }
        
        public Score GetScore(ScoreInfo score) => new Score { ScoreInfo = score };

        public List<ScoreInfo> GetAllUsableScores() => ModelStore.ConsumableItems.Where(s => !s.DeletePending).ToList();

        public IEnumerable<ScoreInfo> QueryScores(Expression<Func<ScoreInfo, bool>> query) => ModelStore.ConsumableItems.AsNoTracking().Where(query);

        public ScoreInfo Query(Expression<Func<ScoreInfo, bool>> query) => ModelStore.ConsumableItems.AsNoTracking().FirstOrDefault(query);
    }
}
