using System;
using System.IO;
using Tachyon.Game.Beatmaps;
using Tachyon.Game.IO;
using Tachyon.Game.Rulesets;

namespace Tachyon.Game.Scoring
{
    public class ScoreDecoder
    {
        private IBeatmap currentBeatmap;
        
        private readonly BeatmapManager beatmaps;

        public ScoreDecoder(BeatmapManager beatmaps)
        {
            this.beatmaps = beatmaps;
        }

        public Score Parse(Stream stream)
        {
            var score = new Score();

            WorkingBeatmap workingBeatmap;

            using (SerializationReader sr = new SerializationReader(stream))
            {
                var scoreInfo = new ScoreInfo();

                score.ScoreInfo = scoreInfo;

                workingBeatmap = GetBeatmap(sr.ReadString());
                if (workingBeatmap is PlaceholderWorkingBeatmap)
                    throw new BeatmapNotFoundException();

                // MD5Hash
                sr.ReadString();

                scoreInfo.SetCountPerfect(sr.ReadUInt16());
                scoreInfo.SetCountGood(sr.ReadUInt16());
                scoreInfo.SetCountMiss(sr.ReadUInt16());

                scoreInfo.TotalScore = sr.ReadInt32();
                scoreInfo.MaxCombo = sr.ReadUInt16();

                /* score.Perfect = */
                sr.ReadBoolean();
                
                currentBeatmap = workingBeatmap.GetPlayableBeatmap(new TachyonRuleset().RulesetInfo);
                scoreInfo.Beatmap = currentBeatmap.BeatmapInfo;

                /* score.HpGraphString = */
                sr.ReadString();

                scoreInfo.Date = sr.ReadDateTime();
            }

            CalculateAccuracy(score.ScoreInfo);

            // before returning for database import, we must restore the database-sourced BeatmapInfo.
            // if not, the clone operation in GetPlayableBeatmap will cause a dereference and subsequent database exception.
            score.ScoreInfo.Beatmap = workingBeatmap.BeatmapInfo;

            return score;
        }

        protected void CalculateAccuracy(ScoreInfo score)
        {
            var totalScore = score.TotalScore;

            if (totalScore == 1000000)
                score.Rank = ScoreRank.S;
            else if (totalScore >= 950000)
                score.Rank = ScoreRank.S;
            else if (totalScore >= 900000)
                score.Rank = ScoreRank.A;
            else if (totalScore >= 800000)
                score.Rank = ScoreRank.B;
            else if (totalScore >= 700000)
                score.Rank = ScoreRank.C;
            else
                score.Rank = ScoreRank.D;
        }

        /// <summary>
        /// Retrieves the <see cref="WorkingBeatmap"/> corresponding to an MD5 hash.
        /// </summary>
        /// <param name="md5Hash">The MD5 hash.</param>
        /// <returns>The <see cref="WorkingBeatmap"/>.</returns>
        public WorkingBeatmap GetBeatmap(string md5Hash) => beatmaps.GetWorkingBeatmap(beatmaps.QueryBeatmap(b => !b.BeatmapSet.DeletePending && b.MD5Hash == md5Hash));

        public class BeatmapNotFoundException : Exception
        {
            public BeatmapNotFoundException()
                : base("No corresponding beatmap for the score could be found.")
            {
            }
        }
    }
}
