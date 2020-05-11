using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osuTK;
using Tachyon.Game.Graphics;

namespace Tachyon.Game.Screens.Generate.Components
{
    public class CenterMarker : CompositeDrawable
    {
        private const float triangle_width = 15;
        private const float triangle_height = 10;
        private const float bar_width = 2;

        public CenterMarker()
        {
            RelativeSizeAxes = Axes.Y;
            Size = new Vector2(triangle_width, 1);

            Anchor = Anchor.Centre;
            Origin = Anchor.Centre;

            InternalChildren = new Drawable[]
            {
                new Box
                {
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre,
                    RelativeSizeAxes = Axes.Y,
                    Width = bar_width,
                },
                new Triangle
                {
                    Anchor = Anchor.TopCentre,
                    Origin = Anchor.BottomCentre,
                    Size = new Vector2(triangle_width, triangle_height),
                    Scale = new Vector2(1, -1)
                },
                new Triangle
                {
                    Anchor = Anchor.BottomCentre,
                    Origin = Anchor.BottomCentre,
                    Size = new Vector2(triangle_width, triangle_height),
                }
            };
        }

        [BackgroundDependencyLoader]
        private void load(TachyonColor colors)
        {
            Colour = colors.RedDark;
        }
    }
}
