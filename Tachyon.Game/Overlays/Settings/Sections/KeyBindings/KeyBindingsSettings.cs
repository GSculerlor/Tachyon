using osu.Framework.Allocation;
using osu.Framework.Graphics;
using Tachyon.Game.Rulesets;

namespace Tachyon.Game.Overlays.Settings.Sections.KeyBindings
{
    public class KeyBindingsSettings : SettingsSubsection
    {
        protected override string Header => "Key Binding";

        [BackgroundDependencyLoader]
        private void load(TachyonRuleset ruleset)
        {
            Children = new Drawable[]
            {
                new RulesetBindingsSection(ruleset.RulesetInfo), 
            };
        }
    }
}
