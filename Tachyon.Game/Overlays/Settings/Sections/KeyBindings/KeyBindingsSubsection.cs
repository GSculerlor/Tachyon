using System.Collections.Generic;
using System.Linq;
using osu.Framework.Allocation;
using osu.Framework.Extensions.IEnumerableExtensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Input.Bindings;
using osuTK;
using Tachyon.Game.Graphics;
using Tachyon.Game.Graphics.Sprites;
using Tachyon.Game.Graphics.UserInterface;
using Tachyon.Game.Input;
using Tachyon.Game.Rulesets;

namespace Tachyon.Game.Overlays.Settings.Sections.KeyBindings
{
    public abstract class KeyBindingsSubsection : SettingsSubsection
    {
        protected IEnumerable<KeyBinding> Defaults;

        protected RulesetInfo Ruleset;

        protected KeyBindingsSubsection()
        {
            FlowContent.Spacing = new Vector2(0, 1);
            FlowContent.Padding = new MarginPadding { Left = SettingsPanel.CONTENT_MARGINS, Right = SettingsPanel.CONTENT_MARGINS };
        }

        [BackgroundDependencyLoader]
        private void load(KeyBindingStore store)
        {
            var bindings = store.Query(Ruleset.ID, 0);

            foreach (var defaultGroup in Defaults.GroupBy(d => d.Action))
            {
                int intKey = (int)defaultGroup.Key;

                Add(new KeyBindingRow(defaultGroup.Key, bindings.Where(b => ((int)b.Action).Equals(intKey)))
                {
                    AllowMainMouseButtons = Ruleset != null,
                    Defaults = defaultGroup.Select(d => d.KeyCombination)
                });
            }

            Add(new ResetButton
            {
                Action = () => Children.OfType<KeyBindingRow>().ForEach(k => k.RestoreDefaults())
            });
        }
    }

    public class ResetButton : TachyonButton
    {
        [BackgroundDependencyLoader]
        private void load(TachyonColor colors)
        {
            Text = "Reset all bindings in section";
            RelativeSizeAxes = Axes.X;
            Margin = new MarginPadding { Top = 5 };
            Height = 20;

            Content.CornerRadius = 5;

            BackgroundColour = colors.PinkDark;
        }

        protected override SpriteText CreateText() => new TachyonSpriteText
        {
            Depth = -1,
            Origin = Anchor.Centre,
            Anchor = Anchor.Centre,
            Font = TachyonFont.GetFont(size: 20, weight: FontWeight.SemiBold)
        };
    }
}
