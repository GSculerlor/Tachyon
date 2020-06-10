using System;
using Tachyon.Game.Beatmaps;
using Tachyon.Game.Beatmaps.ControlPoints;
using Tachyon.Game.Rulesets.Judgements;
using Tachyon.Game.Rulesets.Objects.Types;
using Tachyon.Game.Rulesets.Scoring;

namespace Tachyon.Game.Rulesets.Objects
{
    public class HoldNote : TachyonHitObject, IHasEndTime
    {
        /// <summary>
        /// Drum roll distance that results in a duration of 1 speed-adjusted beat length.
        /// </summary>
        private const float base_distance = 100;

        public double EndTime
        {
            get => StartTime + Duration;
            set => Duration = value - StartTime;
        }

        public double Duration { get; set; }

        /// <summary>
        /// Numer of ticks per beat length.
        /// </summary>
        public int TickRate = 1;

        /// <summary>
        /// Number of drum roll ticks required for a "Perfect" hit.
        /// </summary>
        public double RequiredPerfectHits { get; protected set; }

        /// <summary>
        /// The length (in milliseconds) between ticks of this drumroll.
        /// <para>Half of this value is the hit window of the ticks.</para>
        /// </summary>
        private double tickSpacing = 100;

        private float overallDifficulty = BeatmapDifficulty.DEFAULT_DIFFICULTY;

        protected override void ApplyDefaultsToSelf(ControlPointInfo controlPointInfo, BeatmapDifficulty difficulty)
        {
            base.ApplyDefaultsToSelf(controlPointInfo, difficulty);

            TimingControlPoint timingPoint = controlPointInfo.TimingPointAt(StartTime);

            tickSpacing = timingPoint.BeatLength / TickRate;
            overallDifficulty = difficulty.OverallDifficulty;
        }

        protected override void CreateNestedHitObjects()
        {
            createTicks();

            RequiredPerfectHits = NestedHitObjects.Count * Math.Min(0.30, 0.10 + 0.20 / 6 * overallDifficulty);

            base.CreateNestedHitObjects();
        }

        private void createTicks()
        {
            if (tickSpacing == 0)
                return;

            bool first = true;

            for (double t = StartTime; t < EndTime + tickSpacing / 2; t += tickSpacing)
            {
                AddNested(new HoldNoteTick
                {
                    FirstTick = first,
                    TickSpacing = tickSpacing,
                    StartTime = t
                });

                first = false;
            }
        }

        public override Judgement CreateJudgement() => new RollNoteJudgement();

        protected override HitWindows CreateHitWindows() => HitWindows.Empty;
    }
}
