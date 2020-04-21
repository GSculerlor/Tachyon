using Tachyon.Game.Rulesets.Judgements;

namespace Tachyon.Game.Rulesets.Objects
{
    public class BarLine : TachyonHitObject, IBarLine
    {
        public bool Major { get; set; }

        public override Judgement CreateJudgement() => new IgnoreJudgement();
    }
}
