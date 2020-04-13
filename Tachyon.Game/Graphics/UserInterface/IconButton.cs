using osu.Framework.Graphics;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Input.Events;
using osuTK;
using osuTK.Graphics;

namespace Tachyon.Game.Graphics.UserInterface
{
    public class IconButton : AnimatedButton
    {
        public const float DEFAULT_BUTTON_SIZE = 24;
        
        private Color4? iconColor;

        public Color4 IconColor
        {
            get => iconColor ?? Color4.White;
            set
            {
                iconColor = value;
                icon.Colour = value;
            }
        }

        private Color4? iconHoverColor;

        public Color4 IconHoverColor
        {
            get => iconHoverColor ?? IconColor;
            set => iconHoverColor = value;
        }

        public IconUsage Icon
        {
            get => icon.Icon;
            set => icon.Icon = value;
        }

        public Vector2 IconScale
        {
            get => icon.Scale;
            set => icon.Scale = value;
        }

        private readonly SpriteIcon icon;

        public IconButton()
        {
            Size = new Vector2(DEFAULT_BUTTON_SIZE);

            Add(icon = new SpriteIcon
            {
                Origin = Anchor.Centre,
                Anchor = Anchor.Centre,
                Size = new Vector2(18),
            });
        }

        protected override bool OnHover(HoverEvent e)
        {
            icon.FadeColour(IconHoverColor, 500, Easing.OutQuint);
            return base.OnHover(e);
        }

        protected override void OnHoverLost(HoverLostEvent e)
        {
            icon.FadeColour(IconColor, 500, Easing.OutQuint);
            base.OnHoverLost(e);
        }
    }
}