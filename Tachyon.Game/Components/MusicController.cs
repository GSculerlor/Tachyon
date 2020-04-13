using System;
using System.Linq;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using Tachyon.Game.Beatmaps;

namespace Tachyon.Game.Components
{
    public class MusicController : Component
    {
        [Resolved]
        private BeatmapManager beatmaps { get; set; }
        
        [Resolved]
        private IBindable<WorkingBeatmap> beatmap { get; set; }
        
        private readonly BindableList<BeatmapSetInfo> beatmapSets = new BindableList<BeatmapSetInfo>();
        
        public event Action<WorkingBeatmap, TrackChangeDirection> TrackChanged;
        
        public bool IsPaused { get; private set; }
        
        private WorkingBeatmap currentBeatmap;
        private TrackChangeDirection? queuedDirection;

        [BackgroundDependencyLoader]
        private void load()
        {
            beatmaps.ItemAdded += handleBeatmapAdded;
            beatmaps.ItemRemoved += handleBeatmapRemoved;
            
            beatmapSets.AddRange(beatmaps.GetAllUsableBeatmapSets());
        }

        protected override void LoadComplete()
        {
            base.LoadComplete();

            beatmap.BindValueChanged(beatmapChanged, true);
        }
        
        protected override void Dispose(bool isDisposing)
        {
            base.Dispose(isDisposing);

            if (beatmaps != null)
            {
                beatmaps.ItemAdded -= handleBeatmapAdded;
                beatmaps.ItemRemoved -= handleBeatmapRemoved;
            }
        }
        
        public IBindableList<BeatmapSetInfo> BeatmapSets
        {
            get
            {
                if (LoadState < LoadState.Ready)
                    throw new InvalidOperationException($"{nameof(BeatmapSets)} should not be accessed before the music controller is loaded.");

                return beatmapSets;
            }
        }

        private void handleBeatmapAdded(BeatmapSetInfo set) => Schedule(() =>
        {
            if (!beatmapSets.Contains(set))
                beatmapSets.Add(set);
        });

        private void handleBeatmapRemoved(BeatmapSetInfo set) => Schedule(() =>
        {
            beatmapSets.RemoveAll(s => s.ID == set.ID);
        });
        
        private void beatmapChanged(ValueChangedEvent<WorkingBeatmap> beatmap)
        {
            TrackChangeDirection direction = TrackChangeDirection.None;

            if (currentBeatmap != null)
            {
                bool audioEquals = beatmap.NewValue?.BeatmapInfo?.AudioEquals(currentBeatmap.BeatmapInfo) ?? false;

                if (audioEquals)
                    direction = TrackChangeDirection.None;
                else if (queuedDirection.HasValue)
                {
                    direction = queuedDirection.Value;
                    queuedDirection = null;
                }
                else
                {
                    //figure out the best direction based on order in playlist.
                    var last = BeatmapSets.TakeWhile(b => b.ID != currentBeatmap.BeatmapSetInfo?.ID).Count();
                    var next = beatmap.NewValue == null ? -1 : BeatmapSets.TakeWhile(b => b.ID != beatmap.NewValue.BeatmapSetInfo?.ID).Count();

                    direction = last > next ? TrackChangeDirection.Prev : TrackChangeDirection.Next;
                }
            }

            currentBeatmap = beatmap.NewValue;
            TrackChanged?.Invoke(currentBeatmap, direction);
            queuedDirection = null;
        }
        
        #region Controller
        
        public bool IsPlaying => currentBeatmap?.Track.IsRunning ?? false;
        
        public bool Play(bool restart = false)
        {
            var track = currentBeatmap?.Track;

            IsPaused = false;

            if (track == null)
            {
                if (beatmap.Disabled)
                    return false;

                next(true);
                return true;
            }

            if (restart)
                track.Restart();
            else if (!IsPlaying)
                track.Start();

            return true;
        }
        
        public void Stop()
        {
            var track = currentBeatmap?.Track;

            IsPaused = true;
            if (track?.IsRunning == true)
                track.Stop();
        }
        
        public bool TogglePause()
        {
            var track = currentBeatmap?.Track;

            if (track?.IsRunning == true)
                Stop();
            else
                Play();

            return true;
        }
        
        public bool NextTrack() => next();

        private bool next(bool instant = false)
        {
            if (!instant)
                queuedDirection = TrackChangeDirection.Next;

            var playable = BeatmapSets.SkipWhile(i => i.ID != currentBeatmap.BeatmapSetInfo.ID).ElementAtOrDefault(1) ?? BeatmapSets.FirstOrDefault();

            if (playable != null)
            {
                if (beatmap is Bindable<WorkingBeatmap> working)
                    working.Value = beatmaps.GetWorkingBeatmap(playable.Beatmaps.First(), beatmap.Value);
                beatmap.Value.Track.Restart();
                return true;
            }

            return false;
        }
        
        public bool PrevTrack()
        {
            queuedDirection = TrackChangeDirection.Prev;

            var playable = BeatmapSets.TakeWhile(i => i.ID != currentBeatmap.BeatmapSetInfo.ID).LastOrDefault() ?? BeatmapSets.LastOrDefault();

            if (playable != null)
            {
                if (beatmap is Bindable<WorkingBeatmap> working)
                    working.Value = beatmaps.GetWorkingBeatmap(playable.Beatmaps.First(), beatmap.Value);
                beatmap.Value.Track.Restart();

                return true;
            }

            return false;
        }
        
        #endregion
        
        public enum TrackChangeDirection
        {
            None,
            Next,
            Prev
        }
    }
}