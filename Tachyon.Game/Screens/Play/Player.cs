using System;
using System.Linq;
using osu.Framework.Allocation;
using osu.Framework.Audio;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Input.Events;
using osu.Framework.Logging;
using osu.Framework.Screens;
using osu.Framework.Threading;
using osuTK.Input;
using Tachyon.Game.Beatmaps;
using Tachyon.Game.Graphics.Containers;
using Tachyon.Game.Rulesets;
using Tachyon.Game.Rulesets.UI;

namespace Tachyon.Game.Screens.Play
{
    [Cached]
    public class Player : DimmedBackgroundScreen
    {
        protected DrawableRuleset DrawableRuleset { get; private set; }
        
        private RulesetInfo rulesetInfo;
        private Ruleset ruleset;
        
        protected GameplayClockContainer GameplayClockContainer { get; private set; }
        
        public bool LoadedBeatmapSuccessfully => DrawableRuleset?.Objects.Any() == true;

        private GameplayBeatmap gameplayBeatmap;
        
        private DependencyContainer dependencies;

        protected override IReadOnlyDependencyContainer CreateChildDependencies(IReadOnlyDependencyContainer parent)
            => dependencies = new DependencyContainer(base.CreateChildDependencies(parent));

        [BackgroundDependencyLoader]
        private void load(AudioManager audio)
        {
            if (Beatmap.Value is PlaceholderWorkingBeatmap)
                return;

            IBeatmap playableBeatmap = loadPlayableBeatmap();

            if (playableBeatmap == null)
                return;
            
            DrawableRuleset = ruleset.CreateDrawableRulesetWith(playableBeatmap);
            
            InternalChild = GameplayClockContainer = new GameplayClockContainer(Beatmap.Value, DrawableRuleset.GameplayStartTime);

            AddInternal(gameplayBeatmap = new GameplayBeatmap(playableBeatmap));

            dependencies.CacheAs(gameplayBeatmap);
            
            addGameplayComponents(GameplayClockContainer);
        }
        
        private void addGameplayComponents(Container target)
        {
            target.Add(new ScalingContainer());

            target.AddRange(new Drawable[]
            {
                DrawableRuleset
            });
        }
            
        private IBeatmap loadPlayableBeatmap()
        {
            IBeatmap playable;

            try
            {
                if (Beatmap.Value.Beatmap == null)
                    throw new InvalidOperationException("Beatmap was not loaded");

                rulesetInfo = new TachyonRuleset().RulesetInfo;
                ruleset = rulesetInfo.CreateInstance();

                try
                {
                    playable = Beatmap.Value.GetPlayableBeatmap(ruleset.RulesetInfo);
                }
                catch (BeatmapInvalidForRulesetException)
                {
                    throw new BeatmapInvalidForRulesetException("Failed to load playable beatmap");
                }

                if (playable.HitObjects.Count == 0)
                {
                    Logger.Log("Beatmap contains no hit objects!", level: LogLevel.Error);
                    return null;
                }
            }
            catch (Exception e)
            {
                Logger.Error(e, "Could not load beatmap successfully!");
                //couldn't load, hard abort!
                return null;
            }

            return playable;
        }
        
        public override void OnEntering(IScreen last)
        {
            base.OnEntering(last);

            if (!LoadedBeatmapSuccessfully)
                return;

            Alpha = 0;
            this
                .ScaleTo(0.7f)
                .ScaleTo(1, 750, Easing.OutQuint)
                .Delay(250)
                .FadeIn(250);

            GameplayClockContainer.Restart();
            GameplayClockContainer.FadeInFromZero(750, Easing.OutQuint);
        }
        
        public override void OnSuspending(IScreen next)
        {
            fadeOut();
            base.OnSuspending(next);
        }
        
        public override bool OnExiting(IScreen next)
        {
            // ValidForResume is false when restarting
            if (ValidForResume)
            {
                if (!GameplayClockContainer.IsPaused.Value)
                    // still want to block if we are within the cooldown period and not already paused.
                    return true;
            }

            // GameplayClockContainer performs seeks / start / stop operations on the beatmap's track.
            // as we are no longer the current screen, we cannot guarantee the track is still usable.
            GameplayClockContainer?.StopUsingBeatmapClock();

            fadeOut();
            return base.OnExiting(next);
        }
        
        private void fadeOut(bool instant = false)
        {
            float fadeOutDuration = instant ? 0 : 250;
            this.FadeOut(fadeOutDuration);
        }
        
        private void performImmediateExit()
        {
            ValidForResume = false;

            performUserRequestedExit();
        }

        private void performUserRequestedExit()
        {
            if (!this.IsCurrentScreen()) return;

            this.Exit();
        }

        /// <summary>
        /// Restart gameplay via a parent <see cref="PlayerLoader"/>.
        /// <remarks>This can be called from a child screen in order to trigger the restart process.</remarks>
        /// </summary>
        public void Restart()
        {
            if (this.IsCurrentScreen())
                performImmediateExit();
            else
                this.MakeCurrent();
        }
        
        protected override bool OnKeyDown(KeyDownEvent e)
        {
            switch (e.Key)
            {
                case Key.Escape:
                    performImmediateExit();
                    return true;
            }
            
            return false;
        }
    }
}
