using Tachyon.Game.Rulesets.Judgements;
using Tachyon.Game.Rulesets.Objects;
using Tachyon.Game.Rulesets.Scoring;

namespace Tachyon.Game.Rulesets.Converters
{
    public abstract class ConvertHitObject : HitObject
    {
        public override Judgement CreateJudgement() => new IgnoreJudgement();

        protected override HitWindows CreateHitWindows() => HitWindows.Empty;
    }
}