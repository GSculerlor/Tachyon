using System;
using osu.Framework.Allocation;
using osu.Framework.Extensions.Color4Extensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Effects;
using osu.Framework.Graphics.Shapes;
using osuTK;
using osuTK.Graphics;
using Tachyon.Game.Graphics;
using Tachyon.Game.Graphics.UserInterface;

namespace Tachyon.Game.Overlays.Toolbar
{
    public class Toolbar : VisibilityContainer
    {
        public const float HEIGHT = 40;
        private const double transition_time = 450;

        public ToolbarBackButton ToolbarBackButton;

        public Action Back;

        public Toolbar()
        {
            RelativeSizeAxes = Axes.X;
            Size = new Vector2(1, HEIGHT);
            Margin = new MarginPadding { Top = 20 };
            Padding = new MarginPadding { Horizontal = 20 };
        }

        [BackgroundDependencyLoader(true)]
        private void load(TachyonColor color, TachyonGame tachyonGame)
        {
            Child = new Container
            {
                RelativeSizeAxes = Axes.Both,
                Masking = true,
                MaskingSmoothness = 2,
                EdgeEffect = new EdgeEffectParameters
                {
                    Type = EdgeEffectType.Shadow,
                    Colour = Color4.Black.Opacity(0.2f),
                    Radius = 10,
                },
                Children = new Drawable[]
                {
                    new Box
                    {
                        RelativeSizeAxes = Axes.Both,
                        Colour = color.BackButtonGray
                    },
                    new FillFlowContainer
                    {
                        Direction = FillDirection.Horizontal,
                        RelativeSizeAxes = Axes.Y,
                        AutoSizeAxes = Axes.X,
                        Children = new Drawable[]
                        {
                            ToolbarBackButton = new ToolbarBackButton
                            {
                                Action = () => Back?.Invoke()
                            },
                        }
                    },
                    new FillFlowContainer
                    {
                        Anchor = Anchor.TopRight,
                        Origin = Anchor.TopRight,
                        Direction = FillDirection.Horizontal,
                        RelativeSizeAxes = Axes.Y,
                        AutoSizeAxes = Axes.X,
                        Children = new Drawable[]
                        {
                            new ToolbarMusicButton(),
                            new ToolbarSettingButton(),
                            new ToolbarNotificationButton(), 
                        }
                    }
                }
            };
        }
        
        protected override void PopIn()
        {
            this.MoveToY(0, transition_time, Easing.OutQuint);
            this.FadeIn(transition_time / 2, Easing.OutQuint);
        }

        protected override void PopOut()
        {
            this.MoveToY(-DrawSize.Y, transition_time, Easing.OutQuint);
            this.FadeOut(transition_time);
        }
    }
}