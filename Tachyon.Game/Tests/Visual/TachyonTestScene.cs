using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using osu.Framework.Allocation;
using osu.Framework.Audio;
using osu.Framework.Audio.Track;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Platform;
using osu.Framework.Testing;
using osu.Framework.Timing;
using Tachyon.Game.Beatmaps;
using Tachyon.Game.Database;
using Tachyon.Game.Screens;
using Tachyon.Game.Tests.Beatmaps;

namespace Tachyon.Game.Tests.Visual
{
    public abstract class TachyonTestScene : TestScene
    {
        protected Bindable<WorkingBeatmap> Beatmap { get; private set; }

        protected override Container<Drawable> Content => content ?? base.Content;
        
        protected new TachyonScreenDependencies Dependencies { get; private set; }
        protected DatabaseContextFactory ContextFactory => contextFactory.Value;
        
        private Lazy<Storage> localStorage;
        protected Storage LocalStorage => localStorage.Value;
        private readonly Lazy<DatabaseContextFactory> contextFactory;
        private readonly Container content;
        
        [Resolved]
        protected AudioManager Audio { get; private set; }
        
        protected TachyonTestScene()
        {
            RecycleLocalStorage();
            contextFactory = new Lazy<DatabaseContextFactory>(() =>
            {
                var factory = new DatabaseContextFactory(LocalStorage);
                factory.ResetDatabase();
                using (var usage = factory.Get())
                    usage.Migrate();
                return factory;
            });

            base.Content.Add(content = new DrawSizePreservingFillContainer());
        }
        
        protected override IReadOnlyDependencyContainer CreateChildDependencies(IReadOnlyDependencyContainer parent)
        {
            Dependencies = new TachyonScreenDependencies(false, base.CreateChildDependencies(parent));

            Beatmap = Dependencies.Beatmap;
            Beatmap.SetDefault();

            return Dependencies;
        }
        
        protected override ITestSceneTestRunner CreateRunner() => new TachyonTestSceneTestRunner();

        public virtual void RecycleLocalStorage()
        {
            if (localStorage?.IsValueCreated == true)
            {
                try
                {
                    localStorage.Value.DeleteDirectory(".");
                }
                catch
                {
                    // we don't really care if this fails; it will just leave folders lying around from test runs.
                }
            }

            localStorage = new Lazy<Storage>(() => new NativeStorage($"{GetType().Name}-{Guid.NewGuid()}"));
        }
        
        private class TachyonTestSceneTestRunner : TachyonGameBase, ITestSceneTestRunner
        {
            private TestSceneTestRunner.TestRunner runner;

            protected override void LoadAsyncComplete()
            {
                Add(runner = new TestSceneTestRunner.TestRunner());
            }

            public void RunTestBlocking(TestScene test) => runner.RunTestBlocking(test);
        }
        
        protected virtual IBeatmap CreateBeatmap() => new TestBeatmap();

        protected WorkingBeatmap CreateWorkingBeatmap() =>
            CreateWorkingBeatmap(CreateBeatmap());

        protected virtual WorkingBeatmap CreateWorkingBeatmap(IBeatmap beatmap) =>
            new ClockBackedTestWorkingBeatmap(beatmap, Clock, Audio);

        protected override void Dispose(bool isDisposing)
        {
            base.Dispose(isDisposing);

            if (Beatmap?.Value.TrackLoaded == true)
                Beatmap.Value.Track.Stop();

            if (contextFactory.IsValueCreated)
                contextFactory.Value.ResetDatabase();

            RecycleLocalStorage();
        }
        
        public class ClockBackedTestWorkingBeatmap : TestWorkingBeatmap
        {
            private readonly Track track;

            private readonly TrackVirtualStore store;

            public ClockBackedTestWorkingBeatmap(IFrameBasedClock referenceClock, AudioManager audio)
                : this(new TestBeatmap(), referenceClock, audio)
            {
            }

            public ClockBackedTestWorkingBeatmap(IBeatmap beatmap, IFrameBasedClock referenceClock, AudioManager audio, double length = 60000)
                : base(beatmap)
            {
                if (referenceClock != null)
                {
                    store = new TrackVirtualStore(referenceClock);
                    audio.AddItem(store);
                    track = store.GetVirtual(length);
                }
                else
                    track = audio?.Tracks.GetVirtual(length);
            }

            ~ClockBackedTestWorkingBeatmap()
            {
                // Remove the track store from the audio manager
                store?.Dispose();
            }

            protected override Track GetTrack() => track;

            public class TrackVirtualStore : AudioCollectionManager<Track>, ITrackStore
            {
                private readonly IFrameBasedClock referenceClock;

                public TrackVirtualStore(IFrameBasedClock referenceClock)
                {
                    this.referenceClock = referenceClock;
                }

                public Track Get(string name) => throw new NotImplementedException();

                public Task<Track> GetAsync(string name) => throw new NotImplementedException();

                public Stream GetStream(string name) => throw new NotImplementedException();

                public IEnumerable<string> GetAvailableResources() => throw new NotImplementedException();

                public Track GetVirtual(double length = double.PositiveInfinity)
                {
                    var track = new TrackVirtualManual(referenceClock) { Length = length };
                    AddItem(track);
                    return track;
                }
            }

            public class TrackVirtualManual : Track
            {
                private readonly IFrameBasedClock referenceClock;

                private bool running;

                public TrackVirtualManual(IFrameBasedClock referenceClock)
                {
                    this.referenceClock = referenceClock;
                    Length = double.PositiveInfinity;
                }

                public override bool Seek(double seek)
                {
                    accumulated = Math.Clamp(seek, 0, Length);
                    lastReferenceTime = null;

                    return Math.Abs(accumulated - seek) < 0.01;
                }

                public override void Start()
                {
                    running = true;
                }

                public override void Reset()
                {
                    Seek(0);
                    base.Reset();
                }

                public override void Stop()
                {
                    if (running)
                    {
                        running = false;
                        lastReferenceTime = null;
                    }
                }

                public override bool IsRunning => running;

                private double? lastReferenceTime;

                private double accumulated;

                public override double CurrentTime => Math.Min(accumulated, Length);

                protected override void UpdateState()
                {
                    base.UpdateState();

                    if (running)
                    {
                        double refTime = referenceClock.CurrentTime;

                        if (lastReferenceTime.HasValue)
                            accumulated += (refTime - lastReferenceTime.Value) * Rate;

                        lastReferenceTime = refTime;
                    }

                    if (CurrentTime >= Length)
                    {
                        Stop();
                        RaiseCompleted();
                    }
                }
            }
        }
    }
}