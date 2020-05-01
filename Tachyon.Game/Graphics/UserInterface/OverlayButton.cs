using osu.Framework.Bindables;
using osu.Framework.Extensions.Color4Extensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Effects;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Input.Events;
using osuTK;
using osuTK.Graphics;
using Tachyon.Game.Graphics.Sprites;

namespace Tachyon.Game.Graphics.UserInterface
{
    public class OverlayButton : ClickableContainer
    {
        private const float idle_width = 0.8f;

        private const float hover_duration = 500;

        public readonly BindableBool Selected = new BindableBool();

        private readonly Container backgroundContainer;
        private readonly SpriteText spriteText;
        private readonly SpriteIcon spriteIcon;
        private Vector2 hoverSpacing => new Vector2(3f, 0f);
        
        public OverlayButton()
        {
            RelativeSizeAxes = Axes.X;

            Children = new Drawable[]
            {
                backgroundContainer = new Container
                {
                    RelativeSizeAxes = Axes.Both,
                    Width = 1f
                },
                new Container
                {
                    RelativeSizeAxes = Axes.Both,
                    Origin = Anchor.Centre,
                    Anchor = Anchor.Centre,
                    Masking = true,
                    Children = new Drawable[]
                    {
                        new Container
                        {
                            RelativeSizeAxes = Axes.Both,
                            Origin = Anchor.Centre,
                            Anchor = Anchor.Centre,
                            Width = idle_width,
                            Masking = true,
                            MaskingSmoothness = 2,
                            EdgeEffect = new EdgeEffectParameters
                            {
                                Type = EdgeEffectType.Shadow,
                                Colour = Color4.Black.Opacity(0.2f),
                                Radius = 5,
                            },
                        },
                    },
                },
                new GridContainer
                {
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre,
                    RelativeSizeAxes = Axes.Both,
                    ColumnDimensions = new []
                    {
                        new Dimension(GridSizeMode.Relative, 0.1f, maxSize: 200),
                        new Dimension(GridSizeMode.Relative, 0.3f, maxSize: 850),
                        new Dimension()
                    },
                    Content = new []
                    {
                        new Drawable[]
                        {
                            new Container { RelativeSizeAxes = Axes.Both }, 
                            spriteIcon = new SpriteIcon
                            {
                                Anchor = Anchor.Centre,
                                Origin = Anchor.Centre,
                                Size = new Vector2(26),
                                Colour = IsDestructive ? Color4.Red : Color4.White,
                                Alpha = 0.8f,
                            },
                            spriteText = new TachyonSpriteText
                            {
                                Padding = new MarginPadding { Horizontal = 26 },
                                Text = Text,
                                Anchor = Anchor.CentreLeft,
                                Origin = Anchor.CentreLeft,
                                Font = TachyonFont.GetFont(size: 32, weight: FontWeight.Bold),
                                Shadow = true,
                                ShadowColour = new Color4(0, 0, 0, 0.1f),
                                Colour = IsDestructive ? Color4.Red : Color4.White,
                            },
                        }
                    }
                },
            };

            Selected.ValueChanged += selectionChanged;
        }

        public bool IsDestructive { get; set; }

        private string text;

        public string Text
        {
            get => text;
            set
            {
                text = value;
                spriteText.Text = Text;
            }
        }
        
        private IconUsage icon;

        public IconUsage Icon
        {
            get => icon;
            set
            {
                icon = value;
                spriteIcon.Icon = Icon;
            }
        }

        public override bool ReceivePositionalInputAt(Vector2 screenSpacePos) => backgroundContainer.ReceivePositionalInputAt(screenSpacePos);

        protected override bool OnClick(ClickEvent e)
        {
            Selected.TriggerChange();

            return base.OnClick(e);
        }

        protected override bool OnMouseDown(MouseDownEvent e)
        {
            Selected.Value = true;
            return base.OnMouseDown(e);
        }

        protected override bool OnMouseMove(MouseMoveEvent e)
        {
            Selected.Value = true;
            return base.OnMouseMove(e);
        }

        protected override bool OnHover(HoverEvent e)
        {
            base.OnHover(e);
            Selected.Value = true;

            return true;
        }

        protected override void OnHoverLost(HoverLostEvent e)
        {
            base.OnHoverLost(e);
            Selected.Value = false;
        }

        private void selectionChanged(ValueChangedEvent<bool> args)
        {
            spriteText.TransformSpacingTo(args.NewValue ? hoverSpacing : Vector2.Zero, hover_duration, Easing.OutElastic);
            spriteIcon.ResizeTo(args.NewValue ? new Vector2(32) : new Vector2(26), hover_duration, Easing.OutElastic);
        }
    }
}
