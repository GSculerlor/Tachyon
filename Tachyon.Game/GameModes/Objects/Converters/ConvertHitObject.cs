using Tachyon.Game.GameModes.Judgements;
using Tachyon.Game.GameModes.Scoring;

namespace Tachyon.Game.GameModes.Objects.Converters
{
    public abstract class ConvertHitObject : HitObject
    {
        public override Judgement CreateJudgement() => new IgnoreJudgement();

        protected override HitWindows CreateHitWindows() => HitWindows.Empty;
    }
}