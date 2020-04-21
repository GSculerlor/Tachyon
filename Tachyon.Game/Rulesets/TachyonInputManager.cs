using System.ComponentModel;
using osu.Framework.Input.Bindings;

namespace Tachyon.Game.Rulesets
{
    public class TachyonInputManager : RulesetInputManager<TachyonAction>
    {
        public TachyonInputManager(RulesetInfo ruleset)
            : base(ruleset, 0,  SimultaneousBindingMode.Unique)
        {
        }
    }
    
    public enum TachyonAction
    {
        [Description("Upper (First)")]
        UpperFirst,
        
        [Description("Upper (Second)")]
        UpperSecond,

        [Description("Lower (First)")]
        LowerFirst,
        
        [Description("Lower (Second)")]
        LowerSecond
    }
}
