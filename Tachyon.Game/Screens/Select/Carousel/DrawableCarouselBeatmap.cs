using System.Collections.Generic;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Cursor;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Graphics.UserInterface;
using osuTK;
using osuTK.Graphics;
using Tachyon.Game.Beatmaps;
using Tachyon.Game.Generator;
using Tachyon.Game.Graphics;
using Tachyon.Game.Graphics.Sprites;
using Tachyon.Game.Graphics.UserInterface;

namespace Tachyon.Game.Screens.Select.Carousel
{
    public class DrawableCarouselBeatmap : DrawableCarouselItem, IHasContextMenu
    {
        private BeatmapGenerator beatmapGenerator;
        
        private readonly BeatmapInfo beatmap;

        private Sprite background;

        public DrawableCarouselBeatmap(CarouselBeatmap panel)
            : base(panel)
        {
            beatmap = panel.Beatmap;
            Height *= 0.5f;
        }

        [BackgroundDependencyLoader(true)]
        private void load(BeatmapManager manager)
        {
            Children = new Drawable[]
            {
                background = new Box
                {
                    RelativeSizeAxes = Axes.Both,
                },
                new FillFlowContainer
                {
                    Padding = new MarginPadding(5),
                    Direction = FillDirection.Horizontal,
                    AutoSizeAxes = Axes.Both,
                    Anchor = Anchor.CentreLeft,
                    Origin = Anchor.CentreLeft,
                    Children = new Drawable[]
                    {
                        new FillFlowContainer
                        {
                            Padding = new MarginPadding { Left = 10 },
                            Direction = FillDirection.Vertical,
                            AutoSizeAxes = Axes.Both,
                            Children = new Drawable[]
                            {
                                new FillFlowContainer
                                {
                                    Direction = FillDirection.Horizontal,
                                    Spacing = new Vector2(4, 0),
                                    AutoSizeAxes = Axes.Both,
                                    Children = new[]
                                    {
                                        new TachyonSpriteText
                                        {
                                            Text = beatmap.Version,
                                            Font = TachyonFont.GetFont(size: 20, weight: FontWeight.SemiBold),
                                            Anchor = Anchor.BottomLeft,
                                            Origin = Anchor.BottomLeft
                                        },
                                    }
                                }
                            }
                        }
                    }
                }
            };
        }

        protected override void Selected()
        {
            base.Selected();

            background.Colour = new Color4(181, 84, 0, 255);
        }

        protected override void Deselected()
        {
            base.Deselected();

            background.Colour = new Color4(34, 40, 49, 255);
        }
        
        public MenuItem[] ContextMenuItems
        {
            get
            {
                List<MenuItem> items = new List<MenuItem>();

                if (Item.State.Value == CarouselItemState.Selected)
                    items.Add(new TachyonMenuItem("Generate Beatmap", MenuItemType.Highlighted, handleGenerateBeatmap));
                
                return items.ToArray();
            }
        }

        private void handleGenerateBeatmap()
        {
            AddInternal(beatmapGenerator = new BeatmapGenerator());
            beatmapGenerator.Generate();
        }
    }
}
