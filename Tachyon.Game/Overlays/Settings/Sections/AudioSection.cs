using osu.Framework.Graphics;
using Tachyon.Game.Overlays.Settings.Sections.Audio;

namespace Tachyon.Game.Overlays.Settings.Sections
{
    public class AudioSection : SettingsSection
    {
        public override string Header => "Audio";
        public AudioSection()
        {
            Children = new Drawable[]
            {
                new VolumeSettings(), 
            };
        }
    }
}
