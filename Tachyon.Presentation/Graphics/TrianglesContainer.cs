using osu.Framework.Allocation;
using osu.Framework.Extensions.Color4Extensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Colour;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using Tachyon.Presentation.Utils;

namespace Tachyon.Presentation.Graphics
{
    public class TrianglesContainer : Container
    {
        private readonly Box background;

        public TrianglesContainer()
        {
            RelativeSizeAxes = Axes.Both;

            InternalChildren = new Drawable[]
            {
                background = new Box
                {
                    RelativeSizeAxes = Axes.Both,
                },
                new SectionTriangles
                {
                    Anchor = Anchor.BottomCentre,
                    Origin = Anchor.BottomCentre,
                },
            };
        }

        [BackgroundDependencyLoader]
        private void load(ColorUtils colorUtils)
        {
            background.Colour = colorUtils.Background5;
        }

        private class SectionTriangles : Container
        {
            private readonly Triangles triangles;
            private readonly Box foreground;

            public SectionTriangles()
            {
                RelativeSizeAxes = Axes.X;
                Height = 200;
                Masking = true;
                Children = new Drawable[]
                {
                    triangles = new Triangles
                    {
                        Anchor = Anchor.BottomCentre,
                        Origin = Anchor.BottomCentre,
                        RelativeSizeAxes = Axes.Both,
                        TriangleScale = 3,
                    },
                    foreground = new Box
                    {
                        RelativeSizeAxes = Axes.Both,
                    }
                };
            }

            [BackgroundDependencyLoader]
            private void load(ColorUtils colorUtils)
            {
                triangles.ColourLight = colorUtils.Background4;
                triangles.ColourDark = colorUtils.Background5.Darken(0.2f);
                foreground.Colour = ColourInfo.GradientVertical(colorUtils.Background5, colorUtils.Background5.Opacity(0));
            }
        }
    }
}
