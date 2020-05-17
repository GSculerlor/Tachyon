using System.Collections.Generic;
using System.Linq;
using osu.Framework.Allocation;
using osu.Framework.Audio.Track;
using osu.Framework.Bindables;
using osu.Framework.Extensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Input.Events;
using osuTK;
using osuTK.Graphics;
using Tachyon.Game.Audio;
using Tachyon.Game.Beatmaps;
using Tachyon.Game.Graphics;
using Tachyon.Game.Graphics.Containers;
using Tachyon.Game.Graphics.Sprites;
using Tachyon.Game.Rulesets.Objects;

namespace Tachyon.Game.Screens.Generate.Components
{
    public class HitObjectTable : TableContainer
    {
        private const float horizontal_inset = 20;
        private const float row_height = 25;
        private const int text_size = 20;

        private readonly FillFlowContainer backgroundFlow;

        private List<Waveform.Point> points;
        
        public HitObjectTable()
        {
            RelativeSizeAxes = Axes.X;
            AutoSizeAxes = Axes.Y;

            Padding = new MarginPadding { Horizontal = horizontal_inset };
            RowSize = new Dimension(GridSizeMode.Absolute, row_height);

            AddInternal(backgroundFlow = new FillFlowContainer
            {
                RelativeSizeAxes = Axes.Both,
                Depth = 1f,
                Padding = new MarginPadding { Horizontal = -horizontal_inset },
                Margin = new MarginPadding { Top = row_height }
            });
        }

        [BackgroundDependencyLoader]
        private void load(Bindable<WorkingBeatmap> working)
        {
            points = working.Value.Waveform.GetPoints();
        }
        
        public IEnumerable<HitObject> HitObjects
        {
            set
            {
                Content = null;
                backgroundFlow.Clear();

                if (value?.Any() != true)
                    return;

                foreach (var hitobj in value)
                {
                    backgroundFlow.Add(new RowBackground(hitobj));
                }

                Columns = createHeaders();
                Content = value.Select((g, i) => createContent(i, g)).ToArray().ToRectangular();
            }
        }
        
        private TableColumn[] createHeaders()
        {
            var columns = new List<TableColumn>
            {
                new TableColumn(string.Empty, Anchor.Centre, new Dimension(GridSizeMode.AutoSize)),
                new TableColumn("Time", Anchor.Centre, new Dimension(GridSizeMode.AutoSize)),
                new TableColumn("HitObject", Anchor.Centre, new Dimension()),
                new TableColumn("Left Channel", Anchor.Centre, new Dimension()),
                new TableColumn("Right Channel", Anchor.Centre, new Dimension()),
                new TableColumn("Low Intensity", Anchor.Centre, new Dimension()),
                new TableColumn("Mid Intensity", Anchor.Centre, new Dimension()),
                new TableColumn("High Intensity", Anchor.Centre, new Dimension()),
            };

            return columns.ToArray();
        }
        
        private Drawable[] createContent(int index, HitObject hitObject) => new Drawable[]
        {
            new TachyonSpriteText
            {
                Text = $"#{index + 1}",
                Font = TachyonFont.GetFont(size: text_size, weight: FontWeight.Bold),
                Margin = new MarginPadding(10)
            },
            new TachyonSpriteText
            {
                Text = $"{hitObject.StartTime:0.00}ms",
                Font = TachyonFont.GetFont(size: text_size, weight: FontWeight.Bold)
            },
            new HitObjectAttributes(hitObject)
            {
                Origin = Anchor.CentreLeft,
                Anchor = Anchor.CentreLeft
            },
            new TachyonSpriteText
            {
                Text = $"{points[(int) hitObject.StartTime].Amplitude[0]:0.000}",
                Font = TachyonFont.GetFont(size: text_size, weight: FontWeight.Bold)
            },
            new TachyonSpriteText
            {
                Text = $"{points[(int) hitObject.StartTime].Amplitude[1]:0.000}",
                Font = TachyonFont.GetFont(size: text_size, weight: FontWeight.Bold)
            },
            new TachyonSpriteText
            {
                Text = $"{points[(int) hitObject.StartTime].LowIntensity:0.000}",
                Font = TachyonFont.GetFont(size: text_size, weight: FontWeight.Bold)
            },
            new TachyonSpriteText
            {
                Text = $"{points[(int) hitObject.StartTime].MidIntensity:0.000}",
                Font = TachyonFont.GetFont(size: text_size, weight: FontWeight.Bold)
            },
            new TachyonSpriteText
            {
                Text = $"{points[(int) hitObject.StartTime].HighIntensity:0.000}",
                Font = TachyonFont.GetFont(size: text_size, weight: FontWeight.Bold)
            }
        };
        
        private class HitObjectAttributes : CompositeDrawable
        {
            private readonly IBindableList<HitSampleInfo> hitSampleInfos;

            private readonly FillFlowContainer fill;

            public HitObjectAttributes(HitObject hitObject)
            {
                InternalChild = fill = new FillFlowContainer
                {
                    Origin = Anchor.CentreLeft,
                    Anchor = Anchor.CentreLeft,
                    RelativeSizeAxes = Axes.Both,
                    Direction = FillDirection.Horizontal,
                    Padding = new MarginPadding(10),
                    Spacing = new Vector2(2)
                };

                hitSampleInfos = hitObject.SamplesBindable.GetBoundCopy();
                hitSampleInfos.ItemsAdded += _ => createChildren();
                hitSampleInfos.ItemsRemoved += _ => createChildren();

                createChildren();
            }

            private void createChildren()
            {
                fill.ChildrenEnumerable = hitSampleInfos.Select(createAttribute).Where(c => c != null);
            }

            private Drawable createAttribute(HitSampleInfo sampleInfo)
            {
                
                switch (sampleInfo.Name)
                {
                    case HitSampleInfo.HIT_WHISTLE:
                        return new RowAttribute("Upper Note", () => "Whistle");
                    
                    case HitSampleInfo.HIT_CLAP:
                        return new RowAttribute("Upper Note", () => "Clap");

                    case HitSampleInfo.HIT_FINISH:
                        return new RowAttribute("Lower Note", () => "Finish");
                        
                    case HitSampleInfo.HIT_NORMAL:
                        return new RowAttribute("Lower Note", () => "Normal");
                }

                return null;
            }
        }
        
        protected override Drawable CreateHeader(int index, TableColumn column) => new HeaderText(column?.Header ?? string.Empty);

        private class HeaderText : TachyonSpriteText
        {
            public HeaderText(string text)
            {
                Text = text.ToUpper();
                Font = TachyonFont.GetFont(size: 20, weight: FontWeight.Bold);
            }
        }

        public class RowBackground : TachyonClickableContainer
        {
            private readonly HitObject hitObject;
            private const int fade_duration = 100;

            private readonly Box hoveredBackground;

            [Resolved]
            private Bindable<HitObject> selectedHitObject { get; set; }

            public RowBackground(HitObject hitObject)
            {
                this.hitObject = hitObject;
                RelativeSizeAxes = Axes.X;
                Height = 25;

                AlwaysPresent = true;

                CornerRadius = 3;
                Masking = true;

                Children = new Drawable[]
                {
                    hoveredBackground = new Box
                    {
                        RelativeSizeAxes = Axes.Both,
                        Alpha = 0,
                    },
                };

                Action = () => selectedHitObject.Value = hitObject;
            }

            private Color4 colourHover;
            private Color4 colourSelected;

            [BackgroundDependencyLoader]
            private void load(TachyonColor colors)
            {
                hoveredBackground.Colour = colourHover = colors.BlueDarker;
                colourSelected = colors.YellowDarker;
            }

            protected override void LoadComplete()
            {
                base.LoadComplete();

                selectedHitObject.BindValueChanged(group => { Selected = hitObject == group.NewValue; }, true);
            }

            private bool selected;

            protected bool Selected
            {
                get => selected;
                set
                {
                    if (value == selected)
                        return;

                    selected = value;
                    updateState();
                }
            }

            protected override bool OnHover(HoverEvent e)
            {
                updateState();
                return base.OnHover(e);
            }

            protected override void OnHoverLost(HoverLostEvent e)
            {
                updateState();
                base.OnHoverLost(e);
            }

            private void updateState()
            {
                hoveredBackground.FadeColour(selected ? colourSelected : colourHover, 450, Easing.OutQuint);

                if (selected || IsHovered)
                    hoveredBackground.FadeIn(fade_duration, Easing.OutQuint);
                else
                    hoveredBackground.FadeOut(fade_duration, Easing.OutQuint);
            }
        }
    }
}
