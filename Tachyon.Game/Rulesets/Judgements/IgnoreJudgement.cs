using Tachyon.Game.Rulesets.Scoring;

 namespace Tachyon.Game.Rulesets.Judgements
{
    public class IgnoreJudgement : Judgement
    {
        protected override int NumericResultFor(HitResult result) => 0;

        protected override double HealthIncreaseFor(HitResult result) => 0;
    }
}