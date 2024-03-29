using System;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Extensions.Color4Extensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Cursor;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Input.Bindings;
using osu.Framework.Input.Events;
using osu.Framework.Logging;
using osu.Framework.Platform;
using osu.Framework.Screens;
using osu.Framework.Timing;
using osuTK.Graphics;
using osuTK.Input;
using Tachyon.Game.Beatmaps;
using Tachyon.Game.Configuration;
using Tachyon.Game.Generator;
using Tachyon.Game.Graphics;
using Tachyon.Game.Graphics.UserInterface;
using Tachyon.Game.Input;
using Tachyon.Game.Rulesets;
using Tachyon.Game.Rulesets.Objects;
using Tachyon.Game.Screens.Generate.Components;

namespace Tachyon.Game.Screens.Generate
{
    [Cached(typeof(IBeatSnapProvider))]
    public class Editor : DimmedBackgroundScreen, IKeyBindingHandler<GlobalAction>, IBeatSnapProvider
    {
        private const float vertical_margins = 10;
        private const float horizontal_margins = 20;

        private const float timeline_height = 110;
        
        public override bool AllowBackButton => true;

        public override bool DisallowExternalBeatmapChanges => true;

        [Resolved] 
        private BeatmapManager beatmapManager { get; set; }
        
        [Cached]
        private Bindable<HitObject> selectedHitObject = new Bindable<HitObject>();

        private readonly BindableBeatDivisor beatDivisor = new BindableBeatDivisor();
        private EditorClock clock;
        
        private IBeatmap playableBeatmap;
        private EditorBeatmap editorBeatmap;
        
        private DependencyContainer dependencies;
        private Container timelineContainer;
        
        protected override IReadOnlyDependencyContainer CreateChildDependencies(IReadOnlyDependencyContainer parent)
            => dependencies = new DependencyContainer(base.CreateChildDependencies(parent));

        [BackgroundDependencyLoader]
        private void load(TachyonColor colors, GameHost host, TachyonRuleset ruleset, TachyonConfigManager config)
        {
            beatDivisor.Value = (int) config.GetBindable<DivisorValue>(TachyonSetting.DivisorValue).Value;

            var sourceClock = (IAdjustableClock)Beatmap.Value.Track ?? new StopwatchClock();
            clock = new EditorClock(Beatmap.Value, beatDivisor) { IsCoupled = false };
            clock.ChangeSource(sourceClock);

            dependencies.CacheAs<IFrameBasedClock>(clock);
            dependencies.CacheAs<IAdjustableClock>(clock);

            dependencies.Cache(beatDivisor);
            
            try
            {
                playableBeatmap = Beatmap.Value.GetPlayableBeatmap(ruleset.RulesetInfo);
            }
            catch (Exception e)
            {
                Logger.Error(e, "Could not load beatmap successfully!");
                this.Exit();
            }
            
            AddInternal(editorBeatmap = new EditorBeatmap(playableBeatmap));
            dependencies.CacheAs(editorBeatmap);
            
            AddInternal(new BasicContextMenuContainer
            {
                RelativeSizeAxes = Axes.Both,
                Children = new Drawable[]
                {
                    new Container
                    {
                        Name = "Screen container",
                        RelativeSizeAxes = Axes.Both,
                        Padding = new MarginPadding { Top = 110, Bottom = 60 },
                        Child = new Container
                        {
                            Padding = new MarginPadding(10),
                            RelativeSizeAxes = Axes.Both,
                            Masking = true,
                            Child = new TimingScreen(),
                        }
                    },
                    new Container
                    {
                        Name = "Timeline",
                        RelativeSizeAxes = Axes.X,
                        Height = timeline_height,
                        Children = new Drawable[]
                        {
                            new Box
                            {
                                RelativeSizeAxes = Axes.Both,
                                Colour = Color4.Black.Opacity(0.5f)
                            },
                            new Container
                            {
                                Name = "Timeline content",
                                RelativeSizeAxes = Axes.Both,
                                Padding = new MarginPadding { Horizontal = horizontal_margins, Vertical = vertical_margins },
                                Child = new GridContainer
                                {
                                    RelativeSizeAxes = Axes.Both,
                                    Content = new[]
                                    {
                                        new Drawable[]
                                        {
                                            timelineContainer = new Container
                                            {
                                                RelativeSizeAxes = Axes.Both,
                                                Padding = new MarginPadding { Right = 5 },
                                            },
                                        },
                                    },
                                    ColumnDimensions = new[]
                                    {
                                        new Dimension(),
                                    }
                                },
                            }
                        }
                    },
                    new Container
                    {
                        Name = "Bottom bar",
                        Anchor = Anchor.BottomLeft,
                        Origin = Anchor.BottomLeft,
                        RelativeSizeAxes = Axes.X,
                        Height = 60,
                        Children = new Drawable[]
                        {
                            new Box
                            {
                                RelativeSizeAxes = Axes.Both,
                                Colour = colors.Gray3
                            },
                            new Container
                            {
                                RelativeSizeAxes = Axes.Both,
                                Padding = new MarginPadding { Vertical = 5, Horizontal = 10 },
                                Child = new SummaryTimeline
                                {
                                    Padding = new MarginPadding { Left = HoverableBackButton.SIZE_RETRACTED.X },
                                    RelativeSizeAxes = Axes.Both,
                                },
                            }
                        }
                    },
                }
            });
            
            LoadComponentAsync(new TimelineArea
            {
                RelativeSizeAxes = Axes.Both,
                Children = new[]
                {
                    new TimelineTickDisplay(),
                }
            }, timelineContainer.Add);
        }
        
        protected override void Update()
        {
            base.Update();
            clock.ProcessFrame();
        }
        
        protected override void LoadComplete()
        {
            base.LoadComplete();
            
            selectedHitObject.BindValueChanged(selected =>
            {
                if (selected.NewValue != null)
                    clock.Seek(selected.NewValue.StartTime);
            });
        }

        private double scrollAccumulation;

        protected override bool OnScroll(ScrollEvent e)
        {
            scrollAccumulation += (e.ScrollDelta.X + e.ScrollDelta.Y) * (e.IsPrecise ? 0.1 : 1);

            const int precision = 1;

            while (Math.Abs(scrollAccumulation) > precision)
            {
                if (scrollAccumulation > 0)
                    seek(e, -1);
                else
                    seek(e, 1);

                scrollAccumulation = scrollAccumulation < 0 ? Math.Min(0, scrollAccumulation + precision) : Math.Max(0, scrollAccumulation - precision);
            }

            return true;
        }

        protected override bool OnKeyDown(KeyDownEvent e)
        {
            switch (e.Key)
            {
                case Key.Space:
                    togglePause();
                    return true;
            }

            return base.OnKeyDown(e);
        }

        public bool OnPressed(GlobalAction action)
        {
            return false;
        }

        public void OnReleased(GlobalAction action)
        {
        }
        
        public override void OnEntering(IScreen last)
        {
            base.OnEntering(last);

            resetTrack(true);
        }

        public override bool OnExiting(IScreen next)
        {
            Background.FadeColour(Color4.White, 500);
            resetTrack();

            return base.OnExiting(next);
        }
        
        private void resetTrack(bool seekToStart = false)
        {
            Beatmap.Value.Track?.Stop();

            if (seekToStart)
            {
                double targetTime = 0;

                if (Beatmap.Value.Beatmap.HitObjects.Count > 0)
                {
                    // seek to one beat length before the first hitobject
                    targetTime = Beatmap.Value.Beatmap.HitObjects[0].StartTime;
                    targetTime -= Beatmap.Value.Beatmap.ControlPointInfo.TimingPointAt(targetTime).BeatLength;
                }

                clock.Seek(Math.Max(0, targetTime));
            }
        }
        
        private void seek(UIEvent e, int direction)
        {
            double amount = e.ShiftPressed ? 2 : 1;

            if (direction < 1)
                clock.SeekBackward(!clock.IsRunning, amount);
            else
                clock.SeekForward(!clock.IsRunning, amount);
        }

        private void togglePause()
        {
            if (clock.IsRunning)
                clock.Stop();
            else
                clock.Start();
        }

        public double SnapTime(double time, double? referenceTime) => editorBeatmap.SnapTime(time, referenceTime);

        public double GetBeatLengthAtTime(double referenceTime) => editorBeatmap.GetBeatLengthAtTime(referenceTime);

        public int BeatDivisor => beatDivisor.Value;
    }
}