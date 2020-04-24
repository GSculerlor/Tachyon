using System;
using System.Collections.Generic;
using System.Linq;
using osu.Framework.Allocation;
using osu.Framework.Audio;
using osu.Framework.Audio.Track;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Logging;
using osu.Framework.Screens;
using osu.Framework.Threading;
using osuTK.Graphics;
using Tachyon.Game.Beatmaps;
using Tachyon.Game.Components;
using Tachyon.Game.Graphics;
using Tachyon.Game.Screens.Backgrounds;
using Tachyon.Game.Screens.Select.Detail;

namespace Tachyon.Game.Screens.Select
{
    public abstract class SongSelect : TachyonScreen
    {
        protected override BackgroundScreen CreateBackground() => new BeatmapBackgroundScreen(Beatmap.Value);
        
        protected BeatmapCarousel Carousel { get; private set; }

        private BeatmapDetail beatmapDetail;
        
        [Resolved]
        private BeatmapManager beatmaps { get; set; }
        
        [Resolved(canBeNull: true)]
        private MusicController music { get; set; }

        [BackgroundDependencyLoader]
        private void load(AudioManager audio, TachyonColor colors)
        {
            AddRangeInternal(new Drawable[]
            {
                new VerticalMaskingContainer
                {
                    Children = new Drawable[]
                    {
                        new GridContainer 
                        {
                            RelativeSizeAxes = Axes.Both,
                            ColumnDimensions = new[]
                            {
                                new Dimension(GridSizeMode.Relative, 0.4f),
                                new Dimension(GridSizeMode.Relative, 0.6f),
                            },
                            Content = new[]
                            {
                                new Drawable[]
                                {
                                    new Container
                                    {
                                        Origin = Anchor.BottomLeft,
                                        Anchor = Anchor.BottomLeft,
                                        RelativeSizeAxes = Axes.Both,

                                        Children = new Drawable[]
                                        {
                                            beatmapDetail = new BeatmapDetail
                                            {
                                                Height = 240,
                                                RelativeSizeAxes = Axes.X,
                                                Margin = new MarginPadding
                                                {
                                                    Top = 20,
                                                    Right = 40,
                                                },
                                                ClickedAction = () => FinaliseSelection()
                                            }
                                        }
                                    },
                                    new Container
                                    {
                                        Anchor = Anchor.CentreRight,
                                        Origin = Anchor.CentreRight,
                                        RelativeSizeAxes = Axes.Both,
                                        Child = Carousel = new BeatmapCarousel
                                        {
                                            AllowSelection = false,
                                            Anchor = Anchor.CentreRight,
                                            Origin = Anchor.CentreRight,
                                            RelativeSizeAxes = Axes.Both,
                                            SelectionChanged = updateSelectedBeatmap,
                                            BeatmapSetsChanged = carouselBeatmapsLoaded,
                                        },
                                    },
                                },
                            }
                        }
                    }
                },
            });
        }
        
        private DependencyContainer dependencies;

        protected override IReadOnlyDependencyContainer CreateChildDependencies(IReadOnlyDependencyContainer parent)
        {
            dependencies = new DependencyContainer(base.CreateChildDependencies(parent));

            dependencies.CacheAs(this);

            return dependencies;
        }
        
        /// <summary>
        /// Call to make a selection and perform the default action for this SongSelect.
        /// </summary>
        /// <param name="beatmap">An optional beatmap to override the current carousel selection.</param>
        /// <param name="performStartAction">Whether to trigger <see cref="OnStart"/>.</param>
        public void FinaliseSelection(BeatmapInfo beatmap = null, bool performStartAction = true)
        {
            // This is very important as we have not yet bound to screen-level bindables before the carousel load is completed.
            if (!Carousel.BeatmapSetsLoaded)
                return;

            // avoid attempting to continue before a selection has been obtained.
            // this could happen via a user interaction while the carousel is still in a loading state.
            if (Carousel.SelectedBeatmap == null) return;

            if (beatmap != null)
                Carousel.SelectBeatmap(beatmap);

            if (selectionChangedDebounce?.Completed == false)
            {
                selectionChangedDebounce.RunTask();
                selectionChangedDebounce.Cancel(); // cancel the already scheduled task.
                selectionChangedDebounce = null;
            }

            if (performStartAction && OnStart())
                Carousel.AllowSelection = false;
        }
        
        /// <summary>
        /// Called when a selection is made.
        /// </summary>
        /// <returns>If a resultant action occurred that takes the user away from SongSelect.</returns>
        protected abstract bool OnStart();

        private ScheduledDelegate selectionChangedDebounce;
        
        private void workingBeatmapChanged(ValueChangedEvent<WorkingBeatmap> e)
        {
            if (e.NewValue is PlaceholderWorkingBeatmap || !this.IsCurrentScreen()) return;

            Logger.Log($"working beatmap updated to {e.NewValue}");

            if (!Carousel.SelectBeatmap(e.NewValue.BeatmapInfo, false))
            {
                Carousel.SelectBeatmap(e.NewValue.BeatmapInfo);
            }
        }
        
        // We need to keep track of the last selected beatmap ignoring debounce to play the correct selection sounds.
        private BeatmapInfo beatmapNoDebounce;

        private void updateSelectedBeatmap(BeatmapInfo beatmap)
        {
            if (beatmap?.Equals(beatmapNoDebounce) == true)
                return;

            beatmapNoDebounce = beatmap;

            performUpdateSelected();
        }

        /// <summary>
        /// selection has been changed as the result of a user interaction.
        /// </summary>
        private void performUpdateSelected()
        {
            var beatmap = beatmapNoDebounce;

            selectionChangedDebounce?.Cancel();

            if (beatmap == null)
                run();
            else
                selectionChangedDebounce = Scheduler.AddDelayed(run, 200);

            void run()
            {
                Logger.Log($"updating selection with beatmap:{beatmap?.ID.ToString() ?? "null"}");

                // We may be arriving here due to another component changing the bindable Beatmap.
                // In these cases, the other component has already loaded the beatmap, so we don't need to do so again.
                if (!EqualityComparer<BeatmapInfo>.Default.Equals(beatmap, Beatmap.Value.BeatmapInfo))
                {
                    Logger.Log($"beatmap changed from \"{Beatmap.Value.BeatmapInfo}\" to \"{beatmap}\"");

                    WorkingBeatmap previous = Beatmap.Value;
                    Beatmap.Value = beatmaps.GetWorkingBeatmap(beatmap, previous);
                }

                if (this.IsCurrentScreen())
                    ensurePlayingSelected();

                updateComponentFromBeatmap(Beatmap.Value);
            }
        }
        
        public override void OnEntering(IScreen last)
        {
            base.OnEntering(last);

            this.FadeInFromZero(250);
        }
        
        public override void OnResuming(IScreen last)
        {
            base.OnResuming(last);

            Carousel.AllowSelection = true;

            Beatmap.Value.Track.Looping = true;

            if (Beatmap != null && !Beatmap.Value.BeatmapSetInfo.DeletePending)
            {
                updateComponentFromBeatmap(Beatmap.Value);

                // restart playback on returning to song select, regardless.
                music?.Play();
            }

            this.FadeIn(250);
            this.ScaleTo(1, 250, Easing.OutSine);
        }

        public override void OnSuspending(IScreen next)
        {
            if (Beatmap.Value.Track != null)
                Beatmap.Value.Track.Looping = false;

            this.ScaleTo(1.1f, 250, Easing.InSine);
            this.FadeOut(250);

            base.OnSuspending(next);
        }

        public override bool OnExiting(IScreen next)
        {
            if (base.OnExiting(next))
                return true;

            //beatmapDetail.Hide();

            this.FadeOut(100);

            if (Beatmap.Value.Track != null)
                Beatmap.Value.Track.Looping = false;

            return false;
        }

        /// <summary>
        /// Allow components in SongSelect to update their loaded beatmap details.
        /// This is a debounced call (unlike directly binding to WorkingBeatmap.ValueChanged).
        /// </summary>
        /// <param name="beatmap">The working beatmap.</param>
        private void updateComponentFromBeatmap(WorkingBeatmap beatmap)
        {
            if (Background is BeatmapBackgroundScreen backgroundModeBeatmap)
            {
                backgroundModeBeatmap.Beatmap = beatmap;
                backgroundModeBeatmap.FadeColour(Color4.White, 250);
            }

            beatmapDetail.Beatmap = beatmap;

            if (beatmap.Track != null)
                beatmap.Track.Looping = true;
        }

        private readonly WeakReference<Track> lastTrack = new WeakReference<Track>(null);

        /// <summary>
        /// Ensures some music is playing for the current track.
        /// Will resume playback from a manual user pause if the track has changed.
        /// </summary>
        private void ensurePlayingSelected()
        {
            Track track = Beatmap.Value.Track;

            bool isNewTrack = !lastTrack.TryGetTarget(out var last) || last != track;

            track.RestartPoint = Beatmap.Value.Metadata.PreviewTime;

            if (!track.IsRunning && (music?.IsUserPaused != true || isNewTrack))
                music?.Play(true);

            lastTrack.SetTarget(track);
        }

        private void carouselBeatmapsLoaded()
        {
            bindBindables();

            Carousel.AllowSelection = true;

            // If a selection was already obtained, do not attempt to update the selected beatmap.
            if (Carousel.SelectedBeatmapSet != null)
                return;

            // Attempt to select the current beatmap on the carousel, if it is valid to be selected.
            if (!Beatmap.IsDefault && Beatmap.Value.BeatmapSetInfo?.DeletePending == false)
            {
                if (Carousel.SelectBeatmap(Beatmap.Value.BeatmapInfo, false))
                    return;

                // prefer not changing ruleset at this point, so look for another difficulty in the currently playing beatmap
                var found = Beatmap.Value.BeatmapSetInfo.Beatmaps.FirstOrDefault();

                if (found != null && Carousel.SelectBeatmap(found, false))
                    return;
            }
        }

        private bool boundLocalBindables;

        private void bindBindables()
        {
            if (boundLocalBindables)
                return;
            
            Beatmap.BindValueChanged(workingBeatmapChanged);

            boundLocalBindables = true;
        }
        
        private class VerticalMaskingContainer : Container
        {
            private const float panel_overflow = 1.2f;

            protected override Container<Drawable> Content { get; }

            public VerticalMaskingContainer()
            {
                RelativeSizeAxes = Axes.Both;
                Masking = true;
                Anchor = Anchor.Centre;
                Origin = Anchor.Centre;
                Width = panel_overflow;
                InternalChild = Content = new Container
                {
                    RelativeSizeAxes = Axes.Both,
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre,
                    Width = 1 / panel_overflow,
                };
            }
        }
    }
}
