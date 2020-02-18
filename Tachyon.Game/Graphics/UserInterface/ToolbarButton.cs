using osu.Framework.Allocation;
using osu.Framework.Extensions.Color4Extensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Input.Events;
using osuTK;
using osuTK.Graphics;
using Tachyon.Game.Graphics.Containers;
using Tachyon.Game.Graphics.Sprites;
using Tachyon.Game.Overlays;

namespace Tachyon.Game.Graphics.UserInterface
{
    public class ToolbarButton : ClickableContainer
    {
        private const float WIDTH = Toolbar.HEIGHT;

        private readonly TachyonIconContainer iconContainer;
        private readonly TachyonSpriteText text;
        private readonly Box background;

        public string Text
        {
            get => text.Text;
            set => text.Text = value;
        }

        public IconUsage Icon
        {
            set => SetIcon(value);
        }
        
        public ToolbarButton()
        {
            RelativeSizeAxes = Axes.Y;
            Width = WIDTH;
            
            Children = new Drawable[]
            {
                background = new Box
                {
                    RelativeSizeAxes = Axes.Both,
                    Alpha = 0,
                    Blending = BlendingParameters.Additive,
                },
                new FillFlowContainer
                {
                    Direction = FillDirection.Horizontal,
                    Spacing = new Vector2(10),
                    Anchor = Anchor.TopCentre,
                    Origin = Anchor.TopCentre,
                    Padding = new MarginPadding { Left = Toolbar.HEIGHT / 2, Right = Toolbar.HEIGHT / 2 },
                    RelativeSizeAxes = Axes.Y,
                    AutoSizeAxes = Axes.X,
                    Children = new Drawable[]
                    {
                        iconContainer = new TachyonIconContainer
                        {
                            Anchor = Anchor.CentreLeft,
                            Origin = Anchor.CentreLeft,
                            Size = new Vector2(15),
                        },
                        text = new TachyonSpriteText
                        {
                            Anchor = Anchor.CentreLeft,
                            Origin = Anchor.CentreLeft,
                            Font = TachyonFont.GetFont(size: 20),
                            Alpha = 0
                        },
                    }
                },
            };
        }

        [BackgroundDependencyLoader]
        private void load(TachyonColor color)
        {
            background.Colour = color.BackButtonGray.Opacity(180);
        }
        
        public void SetIcon(IconUsage icon)
        {
            var spriteIcon = new SpriteIcon
            {
                Size = new Vector2(15),
                Icon = icon
            };

            iconContainer.Icon = spriteIcon;
        }

        protected override bool OnMouseDown(MouseDownEvent e) => true;

        protected override bool OnClick(ClickEvent e)
        {
            background.FlashColour(Color4.White.Opacity(100), 450, Easing.OutQuint);
            
            return base.OnClick(e);
        }

        protected override bool OnHover(HoverEvent e)
        {
            background.FadeIn(200);
            text.FadeIn(200);
            
            return base.OnHover(e);
        }

        protected override void OnHoverLost(HoverLostEvent e)
        {
            background.FadeOut(200);
            text.FadeOut(200);
            
            base.OnHoverLost(e);
        }
    }
}