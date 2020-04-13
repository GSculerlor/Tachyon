using System.ComponentModel;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osuTK;
using Tachyon.Game.Graphics;
using Tachyon.Game.Graphics.UserInterface;
using Container = osu.Framework.Graphics.Containers.Container;

namespace Tachyon.Game.Overlays.Toolbar
{
    public class Toolbar : VisibilityContainer
    {
        public const float HEIGHT = 60;
        private const double transition_time = 450;
        
        public readonly Bindable<MenuScreen> Screen = new Bindable<MenuScreen>();
        
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
            TachyonTabControl<MenuScreen> screenTabControl;

            Child = new Container
            {
                RelativeSizeAxes = Axes.Both,
                Masking = true,
                MaskingSmoothness = 2,
                Children = new Drawable[]
                {
                    new Container
                    {
                        RelativeSizeAxes = Axes.Both,
                        Padding = new MarginPadding { Top = 10, Left = 40 },
                        Children = new Drawable[]
                        {
                            screenTabControl = new TachyonTabControl<MenuScreen>
                            {
                                Margin = new MarginPadding(4),
                                Origin = Anchor.BottomLeft,
                                Anchor = Anchor.BottomLeft,
                                RelativeSizeAxes = Axes.X,
                                Height = 24,
                                AutoSort = true,
                                Spacing = new Vector2(40f, 0)
                            }, 
                        }
                    },
                    new FillFlowContainer
                    {
                        Anchor = Anchor.BottomRight,
                        Origin = Anchor.BottomRight,
                        Direction = FillDirection.Horizontal,
                        RelativeSizeAxes = Axes.Y,
                        AutoSizeAxes = Axes.X,
                        Spacing = new Vector2(40, 0),
                        Padding = new MarginPadding { Top = 10, Right = 20 },
                        Children = new Drawable[]
                        {
                            new ToolbarMusicButton(),
                            new ToolbarFullscreenButton(), 
                        }
                    }
                }
            };
            
            Screen.BindTo(screenTabControl.Current);
        }

        protected override void LoadComplete()
        {
            base.LoadComplete();
            
            Screen.TriggerChange();
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
        
        public enum MenuScreen
        {
            [Description("Home")]
            Home,
            
            [Description("Playground")]
            Playground,

            [Description("Editor")]
            Editor,

            [Description("Settings")]
            Settings,
        }
    }
}
