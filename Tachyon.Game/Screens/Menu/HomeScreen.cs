using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Extensions.Color4Extensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Input.Events;
using osuTK;
using osuTK.Graphics;
using Tachyon.Game.Graphics;
using Tachyon.Game.Graphics.Sprites;
using Tachyon.Game.Overlays.Toolbar;

namespace Tachyon.Game.Screens.Menu
{
    /*public class HomeScreen : TachyonScreen
    {
        private const int button_height = 80;

        [Resolved]
        private Bindable<Toolbar.MenuScreen> menuScreen { get; set; }

        [BackgroundDependencyLoader]
        private void load(TachyonColor colors)
        {
            AddInternal(new GridContainer
            {
                Name = @"HomeScreen content container",
                RelativeSizeAxes = Axes.Both,
                ColumnDimensions = new []
                {
                    new Dimension(),
                    new Dimension(),
                    new Dimension(maxSize: 400),
                },
                Content = new []
                {
                    new Drawable[]
                    {
                        new FillFlowContainer
                        {
                            RelativeSizeAxes = Axes.Both,
                            Padding = new MarginPadding { Left = 40 },
                            Origin = Anchor.TopLeft,
                            Anchor = Anchor.TopLeft,
                            Direction = FillDirection.Vertical,
                            Spacing = new Vector2(8, 8),
                            Children = new Drawable[]
                            {
                                new PlaygroundButton
                                {
                                    ButtonIcon = FontAwesome.Solid.Meteor,
                                    ButtonText = @"PLAYGROUND",
                                    ButtonColor = colors.BlueDark,
                                    Action = () => gotoScreen(Toolbar.MenuScreen.Playground)
                                },
                                new GridContainer
                                {
                                    Origin = Anchor.Centre,
                                    Anchor = Anchor.Centre,
                                    RelativeSizeAxes = Axes.X,
                                    Height = button_height,
                                    ColumnDimensions = new []
                                    {
                                        new Dimension(),
                                        new Dimension()
                                    },
                                    Content = new [] { 
                                        new Drawable[]
                                        {
                                            new PlaygroundButton
                                            {
                                                ButtonIcon = FontAwesome.Solid.Edit,
                                                ButtonText = string.Empty,
                                                ButtonColor = colors.Gray2,
                                                Padding = new MarginPadding { Right = 4 },
                                                Action = () => gotoScreen(Toolbar.MenuScreen.Editor)
                                            },
                                            new PlaygroundButton
                                            {
                                                ButtonIcon = FontAwesome.Solid.Cog,
                                                ButtonText = string.Empty,
                                                ButtonColor = colors.Gray2,
                                                Padding = new MarginPadding { Left = 4 },
                                                Action = () => gotoScreen(Toolbar.MenuScreen.Settings)
                                            },
                                        }, 
                                    }
                                }, 
                                new DummyNewsBanner(),
                            }
                        }, 
                        new Container(), 
                        new Container()
                    }, 
                }
            });
        }

        private void gotoScreen(Toolbar.MenuScreen destination)
        {
            menuScreen.Value = destination;
        }

        private class PlaygroundButton : ClickableContainer
        {       
            public string ButtonText { get; set; }
            
            public IconUsage ButtonIcon { get; set; }
            
            public Color4 ButtonColor { get; set; } 
            
            private Box bottomStrip;

            public PlaygroundButton()
            {
                RelativeSizeAxes = Axes.X;
                Origin = Anchor.Centre;
                Anchor = Anchor.Centre;
                Height = button_height;
            }
            
            [BackgroundDependencyLoader]
            private void load(TachyonColor colors)
            {
                Children = new Drawable[]
                {
                    new Container
                    {
                        RelativeSizeAxes = Axes.Both,
                        Children = new Drawable[]
                        {
                            new Box
                            {
                                RelativeSizeAxes = Axes.Both,
                                Colour = ButtonColor.Opacity(0.8f),
                            },
                            new FillFlowContainer
                            {
                                Direction = FillDirection.Horizontal,
                                Spacing = new Vector2(10),
                                Anchor = Anchor.TopCentre,
                                Origin = Anchor.TopCentre,
                                RelativeSizeAxes = Axes.Y,
                                AutoSizeAxes = Axes.X,
                                Children = new Drawable[]
                                {
                                    new SpriteIcon
                                    {
                                        Anchor = Anchor.CentreLeft,
                                        Origin = Anchor.CentreLeft,
                                        Size = new Vector2(30),
                                        Icon = ButtonIcon
                                    },
                                    new TachyonSpriteText
                                    {
                                        Anchor = Anchor.CentreLeft,
                                        Origin = Anchor.CentreLeft,
                                        Margin = new MarginPadding { Bottom = 4, Left = 6 },
                                        Font = TachyonFont.GetFont(size: 36, weight: FontWeight.Bold),
                                        Text = ButtonText
                                    },
                                }
                            },
                        }
                    },
                    bottomStrip = new Box
                    {
                        RelativeSizeAxes = Axes.X,
                        Anchor = Anchor.BottomCentre,
                        Origin = Anchor.BottomCentre,
                        Height = 4f,
                        Colour = Color4.White,
                        Alpha = 0,
                    }
                };
            }
            
            protected override bool OnHover(HoverEvent e)
            {
                bottomStrip.FadeIn(700, Easing.OutQuint);
                
                return base.OnHover(e);
            }

            protected override void OnHoverLost(HoverLostEvent e)
            {
                bottomStrip.FadeOut(500, Easing.OutQuint);
                
                base.OnHoverLost(e);
            }
        }
        
        private class DummyNewsBanner : Container
        {
            private Box bottomStrip;

            public DummyNewsBanner()
            {
                RelativeSizeAxes = Axes.X;
                Origin = Anchor.Centre;
                Anchor = Anchor.Centre;
                Height = button_height * 3;
            }
            
            [BackgroundDependencyLoader]
            private void load(TachyonColor colors)
            {
                Children = new Drawable[]
                {
                    new Container
                    {
                        RelativeSizeAxes = Axes.Both,
                        Children = new Drawable[]
                        {
                            new Box
                            {
                                RelativeSizeAxes = Axes.Both,
                                Colour = Color4.Black.Opacity(0.45f)
                            },
                            new SpriteIcon
                            {
                                Anchor = Anchor.Centre,
                                Origin = Anchor.Centre,
                                Size = new Vector2(80),
                                Icon = FontAwesome.Solid.Newspaper,
                                Colour = Color4.White.Opacity(0.2f)
                            },
                        }
                    },
                    bottomStrip = new Box
                    {
                        RelativeSizeAxes = Axes.X,
                        Anchor = Anchor.BottomCentre,
                        Origin = Anchor.BottomCentre,
                        Height = 4f,
                        Colour = Color4.White,
                        Alpha = 0,
                    }
                };
            }
            
            protected override bool OnHover(HoverEvent e)
            {
                bottomStrip.FadeIn(700, Easing.OutQuint);
                
                return base.OnHover(e);
            }

            protected override void OnHoverLost(HoverLostEvent e)
            {
                bottomStrip.FadeOut(500, Easing.OutQuint);
                
                base.OnHoverLost(e);
            }
        }
    }*/
}
