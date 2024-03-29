﻿using System;
using osu.Framework.Allocation;
using osu.Framework.Testing;
using Tachyon.Game.Configuration;
using Tachyon.Game.Rulesets;

namespace Tachyon.Game.Tests.Visual
{
    public abstract class PlayerTestScene : RateAdjustedBeatmapTestScene
    {
        /// <summary>
        /// Whether custom test steps are provided. Custom tests should invoke <see cref="CreateTest"/> to create the test steps.
        /// </summary>
        protected virtual bool HasCustomSteps { get; } = false;

        private readonly Ruleset ruleset;

        protected TestPlayer Player;

        protected PlayerTestScene(Ruleset ruleset)
        {
            this.ruleset = ruleset;
        }

        protected TachyonConfigManager LocalConfig;

        [BackgroundDependencyLoader]
        private void load()
        {
            Dependencies.Cache(LocalConfig = new TachyonConfigManager(LocalStorage));
        }

        [SetUpSteps]
        public override void SetUpSteps()
        {
            base.SetUpSteps();

            if (!HasCustomSteps)
                CreateTest(null);
        }

        protected void CreateTest(Action action)
        {
            if (action != null && !HasCustomSteps)
                throw new InvalidOperationException($"Cannot add custom test steps without {nameof(HasCustomSteps)} being set.");

            action?.Invoke();

            AddStep(ruleset.RulesetInfo.Name, LoadPlayer);
            AddUntilStep("player loaded", () => Player.IsLoaded && Player.Alpha == 1);
        }

        protected virtual bool AllowFail => false;

        protected virtual bool Autoplay => false;

        protected void LoadPlayer()
        {
            var beatmap = CreateBeatmap();

            Beatmap.Value = CreateWorkingBeatmap(beatmap);

            Player = CreatePlayer(ruleset);
            LoadScreen(Player);
        }

        protected virtual TestPlayer CreatePlayer(Ruleset ruleset) => new TestPlayer();
    }
}
