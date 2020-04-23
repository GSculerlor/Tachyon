using osu.Framework.Graphics;
using Tachyon.Game.Rulesets.Objects.Drawables;

namespace Tachyon.Game.Rulesets.Judgements
{
    /// <summary>
    /// Text that is shown as judgement when a hit object is hit or missed.
    /// </summary>
    public class DrawableTachyonJudgement : DrawableJudgement
    {
        /// <summary>
        /// Creates a new judgement text.
        /// </summary>
        /// <param name="judgedObject">The object which is being judged.</param>
        /// <param name="result">The judgement to visualise.</param>
        public DrawableTachyonJudgement(JudgementResult result, DrawableHitObject judgedObject)
            : base(result, judgedObject)
        {
        }
    }
}
