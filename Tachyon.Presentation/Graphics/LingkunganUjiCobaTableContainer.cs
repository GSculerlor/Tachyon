using System.Collections.Generic;
using System.Linq;
using osu.Framework.Allocation;
using osu.Framework.Extensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Input.Events;
using osuTK.Graphics;
using Tachyon.Game.Graphics;
using Tachyon.Game.Graphics.Containers;
using Tachyon.Game.Graphics.Sprites;

namespace Tachyon.Presentation.Graphics
{
    public class LingkunganUjiCobaTableContainer : TableContainer
    {
        private const float horizontal_inset = 20;
        private const float row_height = 25;
        private const int text_size = 24;

        private readonly FillFlowContainer backgroundFlow;

        private List<KeyValuePair<string, string>> sections;
        
        public LingkunganUjiCobaTableContainer()
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
        private void load()
        {
            sections = new List<KeyValuePair<string, string>>(new []
            {
                new KeyValuePair<string, string>("Perangkat Keras", "• Asus X550VX: Intel Core i7-6700HQ 54, NVIDIA GeForce GTX 950M (2GB GDDR5) 79, 1x 8GB DDR4"), 
                new KeyValuePair<string, string>("", "• Macbook Pro 2017: 3.1GHz Intel Core i5-7267U, Intel Iris Plus Graphics 650, 8GB (2,133MHz LPDDR3)"), 
                new KeyValuePair<string, string>("", "• Xiaomi Redmi Note 5A Prime"), 
                new KeyValuePair<string, string>("", "• iPhone 11"), 
                new KeyValuePair<string, string>("Perangkat Lunak (Sistem Operasi)", "• Microsoft Windows 10 64-bit"), 
                new KeyValuePair<string, string>("", "• Ubuntu 20.04 LTS amd64"), 
                new KeyValuePair<string, string>("", "• macOS Catalina 10.15.3"), 
                new KeyValuePair<string, string>("", "• Android 10"), 
                new KeyValuePair<string, string>("", "• iOS 13.4.1"), 
                new KeyValuePair<string, string>("Perangkat Lunak (Aplikasi Pengembang)", "JetBrains Rider 2019 3.2"), 
                new KeyValuePair<string, string>("Perangkat Lunak (Aplikasi Pembantu)", "Visual Studio Code, xcode, AVD Manager, Discord"), 
            });
        }
        
        public IEnumerable<KeyValuePair<string, string>> Items
        {
            set
            {
                Content = null;
                backgroundFlow.Clear();

                if (value?.Any() != true)
                    return;

                foreach (var unused in value)
                {
                    backgroundFlow.Add(new RowBackground());
                }

                Columns = createHeaders();
                Content = value.Select((g, i) => createContent(i)).ToArray().ToRectangular();
            }
        }
        
        private TableColumn[] createHeaders()
        {
            var columns = new List<TableColumn>
            {
                new TableColumn("Perangkat", Anchor.CentreLeft, new Dimension(GridSizeMode.Absolute, 500)),
                new TableColumn("Spesifikasi", Anchor.CentreLeft, new Dimension()),
            };

            return columns.ToArray();
        }
        
        private Drawable[] createContent(int index) => new Drawable[]
        {
            new TachyonSpriteText
            {
                Text = $"{sections[index].Key}",
                Font = TachyonFont.GetFont(size: text_size, weight: FontWeight.Bold)
            },
            new TachyonSpriteText
            {
                Text = $"{sections[index].Value}",
                Font = TachyonFont.GetFont(size: text_size, weight: FontWeight.Bold)
            },
        };

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
            private const int fade_duration = 100;

            private readonly Box hoveredBackground;

            public RowBackground()
            {
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
            }

            private Color4 colourHover;
            private Color4 colourSelected;

            [BackgroundDependencyLoader]
            private void load(TachyonColor colors)
            {
                hoveredBackground.Colour = colourHover = colors.BlueDarker;
                colourSelected = colors.YellowDarker;
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
