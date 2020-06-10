using Tachyon.Game.Rulesets.Scoring;

namespace Tachyon.Game.Rulesets.Judgements
{
    public class RollNoteTickJudgement : Judgement
    {
        public override bool AffectsCombo => false;

        protected override int NumericResultFor(HitResult result)
        {
            switch (result)
            {
                default:
                    return 0;
            }
        }

        protected override double HealthIncreaseFor(HitResult result)
        {
            switch (result)
            {
                case HitResult.Perfect:
                    return 0.15;

                default:
                    return 0;
            }
        }
    }
}
