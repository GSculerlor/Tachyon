using osu.Framework.Allocation;
using osu.Framework.Extensions.Color4Extensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Colour;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Effects;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Localisation;
using osu.Framework.Utils;
using osuTK.Graphics;
using Tachyon.Game.Beatmaps;
using Tachyon.Game.Beatmaps.Drawables;
using Tachyon.Game.Graphics;
using Tachyon.Game.Graphics.Sprites;

namespace Tachyon.Game.Screens.Select.Detail
{
    public class BeatmapDetail : VisibilityContainer
    {
        protected BeatmapDetailContent DetailContent;
        private BeatmapDetailContent loadingDetailContent;
        
        private WorkingBeatmap beatmap;

        public WorkingBeatmap Beatmap
        {
            get => beatmap;
            set
            {
                if (beatmap == value) return;

                beatmap = value;
                updateDetail();
            }
        }
        
        public BeatmapDetail()
        {
            Masking = true;
            BorderColour = new Color4(221, 255, 255, 255);
            Alpha = 0;
            EdgeEffect = new EdgeEffectParameters
            {
                Type = EdgeEffectType.Glow,
                Colour = new Color4(130, 204, 255, 150),
                Radius = 20,
            };
        }
        
        public override bool IsPresent => base.IsPresent || DetailContent == null;
        
        protected override void PopIn()
        {
            this.MoveToX(0, 800, Easing.OutQuint);
            this.FadeIn(250);
        }

        protected override void PopOut()
        {
            this.MoveToX(-100, 800, Easing.In);
            this.FadeOut(250, Easing.In);
        }

        private void updateDetail()
        {
            void removeOldInfo()
            {
                State.Value = beatmap == null ? Visibility.Hidden : Visibility.Visible;

                DetailContent?.FadeOut(250);
                DetailContent?.Expire();
                DetailContent = null;
            }

            if (beatmap == null)
            {
                removeOldInfo();
                return;
            }

            LoadComponentAsync(loadingDetailContent = new BeatmapDetailContent(beatmap)
            {
                Shear = -Shear,
                Depth = DetailContent?.Depth + 1 ?? 0
            }, loaded =>
            {
                if (loaded != loadingDetailContent) return;

                removeOldInfo();
                Add(DetailContent = loaded);
            });
        }

        public class BeatmapDetailContent : BufferedContainer
        {
            private readonly WorkingBeatmap beatmap;
            
            public TachyonSpriteText TitleLabel { get; private set; }
            public TachyonSpriteText ArtistLabel { get; private set; }
            
            private ILocalisedBindableString titleBinding;
            private ILocalisedBindableString artistBinding;

            public BeatmapDetailContent(WorkingBeatmap beatmap)
            {
                this.beatmap = beatmap;
            }

            [BackgroundDependencyLoader]
            private void load(LocalisationManager localisation)
            {
                var beatmapInfo = beatmap.BeatmapInfo;
                var metadata = beatmapInfo.Metadata ?? beatmap.BeatmapSetInfo?.Metadata ?? new BeatmapMetadata();
                
                CacheDrawnFrameBuffer = true;
                RedrawOnScale = false;

                RelativeSizeAxes = Axes.Both;
                
                titleBinding = localisation.GetLocalisedString(new LocalisedString((metadata.TitleUnicode, metadata.Title)));
                artistBinding = localisation.GetLocalisedString(new LocalisedString((metadata.ArtistUnicode, metadata.Artist)));


                Children = new Drawable[]
                {
                    new Box
                    {
                        RelativeSizeAxes = Axes.Both,
                        Colour = Color4.Black,
                    },
                    new Container
                    {
                        RelativeSizeAxes = Axes.Both,
                        Colour = ColourInfo.GradientVertical(Color4.White, Color4.White.Opacity(0.3f)),
                        Children = new[]
                        {
                            new BeatmapBackgroundSprite(beatmap)
                            {
                                RelativeSizeAxes = Axes.Both,
                                Anchor = Anchor.Centre,
                                Origin = Anchor.Centre,
                                FillMode = FillMode.Fill,
                            },
                        },
                    },
                    new FillFlowContainer
                    {
                        Name = "Bottom-aligned metadata",
                        Anchor = Anchor.BottomLeft,
                        Origin = Anchor.BottomLeft,
                        Y = -7,
                        Direction = FillDirection.Vertical,
                        Padding = new MarginPadding { Left = 25, Bottom = 25 },
                        AutoSizeAxes = Axes.Y,
                        RelativeSizeAxes = Axes.X,
                        Children = new Drawable[]
                        {
                            TitleLabel = new TachyonSpriteText
                            {
                                Font = TachyonFont.GetFont(size: 28, weight: FontWeight.Bold),
                                RelativeSizeAxes = Axes.X,
                                Truncate = true,
                            },
                            ArtistLabel = new TachyonSpriteText
                            {
                                Font = TachyonFont.GetFont(size: 20, weight: FontWeight.SemiBold),
                                RelativeSizeAxes = Axes.X,
                                Truncate = true,
                            },
                            new TachyonSpriteText
                            {
                                Font = TachyonFont.GetFont(size: 16, weight: FontWeight.Bold),
                                RelativeSizeAxes = Axes.X,
                                Truncate = true,
                                Text = $"BPM {getBPMRange(beatmap.Beatmap)}"
                            },
                        }
                    }
                };
                
                titleBinding.BindValueChanged(_ => setMetadata());
                artistBinding.BindValueChanged(_ => setMetadata(), true);
            }
            
            private void setMetadata()
            {
                ArtistLabel.Text = artistBinding.Value;
                TitleLabel.Text = titleBinding.Value;
                ForceRedraw();
            }
            
            private string getBPMRange(IBeatmap beatmap)
            {
                double bpmMax = beatmap.ControlPointInfo.BPMMaximum;
                double bpmMin = beatmap.ControlPointInfo.BPMMinimum;

                if (Precision.AlmostEquals(bpmMin, bpmMax))
                    return $"{bpmMin:0}";

                return $"{bpmMin:0}-{bpmMax:0} (mostly {beatmap.ControlPointInfo.BPMMode:0})";
            }
        }
    }
}
