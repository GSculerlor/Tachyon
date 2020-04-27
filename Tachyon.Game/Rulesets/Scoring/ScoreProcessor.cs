using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using osu.Framework.Bindables;
using osu.Framework.Extensions;
using Tachyon.Game.Rulesets.Judgements;
using Tachyon.Game.Scoring;

namespace Tachyon.Game.Rulesets.Scoring
{
    public class ScoreProcessor : JudgementProcessor
    {
        private const double base_portion = 0.3;
        private const double combo_portion = 0.7;
        private const double max_score = 1000000;

        /// <summary>
        /// The current total score.
        /// </summary>
        public readonly BindableDouble TotalScore = new BindableDouble { MinValue = 0 };

        /// <summary>
        /// The current accuracy.
        /// </summary>
        public readonly BindableDouble Accuracy = new BindableDouble(1) { MinValue = 0, MaxValue = 1 };

        /// <summary>
        /// The current combo.
        /// </summary>
        public readonly BindableInt Combo = new BindableInt();

        /// <summary>
        /// The current rank.
        /// </summary>
        public readonly Bindable<ScoreRank> Rank = new Bindable<ScoreRank>(ScoreRank.S);

        /// <summary>
        /// The highest combo achieved by this score.
        /// </summary>
        public readonly BindableInt HighestCombo = new BindableInt();

        private double maxHighestCombo;

        private double maxBaseScore;
        private double rollingMaxBaseScore;
        private double baseScore;
        private double bonusScore;

        private double scoreMultiplier = 1;

        public ScoreProcessor()
        {
            Debug.Assert(base_portion + combo_portion == 1.0);

            Combo.ValueChanged += combo => HighestCombo.Value = Math.Max(HighestCombo.Value, combo.NewValue);
            Accuracy.ValueChanged += accuracy =>
            {
                Rank.Value = rankFrom(accuracy.NewValue);
            };
        }

        private readonly Dictionary<HitResult, int> scoreResultCounts = new Dictionary<HitResult, int>();

        protected sealed override void ApplyResultInternal(JudgementResult result)
        {
            result.ComboAtJudgement = Combo.Value;
            result.HighestComboAtJudgement = HighestCombo.Value;

            if (result.FailedAtJudgement)
                return;

            if (result.Judgement.AffectsCombo)
            {
                switch (result.Type)
                {
                    case HitResult.None:
                        break;

                    case HitResult.Miss:
                        Combo.Value = 0;
                        break;

                    default:
                        Combo.Value++;
                        break;
                }
            }

            if (result.Judgement.IsBonus)
            {
                if (result.IsHit)
                    bonusScore += result.Judgement.NumericResultFor(result);
            }
            else
            {
                if (result.HasResult)
                    scoreResultCounts[result.Type] = scoreResultCounts.GetOrDefault(result.Type) + 1;

                baseScore += result.Judgement.NumericResultFor(result);
                rollingMaxBaseScore += result.Judgement.MaxNumericResult;
            }

            updateScore();
        }

        protected sealed override void RevertResultInternal(JudgementResult result)
        {
            Combo.Value = result.ComboAtJudgement;
            HighestCombo.Value = result.HighestComboAtJudgement;

            if (result.FailedAtJudgement)
                return;

            if (result.Judgement.IsBonus)
            {
                if (result.IsHit)
                    bonusScore -= result.Judgement.NumericResultFor(result);
            }
            else
            {
                if (result.HasResult)
                    scoreResultCounts[result.Type] = scoreResultCounts.GetOrDefault(result.Type) - 1;

                baseScore -= result.Judgement.NumericResultFor(result);
                rollingMaxBaseScore -= result.Judgement.MaxNumericResult;
            }

            updateScore();
        }

        private void updateScore()
        {
            if (rollingMaxBaseScore != 0)
                Accuracy.Value = baseScore / rollingMaxBaseScore;

            TotalScore.Value = getScore();
        }

        private double getScore()
        {
            return (max_score * (base_portion * baseScore / maxBaseScore + combo_portion * HighestCombo.Value / maxHighestCombo) + bonusScore) * scoreMultiplier;
        }

        private ScoreRank rankFrom(double acc)
        {
            if (acc > 0.95)
                return ScoreRank.S;
            if (acc > 0.9)
                return ScoreRank.A;
            if (acc > 0.8)
                return ScoreRank.B;
            if (acc > 0.7)
                return ScoreRank.C;

            return ScoreRank.D;
        }

        public int GetStatistic(HitResult result) => scoreResultCounts.GetOrDefault(result);

        public double GetStandardisedScore() => getScore();

        /// <summary>
        /// Resets this ScoreProcessor to a default state.
        /// </summary>
        /// <param name="storeResults">Whether to store the current state of the <see cref="ScoreProcessor"/> for future use.</param>
        protected override void Reset(bool storeResults)
        {
            base.Reset(storeResults);

            scoreResultCounts.Clear();

            if (storeResults)
            {
                maxHighestCombo = HighestCombo.Value;
                maxBaseScore = baseScore;
            }

            baseScore = 0;
            rollingMaxBaseScore = 0;
            bonusScore = 0;

            TotalScore.Value = 0;
            Accuracy.Value = 1;
            Combo.Value = 0;
            Rank.Value = ScoreRank.S;
            HighestCombo.Value = 0;
        }
        
        /// <summary>
        /// Retrieve a score populated with data for the current play this processor is responsible for.
        /// </summary>
        public void PopulateScore(ScoreInfo score)
        {
            score.TotalScore = (long)Math.Round(TotalScore.Value);
            score.Combo = Combo.Value;
            score.MaxCombo = HighestCombo.Value;
            score.Accuracy = Math.Round(Accuracy.Value, 4);
            score.Rank = Rank.Value;
            score.Date = DateTimeOffset.Now;

            var hitWindows = CreateHitWindows();

            foreach (var result in Enum.GetValues(typeof(HitResult)).OfType<HitResult>().Where(r => r > HitResult.None && hitWindows.IsHitResultAllowed(r)))
                score.Statistics[result] = GetStatistic(result);
        }

        /// <summary>
        /// Create a <see cref="HitWindows"/> for this processor.
        /// </summary>
        public virtual HitWindows CreateHitWindows() => new HitWindows();
    }
}
