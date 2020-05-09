using Tachyon.Game.Overlays.Settings.Sections.KeyBindings;
using Tachyon.Game.Rulesets;

namespace Tachyon.Game.Overlays.Settings.Sections
{
    public class RulesetBindingsSection : SettingsSection
    {
        public override string Header => "Key Binding";

        private readonly RulesetInfo ruleset;

        public RulesetBindingsSection(RulesetInfo ruleset)
        {
            this.ruleset = ruleset;

            var r = ruleset.CreateInstance();

            Add(new VariantBindingsSubsection(ruleset));
        }
    }
}
