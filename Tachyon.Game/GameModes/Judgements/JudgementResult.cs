using JetBrains.Annotations;
using Tachyon.Game.GameModes.Objects;
using Tachyon.Game.GameModes.Scoring;

namespace Tachyon.Game.GameModes.Judgements
{
    public class JudgementResult
    {
        public HitResult Type;

        [NotNull]
        public readonly HitObject HitObject;

        [NotNull]
        public readonly Judgement Judgement;

        public double TimeOffset { get; internal set; }

        public int ComboAtJudgement { get; internal set; }

        public int HighestComboAtJudgement { get; internal set; }

        public double HealthAtJudgement { get; internal set; }

        public bool FailedAtJudgement { get; internal set; }

        public bool HasResult => Type > HitResult.None;

        public bool IsHit => Type > HitResult.Miss;

        public JudgementResult([NotNull] HitObject hitObject, [NotNull] Judgement judgement)
        {
            HitObject = hitObject;
            Judgement = judgement;
        }

        public override string ToString() => $"{Type} (Score:{Judgement.NumericResultFor(this)} HP:{Judgement.HealthIncreaseFor(this)} {Judgement})";
    }
}