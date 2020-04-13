using osu.Framework.Allocation;
using osu.Framework.Graphics.Sprites;
using Tachyon.Game.Overlays.Music;

namespace Tachyon.Game.Overlays.Toolbar
{
    public class ToolbarMusicButton : ToolbarOverlayToggleButton
    {
        public ToolbarMusicButton()
        {
            Icon = FontAwesome.Solid.Music;
        }
        
        [BackgroundDependencyLoader(true)]
        private void load(MusicPlayerOverlay music)
        {
            StateContainer = music;
        }
    }
}