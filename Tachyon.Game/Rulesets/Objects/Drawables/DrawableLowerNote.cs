using osu.Framework.Graphics.Containers;
using Tachyon.Game.Rulesets.Objects.Drawables.Pieces;

namespace Tachyon.Game.Rulesets.Objects.Drawables
{
    public class DrawableLowerNote : DrawableNote
    {
        public override TachyonAction[] HitActions { get; } = { TachyonAction.LowerFirst, TachyonAction.LowerSecond, TachyonAction.MouseClick };

        public DrawableLowerNote(Note note)
            : base(note)
        {
        }

        protected override CompositeDrawable CreateMainPiece() => new LowerNotePiece();
    }
}
