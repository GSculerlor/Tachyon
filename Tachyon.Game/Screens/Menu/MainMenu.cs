using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Screens;
using Tachyon.Game.Overlays.Toolbar;
using Tachyon.Game.Screens.Backgrounds;
using Tachyon.Game.Screens.Placeholder;
using Tachyon.Game.Screens.Playground;

namespace Tachyon.Game.Screens.Menu
{
    public class MainMenu : TachyonScreen
    {
        private BackgroundScreen background;
        private Screen currentScreen;
        private readonly ScreenStack content;

        protected override BackgroundScreen CreateBackground() => background;

        public MainMenu()
        {
            Padding = new MarginPadding { Top = Toolbar.HEIGHT };
            AddInternal(content = new TachyonScreenStack
            {
                Name = "Screen container",
                RelativeSizeAxes = Axes.Both,
            });
        }
        
        [BackgroundDependencyLoader]
        private void load(Bindable<Toolbar.MenuScreen> screen)
        {
            LoadComponentAsync(background = new DefaultBackgroundScreen());

            screen.ValueChanged += onScreenTabControlChange;
            screen.TriggerChange();
        }

        private void onScreenTabControlChange(ValueChangedEvent<Toolbar.MenuScreen> e)
        {
            currentScreen?.Exit();

            switch (e.NewValue)
            {
                case Toolbar.MenuScreen.Home:
                    currentScreen = new HomeScreen();
                    break;
                
                case Toolbar.MenuScreen.Playground:
                    currentScreen = new PlaygroundScreen();
                    break;
                
                case Toolbar.MenuScreen.Editor:
                    currentScreen = new PlaceholderScreen("Editor", true);
                    break;
                
                case Toolbar.MenuScreen.Settings:
                    currentScreen = new PlaceholderScreen("Settings", true);
                    break;
            }

            content.Push(currentScreen);
        }
        
        public override void OnResuming(IScreen last)
        {
            base.OnResuming(last);
            
            (Background as DefaultBackgroundScreen)?.Next();
        }
    }
}