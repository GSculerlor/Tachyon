using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Colour;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osuTK;
using osuTK.Graphics;
using Tachyon.Game.Beatmaps;
using Tachyon.Game.Graphics;
using Tachyon.Game.Graphics.Sprites;

namespace Tachyon.Game.Screens.Playground.Carousel
{
    public class DrawableCarouselBeatmap : DrawableCarouselItem
    {
        private readonly BeatmapInfo beatmap;

        private Sprite background;

        public DrawableCarouselBeatmap(CarouselBeatmap panel)
            : base(panel)
        {
            beatmap = panel.Beatmap;
            Height *= 0.60f;
        }

        [BackgroundDependencyLoader(true)]
        private void load(BeatmapManager manager)
        {
            Children = new Drawable[]
            {
                background = new Box
                {
                    RelativeSizeAxes = Axes.Both,
                },
                new FillFlowContainer
                {
                    Padding = new MarginPadding(5),
                    Direction = FillDirection.Horizontal,
                    AutoSizeAxes = Axes.Both,
                    Anchor = Anchor.CentreLeft,
                    Origin = Anchor.CentreLeft,
                    Children = new Drawable[]
                    {
                        new FillFlowContainer
                        {
                            Padding = new MarginPadding { Left = 5 },
                            Direction = FillDirection.Vertical,
                            AutoSizeAxes = Axes.Both,
                            Children = new Drawable[]
                            {
                                new FillFlowContainer
                                {
                                    Direction = FillDirection.Horizontal,
                                    Spacing = new Vector2(4, 0),
                                    AutoSizeAxes = Axes.Both,
                                    Children = new[]
                                    {
                                        new TachyonSpriteText
                                        {
                                            Text = beatmap.Version,
                                            Font = TachyonFont.GetFont(size: 20),
                                            Anchor = Anchor.BottomLeft,
                                            Origin = Anchor.BottomLeft
                                        },
                                    }
                                }
                            }
                        }
                    }
                }
            };
        }

        protected override void Selected()
        {
            base.Selected();

            background.Colour = ColourInfo.GradientVertical(
                new Color4(20, 43, 51, 255),
                new Color4(40, 86, 102, 255));
        }

        protected override void Deselected()
        {
            base.Deselected();

            background.Colour = new Color4(20, 43, 51, 255);
        }
    }
}
