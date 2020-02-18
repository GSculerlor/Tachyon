using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Extensions.Color4Extensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Effects;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Graphics.Textures;
using osuTK;
using osuTK.Graphics;
using Tachyon.Game.Components;
using Tachyon.Game.Graphics;
using Tachyon.Game.Graphics.Containers;
using Tachyon.Game.Graphics.Sprites;
using Tachyon.Game.Graphics.UserInterface;

namespace Tachyon.Game.Overlays
{
    public class MusicPlayer : FocusedOverlayContainer
    {
        private const float music_player_height = 80;
        private const float music_detail_height = 60;
        private const float transition_length = 400;

        private Container musicPlayerContainer;
        private CircularContainer songJacketContainer;

        [Resolved]
        private MusicController musicController { get; set; }

        public MusicPlayer()
        {
            Width = 400;
            Height = music_detail_height + music_player_height;
            Margin = new MarginPadding(10);
        }

        [BackgroundDependencyLoader]
        private void load(TachyonColor color)
        {
            Children = new Drawable[]
            {
                musicPlayerContainer = new Container
                {
                    RelativeSizeAxes = Axes.Both,
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre,
                    Children = new Drawable[]
                    {
                        new Container
                        {
                            RelativeSizeAxes = Axes.X,
                            Width = 0.85f,
                            Origin = Anchor.TopCentre,
                            Anchor = Anchor.TopCentre,
                            Height = music_detail_height + 10,
                            Masking = true,
                            MaskingSmoothness = 0,
                            CornerRadius = 10,
                            Padding = new MarginPadding { Bottom = 10 },
                            Children = new Drawable[]
                            {
                                new Box
                                {
                                    RelativeSizeAxes = Axes.Both,
                                    Colour = color.BackButtonGray.Darken(0.2f),
                                },
                                new FillFlowContainer
                                {
                                    RelativeSizeAxes = Axes.Both,
                                    Direction = FillDirection.Vertical,
                                    Origin = Anchor.Centre,
                                    Anchor = Anchor.Centre,
                                    Children = new Drawable[]
                                    {
                                        new TachyonSpriteText
                                        {
                                            Origin = Anchor.CentreLeft,
                                            Anchor = Anchor.Centre,
                                            Font = TachyonFont.GetFont(size: 20, weight: FontWeight.Bold),
                                            Colour = Color4.White,
                                            Text = @"- ΓΛLLΞИ -",
                                        },
                                        new TachyonSpriteText
                                        {
                                            Origin = Anchor.CentreLeft,
                                            Anchor = Anchor.Centre,
                                            Font = TachyonFont.GetFont(size: 15, weight: FontWeight.Bold, italics: true),
                                            Colour = Color4.White,
                                            Text = @"かねこちはる",
                                        },
                                    }
                                }, 
                            }
                        },
                        new Container
                        {
                            RelativeSizeAxes = Axes.X,
                            Origin = Anchor.BottomCentre,
                            Anchor = Anchor.BottomCentre,
                            Height = music_player_height,
                            Masking = true,
                            MaskingSmoothness = 0,
                            CornerRadius = 10,
                            EdgeEffect = new EdgeEffectParameters
                            {
                                Type = EdgeEffectType.Shadow,
                                Radius = 10,
                                Colour = Color4.Black.Opacity(0.3f)
                            },
                            Children = new Drawable[]
                            {
                                new Box
                                {
                                    RelativeSizeAxes = Axes.Both,
                                    Colour = color.BackButtonGray,
                                },
                                new FillFlowContainer
                                {
                                    AutoSizeAxes = Axes.X,
                                    RelativeSizeAxes = Axes.Y,
                                    Direction = FillDirection.Horizontal,
                                    Spacing = new Vector2(30),
                                    Origin = Anchor.CentreLeft,
                                    Anchor = Anchor.Centre,
                                    Children = new Drawable[]
                                    {
                                        new MusicControllerButton
                                        {
                                            Anchor = Anchor.Centre,
                                            Origin = Anchor.Centre,
                                            Icon = FontAwesome.Solid.StepBackward
                                        },
                                        new MusicControllerButton
                                        {
                                            Anchor = Anchor.Centre,
                                            Origin = Anchor.Centre,
                                            Scale = new Vector2(1.3f),
                                            IconScale = new Vector2(1.3f),
                                            Icon = FontAwesome.Solid.Pause,
                                        },
                                        new MusicControllerButton
                                        {
                                            Anchor = Anchor.Centre,
                                            Origin = Anchor.Centre,
                                            Icon = FontAwesome.Solid.StepForward,
                                        },
                                    }
                                },
                            }
                        },
                        new AspectRatioContainer
                        {
                            Origin = Anchor.CentreRight,
                            Anchor = Anchor.Centre,
                            RelativeSizeAxes = Axes.Y,
                            Height = 0.75f,
                            Masking = true,
                            Multiplier = 1,
                            Margin = new MarginPadding { Right = 40 },
                            Child = songJacketContainer = new CircularContainer
                            {
                                Anchor = Anchor.Centre,
                                Origin = Anchor.Centre,
                                RelativeSizeAxes = Axes.Both,
                                Masking = true,
                                EdgeEffect = new EdgeEffectParameters
                                {
                                    Type = EdgeEffectType.Shadow,
                                    Radius = 2,
                                    Colour = Color4.Black.Opacity(0.2f)
                                },
                                Child = new SongJacket
                                {
                                    RelativeSizeAxes = Axes.Both
                                }
                            }, 
                        }
                    }
                }
            };
        }
        
        protected override void LoadComplete()
        {
            base.LoadComplete();
            
            songJacketContainer.Spin(10000, RotationDirection.Clockwise);
        }
        
        protected override void PopIn()
        {
            base.PopIn();

            this.FadeIn(transition_length, Easing.OutQuint);
            musicPlayerContainer.ScaleTo(1, transition_length, Easing.OutElastic);
        }

        protected override void PopOut()
        {
            base.PopOut();

            this.FadeOut(transition_length, Easing.OutQuint);
            musicPlayerContainer.ScaleTo(0.9f, transition_length, Easing.OutQuint);
        }

        private class SongJacket : BufferedContainer
        {
            private readonly Sprite sprite;

            public SongJacket()
            {
                RelativeSizeAxes = Axes.Both;
                CacheDrawnFrameBuffer = true;
                Children = new Drawable[]
                {
                    sprite = new Sprite
                    {
                        RelativeSizeAxes = Axes.Both,
                        Colour = TachyonColor.Gray(150),
                        FillMode = FillMode.Fill,
                    },
                };
            }

            [BackgroundDependencyLoader]
            private void load(TextureStore textures)
            {
                sprite.Texture = textures.Get(@"Etc/song_jacket_example");
            }
        }
        
        private class MusicControllerButton : IconButton
        {
            public MusicControllerButton()
            {
                AutoSizeAxes = Axes.Both;
            }

            [BackgroundDependencyLoader]
            private void load(TachyonColor color)
            {
                HoverColor = color.Gray8.Opacity(0.6f);
                FlashColor = color.Gray8;
            }

            protected override void LoadComplete()
            {
                base.LoadComplete();

                Content.AutoSizeAxes = Axes.None;
                Content.Size = new Vector2(DEFAULT_BUTTON_SIZE);
            }
        }
    }
}