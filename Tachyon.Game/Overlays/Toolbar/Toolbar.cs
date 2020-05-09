using System;
using osu.Framework.Allocation;
using osu.Framework.Extensions.Color4Extensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osuTK;
using osuTK.Graphics;
using Tachyon.Game.Graphics;
using Tachyon.Game.Screens;
using Container = osu.Framework.Graphics.Containers.Container;

namespace Tachyon.Game.Overlays.Toolbar
{
    public class Toolbar : VisibilityContainer
    {
        public const float HEIGHT = 50;
        private const double transition_time = 450;

        public Action EditorAction;
        
        public Toolbar()
        {
            RelativeSizeAxes = Axes.X;
            Anchor = Anchor.TopLeft;
            Origin = Anchor.TopLeft;
            Size = new Vector2(1, HEIGHT);
        }
        
        [BackgroundDependencyLoader(true)]
        private void load(TachyonColor color, TachyonGame tachyonGame)
        {
            Children = new Drawable[]
            {
                new Box
                {
                    RelativeSizeAxes = Axes.Both,
                    Colour = Color4.Black.Opacity(0.4f)
                }, 
                new Container
                {
                    RelativeSizeAxes = Axes.Both,
                    Masking = true,
                    MaskingSmoothness = 2,
                    Children = new Drawable[]
                    {
                        new FillFlowContainer
                        {
                            Anchor = Anchor.BottomRight,
                            Origin = Anchor.BottomRight,
                            Direction = FillDirection.Horizontal,
                            RelativeSizeAxes = Axes.Y,
                            AutoSizeAxes = Axes.X,
                            Spacing = new Vector2(40, 0),
                            Padding = new MarginPadding { Right = 20 },
                            Children = new Drawable[]
                            {
                                new ToolbarEditorButton
                                {
                                    EditorAction = EditorAction
                                }, 
                                new ToolbarSettingsButton(),
                                new ToolbarMusicButton(),
                                new ToolbarFullscreenButton(), 
                            }
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
            this.MoveToY(DrawSize.Y, transition_time, Easing.OutQuint);
            this.FadeOut(transition_time);
        }
    }
}
