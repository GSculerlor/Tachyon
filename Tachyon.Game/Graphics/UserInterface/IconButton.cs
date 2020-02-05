using osu.Framework.Extensions.Color4Extensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Effects;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Input.Events;
using osuTK;
using osuTK.Graphics;

namespace Tachyon.Game.Graphics.UserInterface
{
    public class IconButton : ClickableContainer
    {
        public Color4 NormalColor
        {
            private get => normalColor;
            set
            {
                normalColor = value;
                box.Colour = value;
            }
        }

        public Color4 HoverColor
        {
            set => sideBox.Colour = value;
        }

        public IconUsage Icon
        {
            set => icon.Icon = value;
        }

        private readonly SpriteIcon icon;
        private readonly Box box;
        private readonly Box sideBox;
        private Color4 normalColor = Color4.Black;
        private const int transform_time = 600;

        public IconButton()
        {
            RelativeSizeAxes = Axes.Both;
            Child = new Container
            {
                Origin = Anchor.CentreLeft,
                Anchor = Anchor.CentreLeft,
                RelativeSizeAxes = Axes.Both,
                Masking = true,
                MaskingSmoothness = 2,
                EdgeEffect = new EdgeEffectParameters
                {
                    Type = EdgeEffectType.Shadow,
                    Radius = 10,
                    Colour = Color4.Black.Opacity(0.1f),
                    Offset = new Vector2(0f, 2f),
                },
                Children = new Drawable[]
                {
                    box = new Box
                    {
                        RelativeSizeAxes = Axes.Both
                    },
                    icon = new SpriteIcon
                    {
                        Margin = new MarginPadding { Left = 10 },
                        Origin = Anchor.CentreLeft,
                        Anchor = Anchor.CentreLeft,
                        Size = new Vector2(15),
                    },
                    sideBox = new Box
                    {
                        RelativeSizeAxes = Axes.Both,
                        Width = 0.05f,
                        Origin = Anchor.CentreRight,
                        Anchor = Anchor.CentreRight
                    },
                }
            };
        }
        
        protected override bool OnHover(HoverEvent e)
        {
            box.FadeColour(sideBox.Colour, transform_time / 2f, Easing.OutQuint);

            return true;
        }

        protected override void OnHoverLost(HoverLostEvent e)
        {
            box.FadeColour(NormalColor, transform_time, Easing.Out);
        }
    }
}