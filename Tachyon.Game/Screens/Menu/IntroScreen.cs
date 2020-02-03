using osu.Framework.Allocation;
using osu.Framework.Extensions.Color4Extensions;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Colour;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Input.Events;
using osuTK;
using osuTK.Graphics;
using Tachyon.Game.Graphics;
using Tachyon.Game.Graphics.Containers;
using Tachyon.Game.Screens.Backgrounds;

namespace Tachyon.Game.Screens.Menu
{
    public class IntroScreen : TachyonScreen
    {
        private const int start_button_height = 100;
        
        public override bool AllowBackButton => false;
        
        public IntroScreen()
        {
            ValidForResume = false;
            
            AddInternal(new Container
            {
                RelativeSizeAxes = Axes.Both,
                Children = new Drawable[]
                {
                    new IntroBackgroundImage(),
                    new StartButton()
                },
            });
        }
        
        protected override BackgroundScreen CreateBackground() => new BackgroundScreenBlack();
        
        private class IntroBackgroundImage : Background
        {
            private const string textureName = @"Characters/Exusiai";

            public IntroBackgroundImage() : base(textureName)
            {
                Anchor = Anchor.Centre;
                Origin = Anchor.Centre;
                RelativeSizeAxes = Axes.Both;
                Padding = new MarginPadding { Bottom = start_button_height };
                Masking = true;
                MaskingSmoothness = 0;
                
                AddRangeInternal(new Drawable[]
                {
                    new Box
                    {
                        Anchor = Anchor.BottomCentre,
                        Origin = Anchor.BottomCentre,
                        RelativeSizeAxes = Axes.Both,
                        Colour = ColourInfo.GradientVertical(Color4.Black.Opacity(0.2f), Color4.Black.Opacity(1f))
                    },
                    new FillFlowContainer
                    {
                        Anchor = Anchor.BottomLeft,
                        Origin = Anchor.BottomLeft,
                        RelativeSizeAxes = Axes.X,
                        AutoSizeAxes = Axes.Y,
                        Direction = FillDirection.Vertical,
                        Padding = new MarginPadding { Horizontal = 25, Vertical = 5},
                        Children = new Drawable[]
                        {
                            new SpriteText
                            {
                                Margin = new MarginPadding
                                {
                                    Bottom = 5,
                                },
                                Font = TachyonFont.GetFont(Typeface.Exo, 20, FontWeight.Bold),
                                Text = @"Tachyon",
                            },
                            new TextFlowContainer(text => { text.Font = text.Font.With(size: 14); })
                            {
                                RelativeSizeAxes = Axes.X,
                                AutoSizeAxes = Axes.Y,
                                Text = @"A tachyon is any hypothetical particle that can travel faster than the speed of light. The term tachyon was coined by Gerald Feinberg in 1967. Most scientists do not believe that tachyons exist. Einstein's theory of special relativity states that nothing can accelerate faster than the speed of light, while tachyons are theorized to be constantly traveling faster than the speed of light. If a tachyon did exist, it would have an imaginary number as its mass. Gaada hubungannya sama gambar, emang cuma buat pemanis doang sih itu."
                            }
                        }
                    }
                });
            }
        }

        private class StartButton : ClickableContainer
        {
            private Color4 colorNormal;
            private Color4 colorHover;
            private Box box;
            private AspectRatioContainer container;

            public StartButton()
            {
                Origin = Anchor.BottomCentre;
                RelativeSizeAxes = Axes.X;
                RelativePositionAxes = Axes.Both;
                Position = new Vector2(0.5f, 1f);
                Size = new Vector2(1, start_button_height);
            }

            [BackgroundDependencyLoader]
            private void load(TachyonColor color)
            {
                colorNormal = color.Yellow;
                colorHover = color.YellowDark;

                Children = new Drawable[]
                {
                    new Box
                    {
                        Anchor = Anchor.BottomCentre,
                        Origin = Anchor.BottomCentre,
                        RelativeSizeAxes = Axes.X,
                        Height = start_button_height,
                        Colour = Color4.Black
                    },
                    container = new AspectRatioContainer
                    {
                        Anchor = Anchor.Centre,
                        Origin = Anchor.Centre,
                        RelativeSizeAxes = Axes.Y,
                        Height = 0.4f,
                        Masking = true,
                        Multiplier = 1,
                        Children = new Drawable[]
                        {
                            box = new Box
                            {
                                Origin = Anchor.Centre,
                                Anchor = Anchor.Centre,
                                RelativeSizeAxes = Axes.Both,
                                Colour = colorNormal,
                            },
                            new SpriteIcon
                            {
                                Origin = Anchor.Centre,
                                Anchor = Anchor.Centre,
                                Size = new Vector2(15), 
                                Shadow = true, 
                                Icon = FontAwesome.Solid.ChevronRight
                            },
                        }
                    }
                };
            }

            protected override bool OnHover(HoverEvent e)
            {
                box.FadeColour(colorHover, 500, Easing.OutQuint);
                return true;
            }

            protected override void OnHoverLost(HoverLostEvent e)
            {
                box.FadeColour(colorNormal, 500, Easing.OutQuint);
                base.OnHoverLost(e);
            }

            protected override bool OnMouseDown(MouseDownEvent e)
            {
                container.ScaleTo(0.75f, 500, Easing.OutQuint);
                return base.OnMouseDown(e);
            }

            protected override void OnMouseUp(MouseUpEvent e)
            {
                container.ScaleTo(1, 1000, Easing.OutElastic);
                base.OnMouseUp(e);
            }
        }
    }
}