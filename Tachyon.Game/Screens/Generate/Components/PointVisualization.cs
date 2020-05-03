using osu.Framework.Graphics;
using osu.Framework.Graphics.Shapes;
using osuTK;

namespace Tachyon.Game.Screens.Generate.Components
{
    public class PointVisualisation : Box
    {
        public PointVisualisation(double startTime)
        {
            Origin = Anchor.TopCentre;

            RelativeSizeAxes = Axes.Y;
            Width = 1;
            EdgeSmoothness = new Vector2(1, 0);

            RelativePositionAxes = Axes.X;
            X = (float)startTime;
        }
    }
}