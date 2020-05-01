using System.Collections.Generic;
using osu.Framework.Input.Bindings;
using Tachyon.Game.Beatmaps;
using Tachyon.Game.Rulesets.Beatmaps;
using Tachyon.Game.Rulesets.Scoring;
using Tachyon.Game.Rulesets.UI;
using Tachyon.Game.Rulesets.UI.Scrolling;

namespace Tachyon.Game.Rulesets
{
    public class TachyonRuleset : Ruleset
    {
        public override string Description => "Tachyon Ruleset";

        public override string ShortName => "Tachyon";
        
        public override int RulesetID => 1;
        
        public override IEnumerable<KeyBinding> GetDefaultKeyBindings(int variant = 0) => new[]
        {
            new KeyBinding(InputKey.D, TachyonAction.UpperFirst),
            new KeyBinding(InputKey.F, TachyonAction.UpperSecond),
            new KeyBinding(InputKey.J, TachyonAction.LowerFirst),
            new KeyBinding(InputKey.K, TachyonAction.LowerSecond),
        };

        public override DrawableRuleset CreateDrawableRulesetWith(IBeatmap beatmap) => new DrawableTachyonRuleset(this, beatmap);

        public override IBeatmapConverter CreateBeatmapConverter(IBeatmap beatmap) => new TachyonBeatmapConverter(beatmap, this);

        public override ScoreProcessor CreateScoreProcessor() => new TachyonScoreProcessor();
    }
}
