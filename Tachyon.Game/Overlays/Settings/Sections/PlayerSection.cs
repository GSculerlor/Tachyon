using osu.Framework.Graphics;
using osu.Framework.Graphics.Sprites;
using Tachyon.Game.Overlays.Settings.Sections.Graphics;
using Tachyon.Game.Overlays.Settings.Sections.Player;

namespace Tachyon.Game.Overlays.Settings.Sections
{
    public class PlayerSection : SettingsSection
    {
        public override string Header => "Players";
        public PlayerSection()
        {
            Children = new Drawable[]
            {
                new PlayerSettings(), 
            };
        }
    }
}
