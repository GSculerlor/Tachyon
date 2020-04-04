using Tachyon.Game.GameModes.Scoring;

namespace Tachyon.Game.GameModes.Judgements
{
    public class IgnoreJudgement : Judgement
    {
        protected override int NumericResultFor(HitResult result) => 0;

        protected override double HealthIncreaseFor(HitResult result) => 0;
    }
}