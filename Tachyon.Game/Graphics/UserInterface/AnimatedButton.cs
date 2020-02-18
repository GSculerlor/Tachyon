using osu.Framework.Allocation;
using osu.Framework.Extensions.Color4Extensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Effects;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Input.Events;
using osuTK.Graphics;
using Tachyon.Game.Graphics.Containers;

namespace Tachyon.Game.Graphics.UserInterface
{
    public class AnimatedButton : TachyonClickableContainer
    {
        protected Color4 FlashColor = Color4.White.Opacity(0.3f);

        private Color4 hoverColor = Color4.White.Opacity(0.1f);
        
        protected Color4 HoverColor
        {
            get => hoverColor;
            set
            {
                hoverColor = value;
                hover.Colour = value;
            }
        }
        
        protected override Container<Drawable> Content => content;

        private readonly Container content;
        private readonly Box hover;

        public AnimatedButton()
        {
            base.Content.Add(content = new Container
            {
                Origin = Anchor.Centre,
                Anchor = Anchor.Centre,
                RelativeSizeAxes = Axes.Both,
                CornerRadius = 5,
                Masking = true,
                EdgeEffect = new EdgeEffectParameters
                {
                    Colour = Color4.Black.Opacity(0.04f),
                    Type = EdgeEffectType.Shadow,
                    Radius = 5,
                },
                Children = new Drawable[]
                {
                    hover = new Box
                    {
                        RelativeSizeAxes = Axes.Both,
                        Colour = HoverColor,
                        Blending = BlendingParameters.Additive,
                        Alpha = 0,
                    },
                }
            });
        }
        
        [BackgroundDependencyLoader]
        private void load(TachyonColor colors)
        {
            if (AutoSizeAxes != Axes.None)
            {
                content.RelativeSizeAxes = (Axes.Both & ~AutoSizeAxes);
                content.AutoSizeAxes = AutoSizeAxes;
            }

            Enabled.BindValueChanged(enabled => this.FadeColour(enabled.NewValue ? Color4.White : colors.Gray9, 200, Easing.OutQuint), true);
        }
        
        protected override bool OnHover(HoverEvent e)
        {
            hover.FadeIn(500, Easing.OutQuint);
            return base.OnHover(e);
        }

        protected override void OnHoverLost(HoverLostEvent e)
        {
            hover.FadeOut(500, Easing.OutQuint);
            base.OnHoverLost(e);
        }

        protected override bool OnClick(ClickEvent e)
        {
            hover.FlashColour(FlashColor, 800, Easing.OutQuint);
            return base.OnClick(e);
        }
    }
}