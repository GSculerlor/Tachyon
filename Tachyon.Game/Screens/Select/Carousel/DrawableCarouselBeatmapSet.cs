using System.Linq;
 using osu.Framework.Allocation;
 using osu.Framework.Graphics;
 using osu.Framework.Graphics.Colour;
 using osu.Framework.Graphics.Containers;
 using osu.Framework.Graphics.Shapes;
 using osu.Framework.Localisation;
 using osuTK;
 using osuTK.Graphics;
 using Tachyon.Game.Beatmaps;
 using Tachyon.Game.Beatmaps.Drawables;
 using Tachyon.Game.Graphics;
 using Tachyon.Game.Graphics.Sprites;

 namespace Tachyon.Game.Screens.Select.Carousel
{
    public class DrawableCarouselBeatmapSet : DrawableCarouselItem
    {
        private readonly BeatmapSetInfo beatmapSet;

        public DrawableCarouselBeatmapSet(CarouselBeatmapSet set)
            : base(set)
        {
            beatmapSet = set.BeatmapSet;
        }

        [BackgroundDependencyLoader(true)]
        private void load(BeatmapManager manager)
        {
            Children = new Drawable[]
            {
                new DelayedLoadUnloadWrapper(() =>
                    {
                        var background = new PanelBackground(manager.GetWorkingBeatmap(beatmapSet.Beatmaps.FirstOrDefault()))
                        {
                            RelativeSizeAxes = Axes.Both,
                        };

                        background.OnLoadComplete += d => d.FadeInFromZero(1000, Easing.OutQuint);

                        return background;
                    }, 300, 5000
                ),
                new FillFlowContainer
                {
                    Direction = FillDirection.Vertical,
                    Padding = new MarginPadding { Vertical = 10, Horizontal = 20 },
                    AutoSizeAxes = Axes.Both,
                    Children = new Drawable[]
                    {
                        new TachyonSpriteText
                        {
                            Text = new LocalisedString((beatmapSet.Metadata.TitleUnicode, beatmapSet.Metadata.Title)),
                            Font = TachyonFont.GetFont(weight: FontWeight.Bold, size: 28),
                            Shadow = true,
                        },
                        new TachyonSpriteText
                        {
                            Text = new LocalisedString((beatmapSet.Metadata.ArtistUnicode, beatmapSet.Metadata.Artist)),
                            Font = TachyonFont.GetFont(weight: FontWeight.SemiBold, size: 20),
                            Shadow = true,
                        }
                    }
                }
            };
        }

        private class PanelBackground : BufferedContainer
        {
            public PanelBackground(WorkingBeatmap working)
            {
                CacheDrawnFrameBuffer = true;
                RedrawOnScale = false;

                Children = new Drawable[]
                {
                    new BeatmapBackgroundSprite(working)
                    {
                        RelativeSizeAxes = Axes.Both,
                        Anchor = Anchor.Centre,
                        Origin = Anchor.Centre,
                        FillMode = FillMode.Fill,
                    },
                    new FillFlowContainer
                    {
                        Depth = -1,
                        RelativeSizeAxes = Axes.Both,
                        Direction = FillDirection.Horizontal,
                        Shear = new Vector2(0.8f, 0),
                        Alpha = 0.5f,
                        Children = new[]
                        {
                            new Box
                            {
                                RelativeSizeAxes = Axes.Both,
                                Colour = Color4.Black,
                                Width = 0.4f,
                            },
                            new Box
                            {
                                RelativeSizeAxes = Axes.Both,
                                Colour = ColourInfo.GradientHorizontal(Color4.Black, new Color4(0f, 0f, 0f, 0.9f)),
                                Width = 0.05f,
                            },
                            new Box
                            {
                                RelativeSizeAxes = Axes.Both,
                                Colour = ColourInfo.GradientHorizontal(new Color4(0f, 0f, 0f, 0.9f), new Color4(0f, 0f, 0f, 0.1f)),
                                Width = 0.2f,
                            },
                            new Box
                            {
                                RelativeSizeAxes = Axes.Both,
                                Colour = ColourInfo.GradientHorizontal(new Color4(0f, 0f, 0f, 0.1f), new Color4(0, 0, 0, 0)),
                                Width = 0.05f,
                            },
                        }
                    },
                };
            }
        }
    }
}
