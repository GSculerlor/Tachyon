using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using osu.Framework.Audio;
using osu.Framework.Audio.Track;
using osu.Framework.Graphics.Textures;
using osu.Framework.Logging;
using osu.Framework.Statistics;
using Tachyon.Game.GameModes.Objects.Types;

namespace Tachyon.Game.Beatmaps
{
    public abstract class WorkingBeatmap : IWorkingBeatmap
    {
        public readonly BeatmapInfo BeatmapInfo;

        public readonly BeatmapSetInfo BeatmapSetInfo;

        public readonly BeatmapMetadata Metadata;

        protected AudioManager AudioManager { get; }

        private static readonly GlobalStatistic<int> total_count = GlobalStatistics.Get<int>(nameof(Beatmaps), $"Total {nameof(WorkingBeatmap)}s");

        protected WorkingBeatmap(BeatmapInfo beatmapInfo, AudioManager audioManager)
        {
            AudioManager = audioManager;
            BeatmapInfo = beatmapInfo;
            BeatmapSetInfo = beatmapInfo.BeatmapSet;
            Metadata = beatmapInfo.Metadata ?? BeatmapSetInfo?.Metadata ?? new BeatmapMetadata();

            track = new RecyclableLazy<Track>(() => GetTrack() ?? GetVirtualTrack(1000));
            background = new RecyclableLazy<Texture>(GetBackground, BackgroundStillValid);
            waveform = new RecyclableLazy<Waveform>(GetWaveform);

            total_count.Value++;
        }

        protected virtual Track GetVirtualTrack(double emptyLength = 0)
        {
            const double excess_length = 1000;

            var lastObject = Beatmap.HitObjects.LastOrDefault();

            double length;

            switch (lastObject)
            {
                case null:
                    length = emptyLength;
                    break;

                case IHasEndTime endTime:
                    length = endTime.EndTime + excess_length;
                    break;

                default:
                    length = lastObject.StartTime + excess_length;
                    break;
            }

            return AudioManager.Tracks.GetVirtual(length);
        }

        protected virtual IBeatmapConverter CreateBeatmapConverter(IBeatmap beatmap) => new BeatmapConverter(beatmap);

        public IBeatmap GetPlayableBeatmap(TimeSpan? timeout = null)
        {
            using (var cancellationSource = new CancellationTokenSource(timeout ?? TimeSpan.FromSeconds(10)))
            {
                IBeatmapConverter converter = CreateBeatmapConverter(Beatmap);

                if (Beatmap.HitObjects.Count > 0)
                    throw new Exception($"{nameof(Beatmaps.Beatmap)} can not be converted, converter: {converter}).");
                    
                IBeatmap converted = converter.Convert();

                foreach (var obj in converted.HitObjects)
                {
                    if (cancellationSource.IsCancellationRequested)
                        throw new BeatmapLoadTimeoutException(BeatmapInfo);

                    obj.ApplyDefaults(converted.ControlPointInfo, converted.BeatmapInfo.BaseDifficulty);
                }

                return converted;
            }
        }

        private CancellationTokenSource loadCancellation = new CancellationTokenSource();

        public void BeginAsyncLoad()
        {
            loadBeatmapAsync();
        }

        public void CancelAsyncLoad()
        {
            loadCancellation?.Cancel();
            loadCancellation = new CancellationTokenSource();

            if (beatmapLoadTask?.IsCompleted != true)
                beatmapLoadTask = null;
        }

        private Task<IBeatmap> loadBeatmapAsync() => beatmapLoadTask ??= Task.Factory.StartNew(() =>
        {
            var b = GetBeatmap() ?? new Beatmap();

            // Use the database-backed info for more up-to-date values (beatmap id, ranked status, etc)
            b.BeatmapInfo = BeatmapInfo;

            return b;
        }, loadCancellation.Token, TaskCreationOptions.LongRunning, TaskScheduler.Default);

        public override string ToString() => BeatmapInfo.ToString();

        public bool BeatmapLoaded => beatmapLoadTask?.IsCompleted ?? false;

        public IBeatmap Beatmap
        {
            get
            {
                try
                {
                    return loadBeatmapAsync().Result;
                }
                catch (AggregateException ae)
                {
                    // This is the exception that is generally expected here, which occurs via natural cancellation of the asynchronous load
                    if (ae.InnerExceptions.FirstOrDefault() is TaskCanceledException)
                        return null;

                    Logger.Error(ae, "Beatmap failed to load");
                    return null;
                }
                catch (Exception e)
                {
                    Logger.Error(e, "Beatmap failed to load");
                    return null;
                }
            }
        }

        protected abstract IBeatmap GetBeatmap();
        private Task<IBeatmap> beatmapLoadTask;

        public bool BackgroundLoaded => background.IsResultAvailable;

        public Texture Background => background.Value;
        protected virtual bool BackgroundStillValid(Texture b) => b == null || b.Available;
        protected abstract Texture GetBackground();
        private readonly RecyclableLazy<Texture> background;

        public bool TrackLoaded => track.IsResultAvailable;
        public Track Track => track.Value;
        protected abstract Track GetTrack();
        private RecyclableLazy<Track> track;

        public bool WaveformLoaded => waveform.IsResultAvailable;
        public Waveform Waveform => waveform.Value;
        protected virtual Waveform GetWaveform() => new Waveform(null);
        private readonly RecyclableLazy<Waveform> waveform;

        public virtual void TransferTo(WorkingBeatmap other)
        {
            if (track.IsResultAvailable && Track != null && BeatmapInfo.AudioEquals(other.BeatmapInfo))
                other.track = track;
        }

        public virtual void RecycleTrack() => track.Recycle();

        ~WorkingBeatmap()
        {
            total_count.Value--;
        }

        public class RecyclableLazy<T>
        {
            private Lazy<T> lazy;
            private readonly Func<T> valueFactory;
            private readonly Func<T, bool> stillValidFunction;

            private readonly object fetchLock = new object();

            public RecyclableLazy(Func<T> valueFactory, Func<T, bool> stillValidFunction = null)
            {
                this.valueFactory = valueFactory;
                this.stillValidFunction = stillValidFunction;

                recreate();
            }

            public void Recycle()
            {
                if (!IsResultAvailable) return;

                (lazy.Value as IDisposable)?.Dispose();
                recreate();
            }

            public bool IsResultAvailable => stillValid;

            public T Value
            {
                get
                {
                    lock (fetchLock)
                    {
                        if (!stillValid)
                            recreate();
                        return lazy.Value;
                    }
                }
            }

            private bool stillValid => lazy.IsValueCreated && (stillValidFunction?.Invoke(lazy.Value) ?? true);

            private void recreate() => lazy = new Lazy<T>(valueFactory, LazyThreadSafetyMode.ExecutionAndPublication);
        }

        private class BeatmapLoadTimeoutException : TimeoutException
        {
            public BeatmapLoadTimeoutException(BeatmapInfo beatmapInfo)
                : base($"Timed out while loading beatmap ({beatmapInfo}).")
            {
            }
        }
    }
}