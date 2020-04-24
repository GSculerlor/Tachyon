using Tachyon.Game.Rulesets.Objects;
using Tachyon.Game.Rulesets.Scoring;

 namespace Tachyon.Game.Rulesets.Judgements
{
    /// <summary>
    /// The scoring information provided by a <see cref="HitObject"/>.
    /// </summary>
    public class Judgement
    {
        /// <summary>
        /// The default health increase for a maximum judgement, as a proportion of total health.
        /// By default, each maximum judgement restores 5% of total health.
        /// </summary>
        protected const double DEFAULT_MAX_HEALTH_INCREASE = 0.05;

        /// <summary>
        /// The maximum <see cref="HitResult"/> that can be achieved.
        /// </summary>
        public virtual HitResult MaxResult => HitResult.Perfect;

        /// <summary>
        /// Whether this <see cref="Judgement"/> should affect the current combo.
        /// </summary>
        public virtual bool AffectsCombo => true;

        /// <summary>
        /// Whether this <see cref="Judgement"/> should be counted as base (combo) or bonus score.
        /// </summary>
        public virtual bool IsBonus => !AffectsCombo;

        /// <summary>
        /// The numeric score representation for the maximum achievable result.
        /// </summary>
        public int MaxNumericResult => NumericResultFor(MaxResult);

        /// <summary>
        /// The health increase for the maximum achievable result.
        /// </summary>
        public double MaxHealthIncrease => HealthIncreaseFor(MaxResult);

        /// <summary>
        /// Retrieves the numeric score representation of a <see cref="HitResult"/>.
        /// </summary>
        /// <param name="result">The <see cref="HitResult"/> to find the numeric score representation for.</param>
        /// <returns>The numeric score representation of <paramref name="result"/>.</returns>
        protected virtual int NumericResultFor(HitResult result) => result > HitResult.Miss ? 1 : 0;

        /// <summary>
        /// Retrieves the numeric score representation of a <see cref="JudgementResult"/>.
        /// </summary>
        /// <param name="result">The <see cref="JudgementResult"/> to find the numeric score representation for.</param>
        /// <returns>The numeric score representation of <paramref name="result"/>.</returns>
        public int NumericResultFor(JudgementResult result) => NumericResultFor(result.Type);

        /// <summary>
        /// Retrieves the numeric health increase of a <see cref="HitResult"/>.
        /// </summary>
        /// <param name="result">The <see cref="HitResult"/> to find the numeric health increase for.</param>
        /// <returns>The numeric health increase of <paramref name="result"/>.</returns>
        protected virtual double HealthIncreaseFor(HitResult result)
        {
            switch (result)
            {
                case HitResult.Miss:
                    return -DEFAULT_MAX_HEALTH_INCREASE;

                case HitResult.Good:
                    return -DEFAULT_MAX_HEALTH_INCREASE * 0.5;

                case HitResult.Perfect:
                    return DEFAULT_MAX_HEALTH_INCREASE * 1;

                default:
                    return 0;
            }
        }

        /// <summary>
        /// Retrieves the numeric health increase of a <see cref="JudgementResult"/>.
        /// </summary>
        /// <param name="result">The <see cref="JudgementResult"/> to find the numeric health increase for.</param>
        /// <returns>The numeric health increase of <paramref name="result"/>.</returns>
        public double HealthIncreaseFor(JudgementResult result) => HealthIncreaseFor(result.Type);

        public override string ToString() => $"AffectsCombo:{AffectsCombo} MaxResult:{MaxResult} MaxScore:{MaxNumericResult}";
    }
}