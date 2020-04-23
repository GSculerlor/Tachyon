using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Screens;
using osuTK;
using Tachyon.Game.Graphics;
using Tachyon.Game.Graphics.Containers;
using FontWeight = Tachyon.Game.Graphics.FontWeight;

namespace Tachyon.Game.Screens.Menu
{
    public class HeadsetTextScreen : TachyonScreen
    {
        private readonly TachyonScreen nextScreen;
        
        private FillFlowContainer fill;
        private TachyonTextFlowContainer textFlow;

        public HeadsetTextScreen(TachyonScreen nextScreen)
        {
            this.nextScreen = nextScreen;
            ValidForResume = false;
        }

        [BackgroundDependencyLoader]
        private void load()
        {
            InternalChild = fill = new FillFlowContainer
            {
                RelativeSizeAxes = Axes.Both,
                Direction = FillDirection.Vertical,
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                Children = new Drawable[]
                {
                    textFlow = new TachyonTextFlowContainer
                    {
                        RelativeSizeAxes = Axes.X,
                        AutoSizeAxes = Axes.Y,
                        TextAnchor = Anchor.Centre,
                        Anchor = Anchor.Centre,
                        Origin = Anchor.Centre,
                        Spacing = new Vector2(0, 2),
                    },
                }
            };
            
            textFlow.AddText("Use headphone for the best experience.", t => t.Font = TachyonFont.Default.With(size: 50, weight: FontWeight.Bold));
        }
        
        public override bool AllowBackButton => false;

        public override bool ToolbarVisible => false;

        public override bool CursorVisible => false;
        
        protected override void LoadComplete()
        {
            base.LoadComplete();
            if (nextScreen != null)
                LoadComponentAsync(nextScreen);
        }
        
        public override void OnEntering(IScreen last)
        {
            base.OnEntering(last);

            this
                .FadeInFromZero(500)
                .Then(5500)
                .FadeOut(1000)
                .ScaleTo(0.5f, 1000, Easing.InQuint)
                .Finally(d =>
                {
                    if (nextScreen != null)
                        this.Push(nextScreen);
                });
        }
    }
}
