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
            // This is a hotfix since input is on drawable level via TriggerEvent and it shouldn't be on Row
            // Due to judgement is handled by DrawableTachyonHitObject, not TachyonPlayfield
            // https://discordapp.com/channels/188630481301012481/589331078574112768/705526007976689734
            new KeyBinding(InputKey.MouseLeft, TachyonAction.MouseClick),
        };

        public override DrawableRuleset CreateDrawableRulesetWith(IBeatmap beatmap) => new DrawableTachyonRuleset(this, beatmap);

        public override IBeatmapConverter CreateBeatmapConverter(IBeatmap beatmap) => new TachyonBeatmapConverter(beatmap, this);

        public override ScoreProcessor CreateScoreProcessor() => new TachyonScoreProcessor();
    }
}
