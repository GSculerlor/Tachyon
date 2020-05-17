using osu.Framework.Extensions.Color4Extensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Effects;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Input.Events;
using osuTK;
using osuTK.Graphics;
using Tachyon.Game.Graphics.Containers;
using Tachyon.Game.Graphics.Sprites;

namespace Tachyon.Game.Graphics.UserInterface
{
     public class HoverableBackButton : TachyonClickableContainer
    {
        public Box TextLayer;

        private const int transform_time = 600;
        private readonly Vector2 shear = new Vector2(5f / 50, 0);

        public static readonly Vector2 SIZE_EXTENDED = new Vector2(100, 50);
        public static readonly Vector2 SIZE_RETRACTED = new Vector2(80, 50);
        private readonly SpriteText text;

        public Color4 HoverColour;
        private readonly Container c1;

        public Color4 BackgroundColour
        {
            set
            {
                TextLayer.Colour = value;
            }
        }

        public override Anchor Origin
        {
            get => base.Origin;
            set
            {
                base.Origin = value;
                c1.Origin = c1.Anchor = value;

                X = value.HasFlag(Anchor.x2) ? 5f : 0;

                Remove(c1);
                c1.Depth = value.HasFlag(Anchor.x2) ? 0 : 1;
                Add(c1);
            }
        }

        public HoverableBackButton()
        {
            Size = SIZE_RETRACTED;
            Shear = shear;

            Children = new Drawable[]
            {
                c1 = new Container
                {
                    Origin = Anchor.TopRight,
                    Anchor = Anchor.TopRight,
                    RelativeSizeAxes = Axes.Both,
                    Width = 1f,
                    Children = new Drawable[]
                    {
                        new Container
                        {
                            RelativeSizeAxes = Axes.Both,
                            Masking = true,
                            MaskingSmoothness = 2,
                            EdgeEffect = new EdgeEffectParameters
                            {
                                Type = EdgeEffectType.Shadow,
                                Colour = Color4.Black.Opacity(0.2f),
                                Offset = new Vector2(2, 0),
                                Radius = 2,
                            },
                            Children = new[]
                            {
                                TextLayer = new Box
                                {
                                    Origin = Anchor.TopLeft,
                                    Anchor = Anchor.TopLeft,
                                    RelativeSizeAxes = Axes.Both,
                                    EdgeSmoothness = new Vector2(2, 0),
                                },
                            }
                        },
                        text = new TachyonSpriteText
                        {
                            Origin = Anchor.Centre,
                            Anchor = Anchor.Centre,
                            Font = TachyonFont.Default.With(weight: FontWeight.SemiBold)
                        }
                    }
                },
            };
        }

        public string Text
        {
            set => text.Text = value;
        }

        public override bool ReceivePositionalInputAt(Vector2 screenSpacePos) => TextLayer.ReceivePositionalInputAt(screenSpacePos);

        protected override bool OnHover(HoverEvent e)
        {
            this.ResizeTo(SIZE_EXTENDED, transform_time, Easing.Out);

            return true;
        }

        protected override void OnHoverLost(HoverLostEvent e)
        {
            this.ResizeTo(SIZE_RETRACTED, transform_time, Easing.Out);
        }

        protected override bool OnMouseDown(MouseDownEvent e) => true;

        protected override bool OnClick(ClickEvent e)
        {
            var flash = new Box
            {
                RelativeSizeAxes = Axes.Both,
                Colour = Color4.White.Opacity(0.5f),
            };
            Add(flash);

            flash.Alpha = 1;
            flash.FadeOut(500, Easing.OutQuint);
            flash.Expire();

            return base.OnClick(e);
        }
    }
}
