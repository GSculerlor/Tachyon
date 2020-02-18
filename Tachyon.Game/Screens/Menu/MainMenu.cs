using osu.Framework.Allocation;
using Tachyon.Game.Screens.Backgrounds;

namespace Tachyon.Game.Screens.Menu
{
    public class MainMenu : TachyonScreen
    {
        private TextureBackgroundScreen background;

        protected override BackgroundScreen CreateBackground() => background;

        [BackgroundDependencyLoader]
        private void load()
        {
            LoadComponentAsync(background = new TextureBackgroundScreen());
        }
    }
}