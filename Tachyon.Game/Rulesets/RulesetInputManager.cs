using System.ComponentModel;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Input;
using osu.Framework.Input.Bindings;
using osu.Framework.Input.States;
using Tachyon.Game.Configuration;
using Tachyon.Game.Input.Bindings;
using Container = osu.Framework.Graphics.Containers.Container;

namespace Tachyon.Game.Rulesets
{
    public abstract class RulesetInputManager<T> : PassThroughInputManager
        where T : struct
    {
        protected override InputState CreateInitialState()
        {
            var state = base.CreateInitialState();
            return new RulesetInputManagerInputState<T>(state.Mouse, state.Keyboard, state.Joystick);
        }

        protected readonly KeyBindingContainer<T> KeyBindingContainer;

        protected override Container<Drawable> Content => content;

        private readonly Container content;

        protected RulesetInputManager(RulesetInfo ruleset, int variant, SimultaneousBindingMode unique)
        {
            InternalChild = KeyBindingContainer =
                CreateKeyBindingContainer(ruleset, variant, unique)
                    .WithChild(content = new Container { RelativeSizeAxes = Axes.Both });
        }

        [BackgroundDependencyLoader(true)]
        private void load(TachyonConfigManager config)
        {
        }

        protected virtual KeyBindingContainer<T> CreateKeyBindingContainer(RulesetInfo ruleset, int variant, SimultaneousBindingMode unique)
            => new RulesetKeyBindingContainer(ruleset, variant, unique);

        public class RulesetKeyBindingContainer : DatabasedKeyBindingContainer<T>
        {
            public RulesetKeyBindingContainer(RulesetInfo ruleset, int variant, SimultaneousBindingMode unique)
                : base(ruleset, variant, unique)
            {
            }
        }
    }
    
    public class RulesetInputManagerInputState<T> : InputState
        where T : struct
    {
        public RulesetInputManagerInputState(MouseState mouse = null, KeyboardState keyboard = null, JoystickState joystick = null)
            : base(mouse, keyboard, joystick)
        {
        }
    }
}
