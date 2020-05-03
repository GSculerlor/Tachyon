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
using Tachyon.Game.Overlays.Toolbar;
using Tachyon.Game.Scoring;
using Tachyon.Game.Screens.Backgrounds;
using Tachyon.Game.Screens.Generate;
using Tachyon.Game.Screens.Select.Detail;

namespace Tachyon.Game.Screens.Select
{
    public abstract class SongSelect : TachyonScreen
    {
        protected override BackgroundScreen CreateBackground() => new BeatmapBackgroundScreen(Beatmap.Value);
        
        protected BeatmapCarousel Carousel { get; private set; }

        private Toolbar toolbar;
        
        private BeatmapDetail beatmapDetail;

        private HighscoreDetail highscoreDetail;
        
        [Resolved]
        private BeatmapManager beatmaps { get; set; }
        
        [Resolved]
        private ScoreManager scoreManager { get; set; }
        
        [Resolved(canBeNull: true)]
        private MusicController music { get; set; }

        [BackgroundDependencyLoader]
        private void load(AudioManager audio, TachyonColor colors)
        {
            AddRangeInternal(new Drawable[]
            {
                toolbar = new Toolbar
                {
                    State = { Value = Visibility.Visible },
                    EditorAction = gotoEditor
                },
                new VerticalMaskingContainer
                {
                    Margin = new MarginPadding { Top = Toolbar.HEIGHT * 2 },
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
                                    new FillFlowContainer
                                    {
                                        X = -5,
                                        Origin = Anchor.CentreLeft,
                                        Anchor = Anchor.CentreLeft,
                                        RelativeSizeAxes = Axes.Both,
                                        Direction = FillDirection.Vertical,
                                        Children = new Drawable[]
                                        {
                                            beatmapDetail = new BeatmapDetail
                                            {
                                                Origin = Anchor.Centre,
                                                Anchor = Anchor.Centre,
                                                Height = 180,
                                                RelativeSizeAxes = Axes.X,
                                                ClickedAction = () => FinaliseSelection()
                                            },
                                            highscoreDetail = new HighscoreDetail
                                            {
                                                Origin = Anchor.Centre,
                                                Anchor = Anchor.Centre,
                                                RelativeSizeAxes = Axes.X,
                                                AutoSizeAxes = Axes.Y,
                                            },
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

        private void gotoEditor()
        {
            this.Push(new Editor());
        }
        
        private DependencyContainer dependencies;

        protected override IReadOnlyDependencyContainer CreateChildDependencies(IReadOnlyDependencyContainer parent)
        {
            dependencies = new DependencyContainer(base.CreateChildDependencies(parent));

            dependencies.CacheAs(this);

            return dependencies;
        }
        
        public void FinaliseSelection(BeatmapInfo beatmap = null, bool performStartAction = true)
        {
            if (!Carousel.BeatmapSetsLoaded)
                return;

            if (Carousel.SelectedBeatmap == null) return;

            if (beatmap != null)
                Carousel.SelectBeatmap(beatmap);

            if (selectionChangedDebounce?.Completed == false)
            {
                selectionChangedDebounce.RunTask();
                selectionChangedDebounce.Cancel();
                selectionChangedDebounce = null;
            }

            if (performStartAction && OnStart())
                Carousel.AllowSelection = false;
        }
        
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

            if (Beatmap != null && !Beatmap.Value.BeatmapSetInfo.DeletePending && Beatmap.Value.BeatmapSetInfo?.Protected == false)
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

            beatmapDetail.Hide();
            highscoreDetail.Hide();

            this.FadeOut(100);

            if (Beatmap.Value.Track != null)
                Beatmap.Value.Track.Looping = false;

            return false;
        }

        private void updateComponentFromBeatmap(WorkingBeatmap beatmap)
        {
            if (Background is BeatmapBackgroundScreen backgroundModeBeatmap)
            {
                backgroundModeBeatmap.Beatmap = beatmap;
                backgroundModeBeatmap.FadeColour(Color4.White, 250);
            }
            
            var queriedScore = scoreManager
                               .QueryScores(s => !s.DeletePending && s.Beatmap.ID == beatmap.BeatmapInfo.ID)
                               .OrderByDescending(s => s.TotalScore)
                               .FirstOrDefault();

            beatmapDetail.Beatmap = beatmap;
            highscoreDetail.Score.Value = queriedScore;

            if (beatmap.Track != null)
                beatmap.Track.Looping = true;
        }

        private readonly WeakReference<Track> lastTrack = new WeakReference<Track>(null);

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
