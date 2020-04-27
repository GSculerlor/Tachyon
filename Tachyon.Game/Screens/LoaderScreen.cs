using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Screens;
using osuTK.Graphics;
using Tachyon.Game.Screens.Menu;

namespace Tachyon.Game.Screens
{
    public class LoaderScreen : TachyonScreen
    {
        public override bool AllowBackButton => false;

        private TachyonScreen loadableScreen;
        
        public LoaderScreen()
        {
            ValidForResume = false;
            
            AddInternal(new Container
            {
                RelativeSizeAxes = Axes.Both,
                Children = new Drawable[]
                {
                    new Box
                    {
                        Origin = Anchor.Centre,
                        Anchor = Anchor.Centre,
                        RelativeSizeAxes = Axes.Both,
                        Colour = Color4.Black,
                    },
                },
            });
        }

        public override void OnEntering(IScreen last)
        {
            base.OnEntering(last);

            LoadComponentAsync(loadableScreen = createLoadableScreen());
            
            checkIfLoaded();
        }
        
        private void checkIfLoaded()
        {
            if (loadableScreen.LoadState != LoadState.Ready)
            {
                Schedule(checkIfLoaded);
                return;
            }

            this.Push(loadableScreen);
        }

        private TachyonScreen createLoadableScreen()
        {
            return new HeadsetTextScreen(new IntroScreen());
        }
    }
}
