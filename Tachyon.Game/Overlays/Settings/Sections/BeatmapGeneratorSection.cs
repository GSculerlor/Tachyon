using osu.Framework.Graphics;
using Tachyon.Game.Overlays.Settings.Sections.BeatmapGenerator;
using Tachyon.Game.Overlays.Settings.Sections.Graphics;

namespace Tachyon.Game.Overlays.Settings.Sections
{
    public class BeatmapGeneratorSection : SettingsSection
    {
        public override string Header => "Beatmap Generator";
        public BeatmapGeneratorSection()
        {
            Children = new Drawable[]
            {
                new PatternGeneratorSettings(), 
            };
        }
    }
}
