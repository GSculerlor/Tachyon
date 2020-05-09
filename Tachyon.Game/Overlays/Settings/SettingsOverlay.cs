using System.Collections.Generic;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osuTK;
using Tachyon.Game.Overlays.Settings.Sections;
using Tachyon.Game.Overlays.Settings.Sections.KeyBindings;
using Tachyon.Game.Rulesets;

namespace Tachyon.Game.Overlays.Settings
{
    public class SettingsOverlay : SettingsPanel
    {
        protected override IEnumerable<SettingsSection> CreateSections() => new SettingsSection[]
        {
            new GraphicsSection(),
            new AudioSection(),
        };

        protected override Drawable CreateHeader() => new SettingsHeader("Settings", "Personalize your game behavior");


        [BackgroundDependencyLoader]
        private void load(TachyonRuleset ruleset)
        {
            AddSection(new RulesetBindingsSection(ruleset.RulesetInfo));
        }
    }
}
