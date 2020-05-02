using osu.Framework.Graphics.Containers;
using Tachyon.Game.Rulesets.Objects.Drawables.Pieces;

namespace Tachyon.Game.Rulesets.Objects.Drawables
{
    public class DrawableUpperNote : DrawableNote
    {
        public override TachyonAction[] HitActions { get; } = { TachyonAction.UpperFirst, TachyonAction.UpperSecond, TachyonAction.MouseClick };

        public DrawableUpperNote(Note note)
            : base(note)
        {
        }

        protected override CompositeDrawable CreateMainPiece() => new UpperNotePiece();
    }
}
