using osu.Framework.Bindables;
using osu.Framework.Extensions.Color4Extensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Colour;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Effects;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Input.Events;
using osuTK;
using osuTK.Graphics;
using Tachyon.Game.Graphics.Sprites;

namespace Tachyon.Game.Graphics.UserInterface
{
    public class OverlayButton : ClickableContainer
    {
        private const float idle_width = 0.5f;
        private const float hover_width = 0.7f;

        private const float hover_duration = 500;
        private const float click_duration = 200;

        public readonly BindableBool Selected = new BindableBool();

        private readonly Container colorContainer;
        private readonly Container glowContainer;
        private readonly Box leftGlow;
        private readonly Box centerGlow;
        private readonly Box rightGlow;
        private readonly SpriteText spriteText;
        private Vector2 hoverSpacing => new Vector2(3f, 0f);

        public OverlayButton()
        {
            RelativeSizeAxes = Axes.X;

            Children = new Drawable[]
            {
                glowContainer = new Container
                {
                    RelativeSizeAxes = Axes.Both,
                    Width = 1f,
                    Alpha = 0f,
                    Children = new Drawable[]
                    {
                        leftGlow = new Box
                        {
                            RelativeSizeAxes = Axes.Both,
                            Origin = Anchor.TopLeft,
                            Anchor = Anchor.TopLeft,
                            Width = 0.125f,
                        },
                        centerGlow = new Box
                        {
                            RelativeSizeAxes = Axes.Both,
                            Origin = Anchor.Centre,
                            Anchor = Anchor.Centre,
                            Width = 0.75f,
                        },
                        rightGlow = new Box
                        {
                            RelativeSizeAxes = Axes.Both,
                            Origin = Anchor.TopRight,
                            Anchor = Anchor.TopRight,
                            Width = 0.125f,
                        },
                    },
                },
                new Container
                {
                    RelativeSizeAxes = Axes.Both,
                    Origin = Anchor.Centre,
                    Anchor = Anchor.Centre,
                    Masking = true,
                    Children = new Drawable[]
                    {
                        colorContainer = new Container
                        {
                            RelativeSizeAxes = Axes.Both,
                            Origin = Anchor.Centre,
                            Anchor = Anchor.Centre,
                            Width = idle_width,
                            Masking = true,
                            MaskingSmoothness = 2,
                            Colour = Color4.Transparent,
                            Children = new Drawable[]
                            {
                                new Box
                                {
                                    EdgeSmoothness = new Vector2(2, 0),
                                    RelativeSizeAxes = Axes.Both,
                                },
                            },
                        },
                    },
                },
                spriteText = new TachyonSpriteText
                {
                    Text = Text,
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre,
                    Font = TachyonFont.GetFont(size: 28, weight: FontWeight.Bold),
                    Shadow = true,
                    ShadowColour = new Color4(0, 0, 0, 0.1f),
                    Colour = Color4.White,
                },
            };

            updateGlow();

            Selected.ValueChanged += selectionChanged;
        }

        private Color4 buttonColor;

        public Color4 ButtonColor
        {
            get => buttonColor;
            set
            {
                buttonColor = value;
                updateGlow();
            }
        }

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

        public float TextSize
        {
            get => spriteText.Font.Size;
            set => spriteText.Font = spriteText.Font.With(size: value);
        }

        private bool clickAnimating;

        protected override bool OnClick(ClickEvent e)
        {
            var flash = new Box
            {
                RelativeSizeAxes = Axes.Both,
                Colour = ButtonColor,
                Blending = BlendingParameters.Additive,
                Alpha = 0.05f
            };

            colorContainer.Add(flash);
            flash.FadeOutFromOne(100).Expire();

            clickAnimating = true;
            colorContainer.ResizeWidthTo(colorContainer.Width * 1.05f, 100, Easing.OutQuint)
                           .OnComplete(_ =>
                           {
                               clickAnimating = false;
                               Selected.TriggerChange();
                           });

            return base.OnClick(e);
        }

        protected override bool OnMouseDown(MouseDownEvent e)
        {
            colorContainer.ResizeWidthTo(hover_width * 0.98f, click_duration * 4, Easing.OutQuad);
            return base.OnMouseDown(e);
        }

        protected override void OnMouseUp(MouseUpEvent e)
        {
            if (Selected.Value)
                colorContainer.ResizeWidthTo(hover_width, click_duration, Easing.In);
            base.OnMouseUp(e);
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
            if (clickAnimating)
                return;

            if (args.NewValue)
            {
                spriteText.TransformSpacingTo(hoverSpacing, hover_duration, Easing.OutQuint);
                colorContainer.ResizeWidthTo(hover_width, hover_duration, Easing.OutQuint);
                colorContainer.Colour = buttonColor;
                glowContainer.FadeIn(hover_duration, Easing.OutQuint);
            }
            else
            {
                colorContainer.ResizeWidthTo(idle_width, hover_duration, Easing.OutQuint);
                colorContainer.Colour = Color4.Transparent;
                spriteText.TransformSpacingTo(Vector2.Zero, hover_duration, Easing.OutQuint);
                glowContainer.FadeOut(hover_duration, Easing.OutQuint);
            }
        }

        private void updateGlow()
        {
            leftGlow.Colour = ColourInfo.GradientHorizontal(new Color4(ButtonColor.R, ButtonColor.G, ButtonColor.B, 0f), ButtonColor);
            centerGlow.Colour = ButtonColor;
            rightGlow.Colour = ColourInfo.GradientHorizontal(ButtonColor, new Color4(ButtonColor.R, ButtonColor.G, ButtonColor.B, 0f));
        }
    }
}
