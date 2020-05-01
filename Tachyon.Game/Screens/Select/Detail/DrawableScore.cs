using System.Collections.Generic;
using System.Linq;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Cursor;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Input.Events;
using osuTK;
using osuTK.Graphics;
using Tachyon.Game.Graphics;
using Tachyon.Game.Graphics.Sprites;
using Tachyon.Game.Scoring;

namespace Tachyon.Game.Screens.Select.Detail
{
    public class DrawableScore : Container
    {
        public const float HEIGHT = 80;

        private const float corner_radius = 5;
        private const float background_alpha = 0.25f;

        private readonly ScoreInfo score;

        private Box background;
        private Container content;
        private TachyonSpriteText scoreLabel;

        private List<ScoreComponentLabel> statisticsLabels;

        public DrawableScore(ScoreInfo score)
        {
            this.score = score;

            RelativeSizeAxes = Axes.X;
            Height = HEIGHT;
        }

        [BackgroundDependencyLoader]
        private void load(TachyonColor colors)
        {
            statisticsLabels = GetStatistics(score).Select(s => new ScoreComponentLabel(s)).ToList();

            Children = new Drawable[]
            {
                content = new Container
                {
                    RelativeSizeAxes = Axes.Both,
                    Children = new Drawable[]
                    {
                        new Container
                        {
                            RelativeSizeAxes = Axes.Both,
                            CornerRadius = corner_radius,
                            Masking = true,
                            Children = new[]
                            {
                                background = new Box
                                {
                                    RelativeSizeAxes = Axes.Both,
                                    Colour = Color4.Black,
                                    Alpha = background_alpha,
                                },
                            },
                        },
                        new FillFlowContainer
                        {
                            RelativeSizeAxes = Axes.Both,
                            Anchor = Anchor.TopCentre,
                            Origin = Anchor.TopCentre,
                            Direction = FillDirection.Vertical,
                            Spacing = new Vector2(0f, 8f),
                            Children = new Drawable[]
                            { 
                                new FillFlowContainer
                                {
                                    AutoSizeAxes = Axes.Both,
                                    Anchor = Anchor.Centre,
                                    Origin = Anchor.Centre,
                                    Direction = FillDirection.Horizontal,
                                    Spacing = new Vector2(5f, 0f),
                                    Children = new Drawable[]
                                    {
                                        scoreLabel = new TachyonSpriteText
                                        {
                                            Colour = Color4.White,
                                            Text = score.TotalScore.ToString(@"N0"),
                                            Font = TachyonFont.Numeric.With(size: 23),
                                        },
                                    },
                                },
                                new Container
                                {
                                    Anchor = Anchor.Centre,
                                    Origin = Anchor.Centre,
                                    AutoSizeAxes = Axes.Both,
                                    Children = new Drawable[]
                                    {
                                        new FillFlowContainer
                                        {
                                            Origin = Anchor.BottomCentre,
                                            Anchor = Anchor.BottomCentre,
                                            AutoSizeAxes = Axes.Both,
                                            Direction = FillDirection.Horizontal,
                                            Spacing = new Vector2(10f, 0f),
                                            Children = statisticsLabels
                                        },
                                    },
                                },
                            },
                        },
                    },
                },
            };
        }

        public override void Show()
        {
            foreach (var d in new Drawable[] { scoreLabel }.Concat(statisticsLabels))
                d.FadeOut();

            Alpha = 0;

            content.MoveToY(75);

            this.FadeIn(200);
            content.MoveToY(0, 800, Easing.OutQuint);

            using (BeginDelayedSequence(100, true))
            {
                using (BeginDelayedSequence(250, true))
                {
                    scoreLabel.FadeIn(200);

                    using (BeginDelayedSequence(50, true))
                    {
                        var drawables = new Drawable[] {  }.Concat(statisticsLabels).ToArray();
                        for (int i = 0; i < drawables.Length; i++)
                            drawables[i].FadeIn(100 + i * 50);
                    }
                }
            }
        }

        public IEnumerable<LeaderboardScoreStatistic> GetStatistics(ScoreInfo model) => new[]
        {
            new LeaderboardScoreStatistic(FontAwesome.Solid.Link, "Max Combo", model.MaxCombo.ToString()),
            new LeaderboardScoreStatistic(FontAwesome.Solid.Crosshairs, "Accuracy", model.DisplayAccuracy)
        };

        protected override bool OnHover(HoverEvent e)
        {
            background.FadeTo(0.5f, 300, Easing.OutQuint);
            return base.OnHover(e);
        }

        protected override void OnHoverLost(HoverLostEvent e)
        {
            background.FadeTo(background_alpha, 200, Easing.OutQuint);
            base.OnHoverLost(e);
        }

        private class ScoreComponentLabel : Container, IHasTooltip
        {
            private const float icon_size = 20;
            private readonly FillFlowContainer content;

            public override bool Contains(Vector2 screenSpacePos) => content.Contains(screenSpacePos);

            public string TooltipText { get; }

            public ScoreComponentLabel(LeaderboardScoreStatistic statistic)
            {
                TooltipText = statistic.Name;
                AutoSizeAxes = Axes.Both;

                Child = content = new FillFlowContainer
                {
                    AutoSizeAxes = Axes.Both,
                    Direction = FillDirection.Horizontal,
                    Spacing = new Vector2(10, 0),
                    Children = new Drawable[]
                    {
                        new Container
                        {
                            Anchor = Anchor.CentreLeft,
                            Origin = Anchor.CentreLeft,
                            AutoSizeAxes = Axes.Both,
                            Children = new[]
                            {
                                new SpriteIcon
                                {
                                    Anchor = Anchor.Centre,
                                    Origin = Anchor.Centre,
                                    Size = new Vector2(icon_size - 6),
                                    Colour = Color4.White,
                                    Icon = statistic.Icon,
                                },
                            },
                        },
                        new TachyonSpriteText
                        {
                            Anchor = Anchor.CentreLeft,
                            Origin = Anchor.CentreLeft,
                            Colour = Color4.White,
                            Text = statistic.Value,
                            Font = TachyonFont.GetFont(size: 20, weight: FontWeight.Bold),
                        },
                    },
                };
            }
        }

        public class LeaderboardScoreStatistic
        {
            public IconUsage Icon;
            public readonly string Value;
            public readonly string Name;

            public LeaderboardScoreStatistic(IconUsage icon, string name, string value)
            {
                Icon = icon;
                Name = name;
                Value = value;
            }
        }
    }
}
