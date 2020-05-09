using osu.Framework.Allocation;
using osu.Framework.Graphics.Sprites;
using Tachyon.Game.Overlays.Music;
using Tachyon.Game.Overlays.Settings;

namespace Tachyon.Game.Overlays.Toolbar
{
    public class ToolbarSettingsButton : ToolbarOverlayToggleButton
    {
        public ToolbarSettingsButton()
        {
            Icon = FontAwesome.Solid.Cog;
        }
        
        [BackgroundDependencyLoader(true)]
        private void load(SettingsOverlay settings)
        {
            StateContainer = settings;
        }
    }
}