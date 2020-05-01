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
using Tachyon.Game.Rulesets.Scoring;
using Tachyon.Game.Rulesets.UI;
using Tachyon.Game.Scoring;
using Tachyon.Game.Screens.Result;

namespace Tachyon.Game.Screens.Play
{
    [Cached]
    public class Player : DimmedBackgroundScreen
    {
        protected DrawableRuleset DrawableRuleset { get; private set; }
        
        protected ScoreProcessor ScoreProcessor { get; private set; }
        
        private RulesetInfo rulesetInfo;
        private Ruleset ruleset;
        
        [Resolved]
        private ScoreManager scoreManager { get; set; }
        
        protected GameplayClockContainer GameplayClockContainer { get; private set; }
        
        protected HUDOverlay HUDOverlay { get; private set; }
        
        protected PauseOverlay PauseOverlay { get; private set; }
        
        public bool LoadedBeatmapSuccessfully => DrawableRuleset?.Objects.Any() == true;

        private GameplayBeatmap gameplayBeatmap;
        
        private DependencyContainer dependencies;
        public Action RestartRequested;

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
            
            ScoreProcessor = ruleset.CreateScoreProcessor();
            ScoreProcessor.ApplyBeatmap(playableBeatmap);
            
            InternalChild = GameplayClockContainer = new GameplayClockContainer(Beatmap.Value, DrawableRuleset.GameplayStartTime);

            AddInternal(gameplayBeatmap = new GameplayBeatmap(playableBeatmap));

            dependencies.CacheAs(gameplayBeatmap);
            
            addGameplayComponents(GameplayClockContainer);
            addOverlayComponents(GameplayClockContainer);
            
            DrawableRuleset.OnNewResult += r =>
            {
                ScoreProcessor.ApplyResult(r);
            };

            DrawableRuleset.OnRevertResult += r =>
            {
                ScoreProcessor.RevertResult(r);
            };
            
            ScoreProcessor.AllJudged += onCompletion;
        }
        
        private void addGameplayComponents(Container target)
        {
            target.Add(new ScalingContainer());

            target.AddRange(new Drawable[]
            {
                DrawableRuleset
            });
            
            DrawableRuleset.FrameStableComponents.AddRange(new Drawable[]
            {
                ScoreProcessor
            });
        }

        private void addOverlayComponents(Container target)
        {
            target.AddRange(new Drawable[]
            {
                HUDOverlay = new HUDOverlay(ScoreProcessor, DrawableRuleset)
                {
                    Pause =
                    {
                        Action = performUserRequestedExit,
                        IsPaused = { BindTarget = GameplayClockContainer.IsPaused }
                    },
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre
                },
                PauseOverlay = new PauseOverlay
                {
                    OnResume = Resume,
                    OnRetry = Restart,
                    OnQuit = performUserRequestedExit,
                },
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
        
        public bool IsResuming { get; private set; }

        private const double pause_cooldown = 1000;

        private double? lastPauseActionTime;

        private bool canPause =>
            LoadedBeatmapSuccessfully && ValidForResume
            // cannot pause if already paused (or in a cooldown state) unless we are in a resuming state.
            && (IsResuming || (GameplayClockContainer.IsPaused.Value == false && !pauseCooldownActive));

        private bool pauseCooldownActive =>
            lastPauseActionTime.HasValue && GameplayClockContainer.GameplayClock.CurrentTime < lastPauseActionTime + pause_cooldown;

        private bool canResume =>
            // cannot resume from a non-paused state
            GameplayClockContainer.IsPaused.Value
            // already resuming
            && !IsResuming;

        public void Pause()
        {
            if (!canPause) return;

            if (IsResuming)
            {
                DrawableRuleset.CancelResume();
                IsResuming = false;
            }

            GameplayClockContainer.Stop();
            PauseOverlay.Show();
            lastPauseActionTime = GameplayClockContainer.GameplayClock.CurrentTime;
        }

        public void Resume()
        {
            if (!canResume) return;

            IsResuming = true;
            PauseOverlay.Hide();

            DrawableRuleset.RequestResume(completeResume);

            void completeResume()
            {
                GameplayClockContainer.Start();
                IsResuming = false;
            }
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
            if (completionProgressDelegate != null && !completionProgressDelegate.Cancelled && !completionProgressDelegate.Completed)
            {
                // proceed to result screen if beatmap already finished playing
                completionProgressDelegate.RunTask();
                return true;
            }
            
            // ValidForResume is false when restarting
            if (ValidForResume)
            {
                if (!GameplayClockContainer.IsPaused.Value)
                    // still want to block if we are within the cooldown period and not already paused.
                    return true;
            }
            
            if (canPause)
            {
                Pause();
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
            completionProgressDelegate?.Cancel();
            
            ValidForResume = false;

            performUserRequestedExit();
        }

        private void performUserRequestedExit()
        {
            if (!this.IsCurrentScreen()) return;

            if (canPause)
                Pause();
            else
                this.Exit();
        }

        /// <summary>
        /// Restart gameplay via a parent <see cref="PlayerLoader"/>.
        /// <remarks>This can be called from a child screen in order to trigger the restart process.</remarks>
        /// </summary>
        public void Restart()
        {
            RestartRequested?.Invoke();
            
            if (this.IsCurrentScreen())
                performImmediateExit();
            else
                this.MakeCurrent();
        }
        
        private ScheduledDelegate completionProgressDelegate;

        private void onCompletion()
        {
            // screen may be in the exiting transition phase.
            if (!this.IsCurrentScreen())
                return;

            // Only show the completion screen if the player hasn't failed
            if (completionProgressDelegate != null)
                return;

            ValidForResume = false;

            using (BeginDelayedSequence(1000))
                scheduleToResult();
        }
        
        private void scheduleToResult()
        {
            completionProgressDelegate?.Cancel();
            completionProgressDelegate = Schedule(gotoResult);
        }

        private void gotoResult()
        {    
            var score = new Score { ScoreInfo = createScore() };
            
            scoreManager.Import(score.ScoreInfo)
                        .ContinueWith(imported => Schedule(() =>
                        {
                            if (this.IsCurrentScreen())
                                this.Push(new ResultScreen(imported.Result));
                        }));
        }
        
        private ScoreInfo createScore()
        {
            var score = new ScoreInfo
            {
                Beatmap = Beatmap.Value.BeatmapInfo
            };

            ScoreProcessor.PopulateScore(score);

            return score;
        }
        
        protected override bool OnKeyDown(KeyDownEvent e)
        {
            switch (e.Key)
            {
                //TODO: Ganti pake IKeyBindingHandler
                case Key.Escape:
                    performUserRequestedExit();
                    return true;
            }
            
            return false;
        }
    }
}
