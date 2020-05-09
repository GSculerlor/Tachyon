using System.Collections.Generic;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using Tachyon.Game.Rulesets.Judgements;
using Tachyon.Game.Rulesets.Scoring;
using Tachyon.Game.Rulesets.UI;
using Tachyon.Game.Screens.Play;

namespace Tachyon.Game.Tests.Visual
{
    /// <summary>
    /// A player that exposes many components that would otherwise not be available, for testing purposes.
    /// </summary>
    public class TestPlayer : Player
    {
        public new DrawableRuleset DrawableRuleset => base.DrawableRuleset;

        public new HUDOverlay HUDOverlay => base.HUDOverlay;

        public new GameplayClockContainer GameplayClockContainer => base.GameplayClockContainer;

        public new ScoreProcessor ScoreProcessor => base.ScoreProcessor;

        public new HealthProcessor HealthProcessor => base.HealthProcessor;

        public readonly List<JudgementResult> Results = new List<JudgementResult>();

        [BackgroundDependencyLoader]
        private void load()
        {
            ScoreProcessor.NewJudgement += r => Results.Add(r);
        }
    }
}
