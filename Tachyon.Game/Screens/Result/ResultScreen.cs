using osu.Framework.Allocation;
using osu.Framework.Extensions.Color4Extensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Effects;
using osu.Framework.Screens;
using osuTK.Graphics;
using Tachyon.Game.Graphics;
using Tachyon.Game.Graphics.UserInterface;
using Tachyon.Game.Scoring;
using Tachyon.Game.Screens.Backgrounds;
using Tachyon.Game.Screens.Play;

namespace Tachyon.Game.Screens.Result
{
    public class ResultScreen : TachyonScreen
    {
        public override bool DisallowExternalBeatmapChanges => true;
        
        public readonly ScoreInfo Score;

        private FillFlowContainer<OverlayButton> buttons;

        [Resolved(CanBeNull = true)]
        private Player player { get; set; }

        public ResultScreen(ScoreInfo score)
        {
            Score = score;
        }

        [BackgroundDependencyLoader]
        private void load(TachyonColor colors)
        {
            InternalChildren = new Drawable[]
            {
                new Container
                { 
                    RelativeSizeAxes = Axes.Both,
                    Child = new ResultPanel(Score)
                    {
                        Origin = Anchor.Centre,
                        Anchor = Anchor.Centre,
                    }
                },
                new Container
                {
                    Anchor = Anchor.BottomLeft,
                    Origin = Anchor.BottomLeft,
                    RelativeSizeAxes = Axes.X,
                    Height = 50,
                    Children = new Drawable[]
                    {
                        buttons = new FillFlowContainer<OverlayButton>
                        {
                            Origin = Anchor.TopCentre,
                            Anchor = Anchor.TopCentre,
                            RelativeSizeAxes = Axes.X,
                            AutoSizeAxes = Axes.Y,
                            Direction = FillDirection.Vertical,
                            Masking = true,
                            EdgeEffect = new EdgeEffectParameters
                            {
                                Type = EdgeEffectType.Shadow,
                                Colour = Color4.Black.Opacity(0.6f),
                                Radius = 50
                            },
                        }
                    }
                }
            };
            
            if (player != null)
            {
                buttons.Add(new OverlayButton 
                    {
                        Text = "Retry",
                        ButtonColor = colors.SecondaryDark,
                        Origin = Anchor.TopCentre,
                        Anchor = Anchor.TopCentre,
                        Height = 50,
                        Action = () => player.Restart()
                    }
                );
            }
        }
        
        protected override BackgroundScreen CreateBackground() => new BeatmapBackgroundScreen(Beatmap.Value);
        
        public override void OnEntering(IScreen last)
        {
            base.OnEntering(last);

            Background.FadeTo(0.5f, 250);
        }

        public override bool OnExiting(IScreen next)
        {
            Background.FadeTo(1, 250);

            return base.OnExiting(next);
        }
    }
}
