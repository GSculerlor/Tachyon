using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Timing;
using osuTK;
using Tachyon.Game.Graphics;
using Tachyon.Game.Screens.Generate.Components.Parts;

namespace Tachyon.Game.Screens.Generate.Components
{
    public class SummaryTimeline : BottomBarContainer
    {
        [BackgroundDependencyLoader]
        private void load(TachyonColor colors, IAdjustableClock adjustableClock)
        {
            Children = new Drawable[]
            {
                new MarkerPart(adjustableClock) { RelativeSizeAxes = Axes.Both },
                new ControlPointPart
                {
                    Anchor = Anchor.Centre,
                    Origin = Anchor.BottomCentre,
                    RelativeSizeAxes = Axes.Both,
                    Height = 0.35f
                },
                new Container
                {
                    RelativeSizeAxes = Axes.Both,
                    Colour = colors.Gray5,
                    Children = new Drawable[]
                    {
                        new Circle
                        {
                            Anchor = Anchor.CentreLeft,
                            Origin = Anchor.CentreRight,
                            Size = new Vector2(5)
                        },
                        new Box
                        {
                            Anchor = Anchor.CentreLeft,
                            Origin = Anchor.CentreLeft,
                            RelativeSizeAxes = Axes.X,
                            Height = 1,
                            EdgeSmoothness = new Vector2(0, 1),
                        },
                        new Circle
                        {
                            Anchor = Anchor.CentreRight,
                            Origin = Anchor.CentreLeft,
                            Size = new Vector2(5)
                        },
                    }
                }
            };
        }
    }
}
