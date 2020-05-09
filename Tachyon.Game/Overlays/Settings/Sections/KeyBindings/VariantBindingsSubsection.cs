using Tachyon.Game.Rulesets;

namespace Tachyon.Game.Overlays.Settings.Sections.KeyBindings
{
    public class VariantBindingsSubsection : KeyBindingsSubsection
    {
        protected override string Header { get; }

        public VariantBindingsSubsection(RulesetInfo ruleset)
        {
            Ruleset = ruleset;

            var rulesetInstance = ruleset.CreateInstance();

            Header = rulesetInstance.ShortName;
            Defaults = rulesetInstance.GetDefaultKeyBindings();
        }
    }
}
