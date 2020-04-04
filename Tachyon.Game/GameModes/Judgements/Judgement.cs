using Tachyon.Game.GameModes.Scoring;

namespace Tachyon.Game.GameModes.Judgements
{
    public class Judgement
    {
        private const double DEFAULT_MAX_HEALTH_INCREASE = 0.05;

        public virtual HitResult MaxResult => HitResult.Perfect;

        public int MaxNumericResult => NumericResultFor(MaxResult);

        public double MaxHealthIncrease => HealthIncreaseFor(MaxResult);

        protected virtual int NumericResultFor(HitResult result)
        {
            switch (result)
            {
                case HitResult.Late:
                    return 50;

                case HitResult.Perfect:
                    return 100;

                default:
                    return 0;
            }
        }

        public int NumericResultFor(JudgementResult result) => NumericResultFor(result.Type);

        protected virtual double HealthIncreaseFor(HitResult result)
        {
            switch (result)
            {
                case HitResult.Miss:
                    return -DEFAULT_MAX_HEALTH_INCREASE;

                case HitResult.Late:
                    return DEFAULT_MAX_HEALTH_INCREASE * 0.5;

                case HitResult.Perfect:
                    return DEFAULT_MAX_HEALTH_INCREASE;
                
                default:
                    return 0;
            }
        }

        public double HealthIncreaseFor(JudgementResult result) => HealthIncreaseFor(result.Type);

        public override string ToString() => $"MaxResult:{MaxResult} MaxScore:{MaxNumericResult}";
    }
}