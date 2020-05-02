using osu.Framework.Bindables;
using Tachyon.Game.Rulesets.Judgements;
using Tachyon.Game.Rulesets.Objects.Types;
using Tachyon.Game.Rulesets.Scoring;

namespace Tachyon.Game.Rulesets.Objects
{
    public abstract class TachyonHitObject : HitObject, IHasRow
    {
        /// <summary>
        /// Default size of a drawable taiko hit object.
        /// </summary>
        public const float DEFAULT_SIZE = 0.5f;
        
        public readonly Bindable<int> RowBindable = new Bindable<int>();

        public virtual int Row
        {
            get => RowBindable.Value;
            set => RowBindable.Value = value;
        }

        public override Judgement CreateJudgement() => new Judgement();

        protected override HitWindows CreateHitWindows() => new HitWindows();
    }
}
