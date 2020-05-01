using Tachyon.Game.Rulesets.Scoring;

namespace Tachyon.Game.Rulesets.Judgements
{
    public class HoldNoteJudgement : Judgement
    {
        public override bool AffectsCombo => true;

        protected override double HealthIncreaseFor(HitResult result)
        {
            // Drum rolls can be ignored with no health penalty
            switch (result)
            {
                case HitResult.Miss:
                    return 0;

                default:
                    return base.HealthIncreaseFor(result);
            }
        }
    }
}
