using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Colour;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Localisation;
using osuTK;
using Tachyon.Game.Beatmaps;
using Tachyon.Game.Graphics;
using Tachyon.Game.Graphics.Sprites;
using Tachyon.Game.Scoring;

namespace Tachyon.Game.Screens.Result
{
    public class ResultPanel : CompositeDrawable
    {
        private readonly ScoreInfo score;

        public ResultPanel(ScoreInfo score)
        {
            this.score = score;
            Size = new Vector2(600, 260);
        }

        [BackgroundDependencyLoader]
        private void load(TachyonColor colors, Bindable<WorkingBeatmap> working)
        {
            var beatmap = working.Value.BeatmapInfo;
            var metadata = beatmap.Metadata;
            
            InternalChild = new FillFlowContainer
            {
                RelativeSizeAxes = Axes.Both,
                Direction = FillDirection.Vertical,
                Spacing = new Vector2(0, -16),
                Children = new Drawable[]
                {
                    new Container
                    {
                        Name = "Top layer",
                        RelativeSizeAxes = Axes.Both,
                        Children = new Drawable[]
                        {
                            new Container
                            {
                                RelativeSizeAxes = Axes.Both,
                                CornerRadius = 10,
                                CornerExponent = 2.5f,
                                Masking = true,
                                Child = new Box {
                                    RelativeSizeAxes = Axes.Both,
                                    Colour = ColourInfo.GradientVertical(colors.Primary, colors.PrimaryDark)
                                }
                            },
                            new Container
                            {
                                RelativeSizeAxes = Axes.Both,
                                Child = new ResultDetailPanel(score)
                            }
                        }
                    },
                    new Container
                    {
                        Name = "Bottom layer",
                        RelativeSizeAxes = Axes.X,
                        Height = 120,
                        Children = new Drawable[]
                        {
                            new Container
                            {
                                RelativeSizeAxes = Axes.Both,
                                CornerRadius = 10,
                                CornerExponent = 2.5f,
                                Masking = true,
                                Child = new Box
                                {
                                    RelativeSizeAxes = Axes.Both,
                                    Colour = colors.PrimaryDark
                                }
                            },
                            new Container
                            {
                                RelativeSizeAxes = Axes.Both,
                                Child = new FillFlowContainer
                                {
                                    Anchor = Anchor.Centre,
                                    Origin = Anchor.Centre,
                                    RelativeSizeAxes = Axes.X,
                                    AutoSizeAxes = Axes.Y,
                                    Direction = FillDirection.Vertical,
                                    Children = new Drawable[]
                                    {
                                        new TachyonSpriteText
                                        {
                                            Anchor = Anchor.Centre,
                                            Origin = Anchor.Centre,
                                            Text = new LocalisedString((metadata.TitleUnicode, metadata.Title)),
                                            Font = TachyonFont.Default.With(size: 28, weight: FontWeight.Bold),
                                        },
                                        new TachyonSpriteText
                                        {
                                            Anchor = Anchor.Centre,
                                            Origin = Anchor.Centre,
                                            Text = $"{new LocalisedString((metadata.ArtistUnicode, metadata.Artist))} - [{beatmap.Version}]",
                                            Font = TachyonFont.Default.With(size: 24, weight: FontWeight.SemiBold)
                                        },
                                    }
                                }
                            }
                        }
                    }
                }
            };
        }
    }
}
