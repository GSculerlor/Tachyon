using Tachyon.Game.Rulesets.Judgements;
using Tachyon.Game.Rulesets.Scoring;

namespace Tachyon.Game.Rulesets.Objects
{
    /// <summary>
    /// A scoring tick of a hold note.
    /// </summary>
    public class HoldNoteTick : TachyonHitObject
    {
        /// <summary>
        /// Whether this is the first (initial) tick of the slider.
        /// </summary>
        public bool FirstTick;

        /// <summary>
        /// The length (in milliseconds) between this tick and the next.
        /// <para>Half of this value is the hit window of the tick.</para>
        /// </summary>
        public double TickSpacing;

        /// <summary>
        /// The time allowed to hit this tick.
        /// </summary>
        public double HitWindow => TickSpacing / 2;

        public override Judgement CreateJudgement() => new RollNoteTickJudgement();

        protected override HitWindows CreateHitWindows() => HitWindows.Empty;
    }
}
