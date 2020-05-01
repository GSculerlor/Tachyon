using Tachyon.Game.Rulesets.Scoring;

namespace Tachyon.Game.Scoring
{
    public static class ScoreInfoExtensions
    {
        public static int? GetCountPerfect(this ScoreInfo scoreInfo) => scoreInfo.Statistics[HitResult.Perfect];
        
        public static void SetCountPerfect(this ScoreInfo scoreInfo, int value) =>
            scoreInfo.Statistics[HitResult.Perfect] = value;
        
        public static int? GetCountGood(this ScoreInfo scoreInfo) => scoreInfo.Statistics[HitResult.Good];
        
        public static void SetCountGood(this ScoreInfo scoreInfo, int value) =>
            scoreInfo.Statistics[HitResult.Good] = value;
        
        public static int? GetCountMiss(this ScoreInfo scoreInfo) =>
            scoreInfo.Statistics[HitResult.Miss];

        public static void SetCountMiss(this ScoreInfo scoreInfo, int value) =>
            scoreInfo.Statistics[HitResult.Miss] = value;
    }
}
