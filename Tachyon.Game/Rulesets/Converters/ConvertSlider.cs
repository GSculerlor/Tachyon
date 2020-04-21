using System.Collections.Generic;
using Tachyon.Game.Audio;
using Tachyon.Game.Beatmaps;
using Tachyon.Game.Beatmaps.ControlPoints;
using Tachyon.Game.Rulesets.Objects;
using Tachyon.Game.Rulesets.Objects.Types;

namespace Tachyon.Game.Rulesets.Converters
{
    internal class ConvertSlider : ConvertHitObject, IHasCurve
    {
        /// <summary>
        /// Scoring distance with a speed-adjusted beat length of 1 second.
        /// </summary>
        private const float base_scoring_distance = 100;

        /// <summary>
        /// <see cref="ConvertSlider"/>s don't need a curve since they're converted to ruleset-specific hitobjects.
        /// </summary>
        public SliderPath Path { get; set; }

        public double Distance => Path.Distance;

        public List<IList<HitSampleInfo>> NodeSamples { get; set; }
        public int RepeatCount { get; set; }

        public double EndTime
        {
            get => StartTime + this.SpanCount() * Distance / Velocity;
            set => throw new System.NotSupportedException($"Adjust via {nameof(RepeatCount)} instead"); // can be implemented if/when needed.
        }

        public double Duration => EndTime - StartTime;

        public double Velocity = 1;

        protected override void ApplyDefaultsToSelf(ControlPointInfo controlPointInfo, BeatmapDifficulty difficulty)
        {
            base.ApplyDefaultsToSelf(controlPointInfo, difficulty);

            TimingControlPoint timingPoint = controlPointInfo.TimingPointAt(StartTime);
            DifficultyControlPoint difficultyPoint = controlPointInfo.DifficultyPointAt(StartTime);

            double scoringDistance = base_scoring_distance * difficulty.SliderMultiplier * difficultyPoint.SpeedMultiplier;

            Velocity = scoringDistance / timingPoint.BeatLength;
        }
    }
}