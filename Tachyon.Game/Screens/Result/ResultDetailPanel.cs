using System.Collections.Generic;
using System.Linq;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osuTK;
using Tachyon.Game.Graphics.UserInterface;
using Tachyon.Game.Scoring;

namespace Tachyon.Game.Screens.Result
{
    public class ResultDetailPanel : CompositeDrawable
    {
        private readonly ScoreInfo score;
        private readonly List<StatisticDisplay> statisticDisplays = new List<StatisticDisplay>();
        
        private RollingCounter<long> scoreCounter;
        private AccuracyCounter accuracyCounter;

        public ResultDetailPanel(ScoreInfo score)
        {
            this.score = score;

            RelativeSizeAxes = Axes.Both;
            Masking = true;

            Padding = new MarginPadding { Vertical = 10, Horizontal = 10 };
        }

        [BackgroundDependencyLoader]
        private void load()
        {
            var bottomStatistics = new List<StatisticDisplay>();
            foreach (var stat in score.SortedStatistics)
                bottomStatistics.Add(new HitResultStatistic(stat.Key, stat.Value));
            
            statisticDisplays.AddRange(bottomStatistics);
            
            InternalChild = new FillFlowContainer
            {
                RelativeSizeAxes = Axes.Both,
                Direction = FillDirection.Vertical,
                Spacing = new Vector2(20),
                Children = new Drawable[]
                {
                    new FillFlowContainer
                    {
                        Anchor = Anchor.TopCentre,
                        Origin = Anchor.TopCentre,
                        RelativeSizeAxes = Axes.X,
                        AutoSizeAxes = Axes.Y,
                        Direction = FillDirection.Horizontal,
                        Children = new Drawable[]
                        {
                            scoreCounter = new ScoreCounter
                            {
                                Margin = new MarginPadding { Top = 20 },
                                Current = { Value = 0 },
                                Alpha = 0,
                                AlwaysPresent = true
                            },
                        }
                    },
                    accuracyCounter = new AccuracyCounter(score.Accuracy)
                    {
                        Alpha = 0,
                        AlwaysPresent = true
                    },
                    new FillFlowContainer
                    {
                        RelativeSizeAxes = Axes.X,
                        AutoSizeAxes = Axes.Y,
                        Direction = FillDirection.Vertical,
                        Spacing = new Vector2(0, 5),
                        Children = new Drawable[]
                        {
                            new GridContainer
                            {
                                RelativeSizeAxes = Axes.X,
                                AutoSizeAxes = Axes.Y,
                                Content = new[] { bottomStatistics.Cast<Drawable>().ToArray() },
                                RowDimensions = new[]
                                {
                                    new Dimension(GridSizeMode.AutoSize),
                                }
                            }
                        }
                    },
                }
            };
        }
        
        protected override void LoadComplete()
        {
            base.LoadComplete();

            // Score counter value setting must be scheduled so it isn't transferred instantaneously
            ScheduleAfterChildren(() =>
            {
                using (BeginDelayedSequence(300, true))
                {
                    scoreCounter.FadeIn();
                    scoreCounter.Current.Value = score.TotalScore;
                    
                    accuracyCounter.FadeIn();
                    accuracyCounter.Appear();

                    double delay = 0;

                    foreach (var stat in statisticDisplays)
                    {
                        using (BeginDelayedSequence(delay, true))
                            stat.Appear();

                        delay += 200;
                    }
                }
            });
        }
    }
}
