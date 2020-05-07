using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osuTK;
using Tachyon.Game.Graphics;
using Tachyon.Game.Graphics.Sprites;
using Tachyon.Game.Graphics.UserInterface;
using Tachyon.Game.Rulesets.Scoring;
using Tachyon.Game.Rulesets.UI;
using Tachyon.Game.Screens.Play.HUD;

namespace Tachyon.Game.Screens.Play
{
    public class HUDOverlay : Container
    {
        public readonly BeatmapProgress Progress;
        public readonly RollingCounter<int> ComboCounter;
        public readonly RollingCounter<double> ScoreCounter;
        public readonly HealthBar HealthBar;
        public readonly PauseButton Pause;
        public readonly BeatmapTitle Title;
        
        private readonly DrawableRuleset drawableRuleset;
        private readonly ScoreProcessor scoreProcessor;
        private readonly HealthProcessor healthProcessor;


        public HUDOverlay(ScoreProcessor scoreProcessor, HealthProcessor healthProcessor, DrawableRuleset drawableRuleset)
        {
            this.scoreProcessor = scoreProcessor;
            this.healthProcessor = healthProcessor;
            this.drawableRuleset = drawableRuleset;
            
            RelativeSizeAxes = Axes.Both;
            
            Children = new Drawable[]
            {
                new Container
                {
                    RelativeSizeAxes = Axes.Both,
                    Children = new Drawable[]
                    {
                        new Container
                        {
                            Anchor = Anchor.TopCentre,
                            Origin = Anchor.TopCentre,
                            AutoSizeAxes = Axes.Y,
                            RelativeSizeAxes = Axes.X,
                            Children = new Drawable[]
                            {
                                Pause = new PauseButton
                                {
                                    AutoSizeAxes = Axes.X,
                                    Height = 24,
                                    Anchor = Anchor.TopLeft,
                                    Origin = Anchor.TopLeft,
                                    Margin = new MarginPadding { Top = 25, Horizontal = 20 }
                                },
                                new FillFlowContainer
                                {
                                    Anchor = Anchor.TopCentre,
                                    Origin = Anchor.TopCentre,
                                    AutoSizeAxes = Axes.Both,
                                    Direction = FillDirection.Vertical,
                                    Margin = new MarginPadding { Top = 20, Horizontal = 20 },
                                    Children = new Drawable[]
                                    {
                                        new TachyonSpriteText
                                        {
                                            Text = "COMBO",
                                            Spacing = new Vector2(8, 0),
                                            Font = TachyonFont.Default.With(size: 24, weight: FontWeight.SemiBold)
                                        },
                                        ComboCounter = new ComboCounter
                                        {
                                            TextSize = 24,
                                            BypassAutoSizeAxes = Axes.X,
                                            Anchor = Anchor.TopCentre,
                                            Origin = Anchor.TopCentre,
                                            Margin = new MarginPadding { Top = 8 }
                                        }
                                    }
                                },
                                ScoreCounter = new ScoreCounter
                                {
                                    TextSize = 24,
                                    Anchor = Anchor.TopRight,
                                    Origin = Anchor.TopRight,
                                    Margin = new MarginPadding { Top = 25, Horizontal = 20 }
                                },
                            },
                        },
                        Progress = new BeatmapProgress
                        {
                            Anchor = Anchor.TopLeft,
                            Origin = Anchor.TopLeft,
                            RelativeSizeAxes = Axes.X,
                        },
                        Title = new BeatmapTitle
                        {
                            Margin = new MarginPadding { Left = 20, Bottom = 15 },
                            Anchor = Anchor.BottomLeft,
                            Origin = Anchor.BottomLeft,
                        },
                        HealthBar = new HealthBar
                        {
                            Size = new Vector2(1, 5),
                            RelativeSizeAxes = Axes.X,
                            Anchor = Anchor.BottomLeft,
                            Origin = Anchor.BottomLeft,
                        }
                    }
                }
            };
        }

        [BackgroundDependencyLoader]
        private void load()
        {
            if (scoreProcessor != null)
                BindScoreProcessor(scoreProcessor);
            
            if (healthProcessor != null)
                BindHealthProcessor(healthProcessor);
            
            if (drawableRuleset != null)
            {
                Progress.Objects = drawableRuleset.Objects;
                Progress.Show();
            }
        }
        
        protected virtual void BindScoreProcessor(ScoreProcessor processor)
        {
            ScoreCounter?.Current.BindTo(processor.TotalScore);
            ComboCounter?.Current.BindTo(processor.Combo);
        }
        
        protected virtual void BindHealthProcessor(HealthProcessor processor)
        {
            HealthBar?.BindHealthProcessor(processor);
        }
    }
}
