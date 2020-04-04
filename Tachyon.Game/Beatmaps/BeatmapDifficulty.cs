using Tachyon.Game.Database;

namespace Tachyon.Game.Beatmaps
{
    public class BeatmapDifficulty : IHasPrimaryKey
    {
        public const float DEFAULT_DIFFICULTY = 5;

        public int ID { get; set; }

        public float DrainRate { get; set; } = DEFAULT_DIFFICULTY;
        public float CircleSize { get; set; } = DEFAULT_DIFFICULTY;
        public float OverallDifficulty { get; set; } = DEFAULT_DIFFICULTY;

        private float? approachRate;

        public float ApproachRate
        {
            get => approachRate ?? OverallDifficulty;
            set => approachRate = value;
        }

        public double SliderMultiplier { get; set; } = 1;
        public double SliderTickRate { get; set; } = 1;

        public BeatmapDifficulty Clone() => (BeatmapDifficulty)MemberwiseClone();

        public static double DifficultyRange(double difficulty, double min, double mid, double max)
        {
            if (difficulty > 5)
                return mid + (max - mid) * (difficulty - 5) / 5;
            if (difficulty < 5)
                return mid - (mid - min) * (5 - difficulty) / 5;

            return mid;
        }

        public static double DifficultyRange(double difficulty, (double od0, double od5, double od10) range)
            => DifficultyRange(difficulty, range.od0, range.od5, range.od10);
    }
}
