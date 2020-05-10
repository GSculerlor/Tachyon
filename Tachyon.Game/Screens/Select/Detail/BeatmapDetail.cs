using System;
using System.Collections.Generic;
using System.Linq;
using osu.Framework.Allocation;
using osu.Framework.Extensions.Color4Extensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Colour;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Cursor;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Input.Events;
using osu.Framework.Localisation;
using osu.Framework.Logging;
using osu.Framework.Utils;
using osuTK;
using osuTK.Graphics;
using Tachyon.Game.Beatmaps;
using Tachyon.Game.Beatmaps.Drawables;
using Tachyon.Game.Graphics;
using Tachyon.Game.Graphics.Sprites;
using Tachyon.Game.Rulesets.UI;

namespace Tachyon.Game.Screens.Select.Detail
{
    public class BeatmapDetail : VisibilityContainer
    {
        private const float corner_radius = 5;
        private const float background_alpha = 0.25f;
        
        protected BeatmapDetailContent DetailContent;
        private BeatmapDetailContent loadingDetailContent;
        
        private WorkingBeatmap beatmap;

        public Action ClickedAction;

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

        [BackgroundDependencyLoader]
        private void load(TachyonColor colors)
        {
            CornerRadius = corner_radius;
            Masking = true;
            BorderColour = colors.Blue;
            Alpha = 0;
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
                Depth = DetailContent?.Depth + 1 ?? 0
            }, loaded =>
            {
                if (loaded != loadingDetailContent) return;

                removeOldInfo();
                Add(DetailContent = loaded);
            });
        }

        protected override bool OnClick(ClickEvent e)
        {
            ClickedAction?.Invoke();
            
            return true;
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
                        Children = new Drawable[]
                        {
                            new BeatmapBackgroundSprite(beatmap)
                            {
                                RelativeSizeAxes = Axes.Both,
                                Anchor = Anchor.Centre,
                                Origin = Anchor.Centre,
                                FillMode = FillMode.Fill,
                            },
                            new FillFlowContainer
                            {
                                Depth = -1,
                                RelativeSizeAxes = Axes.Both,
                                Direction = FillDirection.Horizontal,
                                Shear = new Vector2(0.8f, 0),
                                Alpha = 0.5f,
                                Children = new[]
                                {
                                    new Box
                                    {
                                        RelativeSizeAxes = Axes.Both,
                                        Colour = Color4.Black,
                                        Width = 0.4f,
                                    },
                                    new Box
                                    {
                                        RelativeSizeAxes = Axes.Both,
                                        Colour = ColourInfo.GradientHorizontal(Color4.Black, new Color4(0f, 0f, 0f, 0.9f)),
                                        Width = 0.05f,
                                    },
                                    new Box
                                    {
                                        RelativeSizeAxes = Axes.Both,
                                        Colour = ColourInfo.GradientHorizontal(new Color4(0f, 0f, 0f, 0.9f), new Color4(0f, 0f, 0f, 0.1f)),
                                        Width = 0.2f,
                                    },
                                    new Box
                                    {
                                        RelativeSizeAxes = Axes.Both,
                                        Colour = ColourInfo.GradientHorizontal(new Color4(0f, 0f, 0f, 0.1f), new Color4(0, 0, 0, 0)),
                                        Width = 0.05f,
                                    },
                                }
                            },
                        },
                    },
                    new FillFlowContainer
                    {
                        Anchor = Anchor.CentreLeft,
                        Origin = Anchor.CentreLeft,
                        Direction = FillDirection.Vertical,
                        Padding = new MarginPadding { Left = 50 },
                        AutoSizeAxes = Axes.Y,
                        RelativeSizeAxes = Axes.X,
                        Children = new Drawable[]
                        {
                            TitleLabel = new TachyonSpriteText
                            {
                                Font = TachyonFont.GetFont(size: 30, weight: FontWeight.Bold),
                                RelativeSizeAxes = Axes.X,
                                Truncate = true,
                            },
                            ArtistLabel = new TachyonSpriteText
                            {
                                Font = TachyonFont.GetFont(size: 24, weight: FontWeight.SemiBold),
                                RelativeSizeAxes = Axes.X,
                                Truncate = true,
                                Margin = new MarginPadding { Bottom = 16 }
                            },
                            new FillFlowContainer
                            {
                                Margin = new MarginPadding { Top = 10 },
                                Direction = FillDirection.Horizontal,
                                AutoSizeAxes = Axes.Both,
                                Children = getMapper(metadata)
                            },
                            new FillFlowContainer
                            {
                                Margin = new MarginPadding { Top = 20 },
                                Spacing = new Vector2(20, 0),
                                AutoSizeAxes = Axes.Both,
                                Children = getInfoLabels()
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
            
            private InfoLabel[] getInfoLabels()
            {
                var b = beatmap.Beatmap;

                List<InfoLabel> labels = new List<InfoLabel>();

                if (b?.HitObjects?.Any() == true)
                {
                    labels.Add(new InfoLabel(new BeatmapStatistic
                    {
                        Name = "Length",
                        Icon = FontAwesome.Regular.Clock,
                        Content = TimeSpan.FromMilliseconds(b.BeatmapInfo.Length).ToString(@"m\:ss"),
                    }));

                    labels.Add(new InfoLabel(new BeatmapStatistic
                    {
                        Name = "BPM",
                        Icon = FontAwesome.Solid.Stopwatch,
                        Content = getBPMRange(b),
                    }));
                }

                return labels.ToArray();
            }
            
            private TachyonSpriteText[] getMapper(BeatmapMetadata metadata)
            {
                if (string.IsNullOrEmpty(metadata.Author))
                    return Array.Empty<TachyonSpriteText>();

                return new[]
                {
                    new TachyonSpriteText
                    {
                        Text = "mapped by ",
                        Font = TachyonFont.GetFont(size: 20),
                    },
                    new TachyonSpriteText
                    {
                        Text = metadata.Author,
                        Font = TachyonFont.GetFont(size: 20, weight: FontWeight.SemiBold),
                    }
                };
            }
            
            private string getBPMRange(IBeatmap beatmap)
            {
                double bpmMax = beatmap.ControlPointInfo.BPMMaximum;
                double bpmMin = beatmap.ControlPointInfo.BPMMinimum;

                if (Precision.AlmostEquals(bpmMin, bpmMax))
                    return $"{bpmMin:0}";

                return $"BPM {bpmMin:0}-{bpmMax:0} (mostly {beatmap.ControlPointInfo.BPMMode:0})";
            }
            
            public class InfoLabel : Container, IHasTooltip
            {
                public string TooltipText { get; }

                public InfoLabel(BeatmapStatistic statistic)
                {
                    TooltipText = statistic.Name;
                    AutoSizeAxes = Axes.Both;

                    Children = new Drawable[]
                    {
                        new Container
                        {
                            Anchor = Anchor.CentreLeft,
                            Origin = Anchor.CentreLeft,
                            Size = new Vector2(20),
                            Children = new[]
                            {
                                new SpriteIcon
                                {
                                    Anchor = Anchor.Centre,
                                    Origin = Anchor.Centre,
                                    RelativeSizeAxes = Axes.Both,
                                    Scale = new Vector2(0.8f),
                                    Icon = statistic.Icon,
                                },
                            }
                        },
                        new TachyonSpriteText
                        {
                            Anchor = Anchor.CentreLeft,
                            Origin = Anchor.CentreLeft,
                            Font = TachyonFont.GetFont(weight: FontWeight.Bold, size: 18),
                            Margin = new MarginPadding { Left = 30 },
                            Text = statistic.Content,
                        }
                    };
                }
            }
        }
    }
}
