using System;
using System.Collections.Generic;
using osu.Framework.Allocation;
using osu.Framework.Audio;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Timing;
using osuTK;
using osuTK.Graphics;
using Tachyon.Game.Beatmaps;
using Tachyon.Game.Graphics.UserInterface;
using Tachyon.Game.Rulesets;
using Tachyon.Game.Screens.Generate;
using Tachyon.Game.Screens.Generate.Components;

namespace Tachyon.Game.Tests.Visual.Generate
{
    public abstract class TimelineTestScene : EditorClockTestScene
    {
        public override IReadOnlyList<Type> RequiredTypes => new[]
        {
            typeof(Timeline),
            typeof(TimelineArea),
            typeof(TimelineButton),
        };
        
        protected TimelineArea TimelineArea { get; private set; }

        [BackgroundDependencyLoader]
        private void load(AudioManager audio)
        {
            Beatmap.Value = new WaveformTestBeatmap(audio);

            var playable = Beatmap.Value.GetPlayableBeatmap(new TachyonRuleset().RulesetInfo);

            var editorBeatmap = new EditorBeatmap(playable);

            Dependencies.Cache(editorBeatmap);
            Dependencies.CacheAs<IBeatSnapProvider>(editorBeatmap);

            AddRange(new Drawable[]
            {
                editorBeatmap,
                new FillFlowContainer
                {
                    AutoSizeAxes = Axes.Both,
                    Direction = FillDirection.Vertical,
                    Spacing = new Vector2(0, 5),
                    Children = new Drawable[]
                    {
                        new StartStopButton(),
                        new AudioVisualiser(),
                    }
                },
                TimelineArea = new TimelineArea
                {
                    Child = CreateTestComponent(),
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre,
                    RelativeSizeAxes = Axes.X,
                    Size = new Vector2(0.8f, 100),
                }
            });
        }

        public abstract Drawable CreateTestComponent();

        private class AudioVisualiser : CompositeDrawable
        {
            private readonly Drawable marker;

            [Resolved]
            private IBindable<WorkingBeatmap> beatmap { get; set; }

            [Resolved]
            private IAdjustableClock adjustableClock { get; set; }

            public AudioVisualiser()
            {
                Size = new Vector2(250, 25);

                InternalChildren = new[]
                {
                    new Box
                    {
                        RelativeSizeAxes = Axes.Both,
                        Alpha = 0.25f,
                    },
                    marker = new Box
                    {
                        RelativePositionAxes = Axes.X,
                        RelativeSizeAxes = Axes.Y,
                        Width = 2,
                    }
                };
            }

            protected override void Update()
            {
                base.Update();

                if (beatmap.Value.Track.IsLoaded)
                    marker.X = (float)(adjustableClock.CurrentTime / beatmap.Value.Track.Length);
            }
        }

        private class StartStopButton : TachyonButton
        {
            private IAdjustableClock adjustableClock;
            private bool started;

            public StartStopButton()
            {
                BackgroundColour = Color4.SlateGray;
                Size = new Vector2(100, 50);
                Text = "Start";

                Action = onClick;
            }

            [BackgroundDependencyLoader]
            private void load(IAdjustableClock adjustableClock)
            {
                this.adjustableClock = adjustableClock;
            }

            private void onClick()
            {
                if (started)
                {
                    adjustableClock.Stop();
                    Text = "Start";
                }
                else
                {
                    adjustableClock.Start();
                    Text = "Stop";
                }

                started = !started;
            }
        }
    }
}