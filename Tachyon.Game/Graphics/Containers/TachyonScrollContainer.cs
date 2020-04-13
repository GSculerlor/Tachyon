using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Input.Events;
using osuTK;
using osuTK.Graphics;
using osuTK.Input;

namespace Tachyon.Game.Graphics.Containers
{
    public class TachyonScrollContainer : ScrollContainer<Drawable>
    {
        public const float SCROLL_BAR_HEIGHT = 10;
        public const float SCROLL_BAR_PADDING = 3;
        
        public TachyonScrollContainer(Direction scrollDirection = Direction.Vertical)
            : base(scrollDirection)
        {
        }
        
        protected override ScrollbarContainer CreateScrollbar(Direction direction) => new TachyonScrollbar(direction);
        
        protected class TachyonScrollbar : ScrollbarContainer
        {
            private Color4 hoverColour;
            private Color4 defaultColour;
            private Color4 highlightColour;

            private readonly Box box;

            public TachyonScrollbar(Direction scrollDir)
                : base(scrollDir)
            {
                Blending = BlendingParameters.Additive;

                CornerRadius = 5;

                const float margin = 3;

                Margin = new MarginPadding
                {
                    Left = scrollDir == Direction.Vertical ? margin : 0,
                    Right = scrollDir == Direction.Vertical ? margin : 0,
                    Top = scrollDir == Direction.Horizontal ? margin : 0,
                    Bottom = scrollDir == Direction.Horizontal ? margin : 0,
                };

                Masking = true;
                Child = box = new Box { RelativeSizeAxes = Axes.Both };
            }

            [BackgroundDependencyLoader]
            private void load(TachyonColor colors)
            {
                Colour = defaultColour = colors.Gray8;
                hoverColour = colors.GrayF;
                highlightColour = colors.Yellow;
            }

            public override void ResizeTo(float val, int duration = 0, Easing easing = Easing.None)
            {
                Vector2 size = new Vector2(SCROLL_BAR_HEIGHT)
                {
                    [(int)ScrollDirection] = val
                };
                this.ResizeTo(size, duration, easing);
            }

            protected override bool OnHover(HoverEvent e)
            {
                this.FadeColour(hoverColour, 100);
                return true;
            }

            protected override void OnHoverLost(HoverLostEvent e)
            {
                this.FadeColour(defaultColour, 100);
            }

            protected override bool OnMouseDown(MouseDownEvent e)
            {
                if (!base.OnMouseDown(e)) return false;

                box.FadeColour(highlightColour, 100);
                return true;
            }

            protected override void OnMouseUp(MouseUpEvent e)
            {
                if (e.Button != MouseButton.Left) return;

                box.FadeColour(Color4.White, 100);

                base.OnMouseUp(e);
            }
        }
    }
}