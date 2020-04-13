using System;
using System.Collections.Generic;
using System.Linq;
using osu.Framework.Allocation;
using osu.Framework.Audio.Track;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Logging;
using osu.Framework.Screens;
using osu.Framework.Threading;
using Tachyon.Game.Beatmaps;
using Tachyon.Game.Components;

namespace Tachyon.Game.Screens.Playground
{
    public class PlaygroundScreen : TachyonScreen
    {
        [Resolved]
        private BeatmapManager beatmaps { get; set; }
        
        [Resolved(canBeNull: true)]
        private MusicController music { get; set; }
        
        protected BeatmapCarousel Carousel { get; private set; }

        [BackgroundDependencyLoader]
        private void load()
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
                                new Dimension(),
                                new Dimension(GridSizeMode.Relative, 0.5f, maxSize: 850),
                            },
                            Content = new[]
                            {
                                new Drawable[]
                                {
                                    new Container
                                    {
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
                                    }
                                },
                            }
                        },
                    }
                },
            });
        }

        private bool boundLocalBindables;
        
        private void bindBindables()
        {
            if (boundLocalBindables)
                return;

            Beatmap.BindValueChanged(workingBeatmapChanged);

            boundLocalBindables = true;
        }
        
        public override void OnEntering(IScreen last)
        {
            base.OnEntering(last);

            this.FadeInFromZero(250);
            //beatmapDetail.Show();
        }
        
        public override void OnResuming(IScreen last)
        {
            base.OnResuming(last);

            Beatmap.Value.Track.Looping = true;

            if (Beatmap != null && !Beatmap.Value.BeatmapSetInfo.DeletePending)
            {
                updateComponentFromBeatmap(Beatmap.Value);

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
        
        private void carouselBeatmapsLoaded()
        {
            bindBindables();

            Carousel.AllowSelection = true;

            if (Carousel.SelectedBeatmapSet != null)
                return;

            if (!Beatmap.IsDefault && Beatmap.Value.BeatmapSetInfo?.DeletePending == false)
            {
                if (Carousel.SelectBeatmap(Beatmap.Value.BeatmapInfo, false))
                    return;

                var found = Beatmap.Value.BeatmapSetInfo.Beatmaps.FirstOrDefault();

                if (found != null && Carousel.SelectBeatmap(found, false))
                    return;
            }
        }
        
        private ScheduledDelegate selectionChangedDebounce;
        private BeatmapInfo beatmapNoDebounce;
        
        private void updateSelectedBeatmap(BeatmapInfo beatmap)
        {
            if (beatmap?.Equals(beatmapNoDebounce) == true)
                return;

            beatmapNoDebounce = beatmap;

            performUpdateSelected();
        }
        
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
        
        private readonly WeakReference<Track> lastTrack = new WeakReference<Track>(null);
        
        private void ensurePlayingSelected()
        {
            Track track = Beatmap.Value.Track;

            bool isNewTrack = !lastTrack.TryGetTarget(out var last) || last != track;

            track.RestartPoint = Beatmap.Value.Metadata.PreviewTime;

            if (!track.IsRunning && (music?.IsPaused != true || isNewTrack))
                music?.Play(true);

            lastTrack.SetTarget(track);
        }
        
        private void updateComponentFromBeatmap(WorkingBeatmap beatmap)
        {
            //beatmapDetail.Beatmap = beatmap;

            if (beatmap.Track != null)
                beatmap.Track.Looping = true;
        }
        
        private void workingBeatmapChanged(ValueChangedEvent<WorkingBeatmap> e)
        {
            if (e.NewValue is PlaceholderWorkingBeatmap || !this.IsCurrentScreen()) return;

            Logger.Log($"working beatmap updated to {e.NewValue}");
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
