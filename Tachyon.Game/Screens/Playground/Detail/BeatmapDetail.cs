using System;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Localisation;
using osuTK.Graphics;
using Tachyon.Game.Beatmaps;
using Tachyon.Game.Graphics;
using Tachyon.Game.Graphics.Sprites;

namespace Tachyon.Game.Screens.Playground.Detail
{
    public class BeatmapDetail : CompositeDrawable
    {
        private const float height = 145;
        
        private FillFlowContainer flow;
        
        private WorkingBeatmap beatmap;

        protected BeatmapMetadataDetail MetadataDetail;
        
        public WorkingBeatmap Beatmap
        {
            get => beatmap;
            set
            {
                if (beatmap == value)
                    return;

                beatmap = value;
                updateBeatmapDetail();
            }
        }
        

        [BackgroundDependencyLoader]
        private void load()
        {
            RelativeSizeAxes = Axes.X;
            Width = 0.7f;
            Anchor = Anchor.BottomCentre;
            Origin = Anchor.BottomCentre;

            InternalChildren = new Drawable[]
            {
                flow = new FillFlowContainer
                {
                    RelativeSizeAxes = Axes.X,
                    AutoSizeAxes = Axes.Y,
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre,
                    LayoutDuration = 500,
                    LayoutEasing = Easing.OutQuint,
                    Direction = FillDirection.Full,
                    CornerRadius = height / 2,
                    CornerExponent = 2,
                }
            };
        }

        private void updateBeatmapDetail()
        {
            if (beatmap == null)
            {
                flow.Clear();
                return;
            }

            flow.Children = new Drawable[]
            {
                MetadataDetail = new BeatmapMetadataDetail(beatmap)
                {
                    RelativeSizeAxes = Axes.X,
                    Width = 1f,
                    Height = height / 2,
                    Anchor = Anchor.BottomRight,
                    Origin = Anchor.BottomRight, 
                }
            };
        }

        public class BeatmapMetadataDetail : CompositeDrawable
        {
            private readonly BeatmapInfo beatmapInfo;
            
            public TachyonSpriteText TitleLabel { get; private set; }
            
            public BeatmapMetadataDetail(WorkingBeatmap beatmap)
            {
                Width = 400;
                Height = 50;

                beatmapInfo = beatmap.BeatmapInfo;
            }
            
            [BackgroundDependencyLoader]
            private void load()
            {
                Masking = true;

                AddRangeInternal(new Drawable[]
                {
                    new Box
                    {
                        RelativeSizeAxes = Axes.X,
                        Width = 2f,
                        Colour = TachyonColor.Gray(0.93f),
                    },
                    new FillFlowContainer
                    {
                        AutoSizeAxes = Axes.Both,
                        Anchor = Anchor.CentreLeft,
                        Origin = Anchor.CentreLeft,
                        Direction = FillDirection.Vertical,
                        Children = new Drawable[]
                        {
                            TitleLabel = new TachyonSpriteText
                            {
                                Text = new LocalisedString((
                                    $"{beatmapInfo.Metadata.ArtistUnicode ?? beatmapInfo.Metadata.Artist} - {beatmapInfo.Metadata.TitleUnicode ?? beatmapInfo.Metadata.Title}",
                                    $"{beatmapInfo.Metadata.Artist} - {beatmapInfo.Metadata.Title}")),
                                Font = TachyonFont.Default.With(weight: FontWeight.Bold, size: 40),
                            },
                            new FillFlowContainer
                            {
                                AutoSizeAxes = Axes.Both,
                                Direction = FillDirection.Horizontal,
                                Children = new Drawable[]
                                {
                                    new TachyonSpriteText
                                    {
                                        Text = "difficulty",
                                        Padding = new MarginPadding { Right = 5 },
                                        Font = TachyonFont.Default.With(weight: FontWeight.Regular, size: 24)
                                    },
                                    new TachyonSpriteText
                                    {
                                        Text = beatmapInfo.Version,
                                        Padding = new MarginPadding { Right = 20 },
                                        Font = TachyonFont.Default.With(weight: FontWeight.Bold, size: 24)
                                    },
                                    new TachyonSpriteText
                                    {
                                        Text = "star rating",
                                        Padding = new MarginPadding { Right = 5 },
                                        Font = TachyonFont.Default.With(weight: FontWeight.Regular, size: 24)
                                    },
                                    new TachyonSpriteText
                                    {
                                        Text = $"{beatmapInfo.StarDifficulty:0.#}",
                                        Padding = new MarginPadding { Right = 20 },
                                        Font = TachyonFont.Default.With(weight: FontWeight.Bold, size: 24)
                                    },
                                    new TachyonSpriteText
                                    {
                                        Text = "BPM",
                                        Padding = new MarginPadding { Right = 5 },
                                        Font = TachyonFont.Default.With(weight: FontWeight.Regular, size: 24)
                                    },
                                    new TachyonSpriteText
                                    {
                                        Text = $"{beatmapInfo.BeatmapSet?.MaxBPM:0.#}",
                                        Padding = new MarginPadding { Right = 20 },
                                        Font = TachyonFont.Default.With(weight: FontWeight.Bold, size: 24)
                                    },
                                    new TachyonSpriteText
                                    {
                                        Text = "length",
                                        Padding = new MarginPadding { Right = 5 },
                                        Font = TachyonFont.Default.With(weight: FontWeight.Regular, size: 24)
                                    },
                                    new TachyonSpriteText
                                    {
                                        Text = TimeSpan.FromMilliseconds(beatmapInfo.Length).ToString(@"mm\:ss"),
                                        Padding = new MarginPadding { Right = 20 },
                                        Font = TachyonFont.Default.With(weight: FontWeight.Bold, size: 24)
                                    },
                                }
                            }
                        },
                    }
                });
            }
        }
    }
}
