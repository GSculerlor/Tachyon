﻿using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Input;
using osu.Framework.Input.Bindings;
using osu.Framework.Input.States;
using Tachyon.Game.Configuration;
using Tachyon.Game.Input.Bindings;

namespace Tachyon.Game.Rulesets
{
    public abstract class RulesetInputManager<T> : PassThroughInputManager
        where T : struct
    {
        protected override InputState CreateInitialState() => new RulesetInputManagerInputState(base.CreateInitialState());

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
    
    public class RulesetInputManagerInputState : InputState
    {
        public RulesetInputManagerInputState(InputState state = null)
            : base(state)
        {
        }
    }
}
