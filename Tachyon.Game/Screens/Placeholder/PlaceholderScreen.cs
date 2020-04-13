using osu.Framework.Allocation;
using osu.Framework.Extensions.Color4Extensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osuTK;
using Tachyon.Game.Graphics;
using Tachyon.Game.Graphics.Sprites;
using Tachyon.Game.Screens.Backgrounds;

namespace Tachyon.Game.Screens.Placeholder
{
    public class PlaceholderScreen : TachyonScreen
    {
        private const double transition_time = 1000;
        private readonly bool transparentBackground;

        public PlaceholderScreen(string title, bool transparentBackground = false)
        {
            this.transparentBackground = transparentBackground;
            
            InternalChild = new PlaceholderMessage(title);
        }
        
        protected override BackgroundScreen CreateBackground() => transparentBackground ? (BackgroundScreen) new TransparentScreen() : new TextureBackgroundScreen();

        private class PlaceholderMessage : CompositeDrawable
        {
            private readonly Container boxContainer;
            private readonly Box box;

            public PlaceholderMessage(string screenName)
            {
                RelativeSizeAxes = Axes.Both;
                Size = new Vector2(0.5f, 0.2f);
                Anchor = Anchor.Centre;
                Origin = Anchor.Centre;

                InternalChildren = new Drawable[]
                {
                    boxContainer = new Container
                    {
                        CornerRadius = 20,
                        Masking = true,
                        RelativeSizeAxes = Axes.Both,
                        Anchor = Anchor.Centre,
                        Origin = Anchor.Centre,
                        Children = new Drawable[]
                        {
                            box = new Box
                            {
                                RelativeSizeAxes = Axes.Both,
                                Alpha = 0.2f,
                                Blending = BlendingParameters.Additive,
                            },
                            new FillFlowContainer
                            {
                                AutoSizeAxes = Axes.Both,
                                Anchor = Anchor.Centre,
                                Origin = Anchor.Centre,
                                Direction = FillDirection.Vertical,
                                Children = new Drawable[]
                                {
                                    new TachyonSpriteText
                                    {
                                        Anchor = Anchor.TopCentre,
                                        Origin = Anchor.TopCentre,
                                        Text = $"Should be {screenName} here",
                                        Font = TachyonFont.GetFont(size: 26, weight: FontWeight.Bold),
                                    },
                                    new TachyonSpriteText
                                    {
                                        Anchor = Anchor.TopCentre,
                                        Origin = Anchor.TopCentre,
                                        Text = "Still under development! Please wait...",
                                        Font = TachyonFont.GetFont(size: 18),
                                    },
                                }
                            },
                        }
                    },
                };
            }
            
            [BackgroundDependencyLoader]
            private void load(TachyonColor color)
            {
                box.Colour = color.RedSomething.Opacity(0.75f);
            }

            protected override void LoadComplete()
            {
                base.LoadComplete();

                boxContainer.Hide();
                boxContainer.ScaleTo(0.5f);

                using (BeginDelayedSequence(300, true))
                {
                    boxContainer.ScaleTo(1, transition_time, Easing.OutBack);
                    boxContainer.FadeIn(transition_time, Easing.OutExpo);
                }
            }
        }
    }
}