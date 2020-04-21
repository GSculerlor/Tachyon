using osu.Framework.Graphics;

namespace Tachyon.Game.Rulesets.Objects.Drawables.Pieces
{
    public class ElongatedCirclePiece : CirclePiece
    {
        public ElongatedCirclePiece()
        {
            RelativeSizeAxes = Axes.Y;
        }

        protected override void Update()
        {
            base.Update();

            var padding = Content.DrawHeight * Content.Width / 2;

            Content.Padding = new MarginPadding
            {
                Left = padding,
                Right = padding,
            };

            Width = Parent.DrawSize.X + DrawHeight;
        }
    }
}
