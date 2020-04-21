using osu.Framework.Bindables;
using Tachyon.Game.Rulesets.Judgements;
using Tachyon.Game.Rulesets.Objects.Types;
using Tachyon.Game.Rulesets.Scoring;

namespace Tachyon.Game.Rulesets.Objects
{
    public abstract class TachyonHitObject : HitObject
    {
        /// <summary>
        /// Default size of a drawable taiko hit object.
        /// </summary>
        public const float DEFAULT_SIZE = 0.45f;

        public override Judgement CreateJudgement() => new Judgement();

        protected override HitWindows CreateHitWindows() => new HitWindows();
    }
}
