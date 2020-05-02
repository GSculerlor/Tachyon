using System;
using System.Threading.Tasks;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Extensions.Color4Extensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Effects;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Graphics.Textures;
using osu.Framework.Input.Events;
using osu.Framework.Localisation;
using osuTK;
using osuTK.Graphics;
using Tachyon.Game.Beatmaps;
using Tachyon.Game.Components;
using Tachyon.Game.Graphics;
using Tachyon.Game.Graphics.Containers;
using Tachyon.Game.Graphics.Sprites;
using Tachyon.Game.Graphics.UserInterface;

namespace Tachyon.Game.Overlays.Music
{
    public class MusicPlayerOverlay : TachyonFocusedOverlayContainer
    {
        private const float player_height = 80;
        private const float button_height = 50;

        private BufferedBackground background;
        private Drawable playButton;
        private SpriteText title, artist;
        private Container musicPlayer;
        private PlaylistOverlay playlist;
        private Action pendingBeatmapSwitch;
        
        [Resolved]
        private MusicController musicController { get; set; }

        [Resolved]
        private Bindable<WorkingBeatmap> beatmap { get; set; }

        [Resolved]
        private TachyonColor colors { get; set; }

        public MusicPlayerOverlay()
        {
            AutoSizeAxes = Axes.Y;
            Width = 400;
            Margin = new MarginPadding(10);
        }

        [BackgroundDependencyLoader]
        private void load()
        {
            Children = new Drawable[]
            {
                playlist = new PlaylistOverlay
                {
                    RelativeSizeAxes = Axes.X,
                    Depth = 5,
                    CornerRadius = 5,
                    Origin = Anchor.TopCentre,
                    Anchor = Anchor.TopCentre,
                    Margin = new MarginPadding { Top = player_height - 4 },
                },
                new Container
                {
                    RelativeSizeAxes = Axes.X,
                    Origin = Anchor.TopCentre,
                    Anchor = Anchor.TopCentre,
                    Height = player_height + (button_height / 2),
                    Children = new[]
                    {
                        playButton = new PlayButtonContainer
                        {
                            Origin = Anchor.BottomCentre,
                            Anchor = Anchor.BottomCentre,
                            Action = () => musicController.TogglePause(),
                            Icon = FontAwesome.Solid.Play
                        },
                        musicPlayer = new Container
                        {
                            Depth = 5,
                            RelativeSizeAxes = Axes.X,
                            Height = player_height,
                            Origin = Anchor.TopCentre,
                            Anchor = Anchor.TopCentre,
                            Masking = true,
                            CornerRadius = 5,
                            EdgeEffect = new EdgeEffectParameters
                            {
                                Type = EdgeEffectType.Shadow,
                                Colour = Color4.Black.Opacity(40),
                                Radius = 5,
                            },
                            Children = new Drawable[]
                            {
                                background = new BufferedBackground(),
                                title = new TachyonSpriteText
                                {
                                    Origin = Anchor.TopCentre,
                                    Anchor = Anchor.TopCentre,
                                    Margin = new MarginPadding { Top = 10 },
                                    Font = TachyonFont.GetFont(size: 25),
                                    Colour = Color4.White,
                                    Text = @"Nothing to play",
                                },
                                artist = new TachyonSpriteText
                                {
                                    Origin = Anchor.TopCentre,
                                    Anchor = Anchor.TopCentre,
                                    Margin = new MarginPadding { Top = 34 },
                                    Font = TachyonFont.GetFont(size: 16, weight: FontWeight.Bold),
                                    Colour = Color4.White,
                                    Text = @"Nothing to play",
                                },
                                new MusicControllerButton
                                {
                                    Anchor = Anchor.CentreLeft,
                                    Origin = Anchor.CentreLeft,
                                    Margin = new MarginPadding { Left = 10 },
                                    Icon = FontAwesome.Solid.ChevronLeft,
                                    Action = () => musicController.PreviousTrack()
                                },
                                new MusicControllerButton
                                {
                                    Anchor = Anchor.CentreRight,
                                    Origin = Anchor.CentreRight,
                                    Margin = new MarginPadding { Right = 10 },
                                    Icon = FontAwesome.Solid.ChevronRight,
                                    Action = () => musicController.NextTrack()
                                },
                            }
                        }
                    }
                },
            };
        }

        protected override void LoadComplete()
        {
            base.LoadComplete();
            
            playlist.BeatmapSets.BindTo(musicController.BeatmapSets);

            musicController.TrackChanged += trackChanged;
            trackChanged(beatmap.Value);
        }

        protected override void Update()
        {
            base.Update();

            if (pendingBeatmapSwitch != null)
            {
                pendingBeatmapSwitch();
                pendingBeatmapSwitch = null;
            }
            
            var track = beatmap.Value?.TrackLoaded ?? false ? beatmap.Value.Track : null;
            
            if (track?.IsDummyDevice == false)
            {
                ((PlayButtonContainer)playButton).Icon = track.IsRunning ? FontAwesome.Solid.Pause : FontAwesome.Solid.Play;
            }
            else
            {
                ((PlayButtonContainer)playButton).Icon = FontAwesome.Solid.Play;
            }
        }
        
        protected override void PopIn()
        {
            base.PopIn();

            this.FadeIn(400, Easing.OutQuint);
            this.ScaleTo(1, 400, Easing.OutQuint);
        }

        protected override void PopOut()
        {
            base.PopOut();

            this.FadeOut(400, Easing.OutQuint);
            this.ScaleTo(0.9f, 400, Easing.OutQuint);
        }
        
        private void trackChanged(WorkingBeatmap beatmap, TrackChangeDirection direction = TrackChangeDirection.None)
        {
            pendingBeatmapSwitch = delegate
            {
                Task.Run(() =>
                {
                    if (beatmap?.Beatmap == null)
                    {
                        title.Text = @"Nothing to play";
                        artist.Text = @"Nothing to play";
                    }
                    else
                    {
                        BeatmapMetadata metadata = beatmap.Metadata;
                        title.Text = new LocalisedString((metadata.TitleUnicode, metadata.Title));
                        artist.Text = new LocalisedString((metadata.ArtistUnicode, metadata.Artist));
                    }
                });

                LoadComponentAsync(new BufferedBackground(beatmap) { Depth = float.MaxValue }, newBackground =>
                {
                    newBackground.Alpha = 0f;
                    
                    background.FadeOut(700, Easing.OutQuint);
                    newBackground.FadeIn(700, Easing.OutQuint);

                    background.Expire();
                    background = newBackground;

                    musicPlayer.Add(newBackground);
                });
            };
        }

        
        private class PlayButtonContainer : CircularContainer
        {
            private readonly Box background;
            private readonly SpriteIcon icon;

            public Action Action { get; set; }
            
            public IconUsage Icon
            {
                set => icon.Icon = value;
            }

            public PlayButtonContainer()
            {
                Size = new Vector2(button_height);
                Masking = true;
                
                Children = new Drawable[]
                {
                    background = new Box
                    {
                      RelativeSizeAxes = Axes.Both,
                      Colour = TachyonColor.FromHex(@"424242")
                    },
                    icon = new SpriteIcon
                    {
                        Margin = new MarginPadding { Left = 1 },
                        Origin = Anchor.Centre,
                        Anchor = Anchor.Centre,
                        Size = new Vector2(16),
                    }
                };
            }
            
            protected override void LoadComplete()
            {
                base.LoadComplete();

                Content.AutoSizeAxes = Axes.None;
                Content.Size = new Vector2(button_height);
            }

            protected override bool OnHover(HoverEvent e)
            {
                background.Colour = TachyonColor.FromHex(@"424242").Darken(0.5f);
                    
                return base.OnHover(e);
            }

            protected override void OnHoverLost(HoverLostEvent e)
            {
                background.Colour = TachyonColor.FromHex(@"424242");
                
                base.OnHoverLost(e);
            }

            protected override bool OnClick(ClickEvent e)
            {
                Action?.Invoke();
                
                return true;
            }
        }
        
        private class MusicControllerButton : IconButton
        {
            public MusicControllerButton()
            {
                AutoSizeAxes = Axes.Both;
                IconColor = Color4.White.Opacity(0.5f);
            }

            protected override void LoadComplete()
            {
                base.LoadComplete();

                Content.AutoSizeAxes = Axes.None;
                Content.Size = new Vector2(DEFAULT_BUTTON_SIZE);
            }
        }

        private class BufferedBackground : BufferedContainer
        {
            private readonly Sprite sprite;
            private readonly WorkingBeatmap beatmap;

            public BufferedBackground(WorkingBeatmap beatmap = null)
            {
                this.beatmap = beatmap;

                Depth = float.MaxValue;
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
                    new Box
                    {
                        RelativeSizeAxes = Axes.Both,
                        Colour = Color4.Black.Opacity(0.5f)
                    }
                };
            }

            [BackgroundDependencyLoader]
            private void load(TextureStore textures)
            {
                sprite.Texture = beatmap?.Background ?? textures.Get(@"Characters/Exusiai_1");
            }
        }
    }
}