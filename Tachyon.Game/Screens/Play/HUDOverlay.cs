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
        
        private readonly DrawableRuleset drawableRuleset;
        private readonly ScoreProcessor scoreProcessor;


        public HUDOverlay(ScoreProcessor scoreProcessor, DrawableRuleset drawableRuleset)
        {
            this.scoreProcessor = scoreProcessor;
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
                                ScoreCounter = new ScoreCounter
                                {
                                    TextSize = 24,
                                    Anchor = Anchor.TopRight,
                                    Origin = Anchor.TopRight,
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
                                }
                            },
                        },
                        Progress = new BeatmapProgress
                        {
                            Anchor = Anchor.TopLeft,
                            Origin = Anchor.TopLeft,
                            RelativeSizeAxes = Axes.X,
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
    }
}
