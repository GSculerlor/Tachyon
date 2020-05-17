using osu.Framework.Graphics;
using osu.Framework.Graphics.Sprites;
using Tachyon.Game.Overlays.Settings.Sections.Graphics;

namespace Tachyon.Game.Overlays.Settings.Sections
{
    public class GraphicsSection : SettingsSection
    {
        public override string Header => "Graphics";
        public GraphicsSection()
        {
            Children = new Drawable[]
            {
                new GraphicsSettings(), 
            };
        }
    }
}
