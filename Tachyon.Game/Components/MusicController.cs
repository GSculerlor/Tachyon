using System;
using System.Linq;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Threading;
using Tachyon.Game.Beatmaps;

namespace Tachyon.Game.Components
{
    public class MusicController : Component
    {
        [Resolved]
        private BeatmapManager beatmaps { get; set; }

        public IBindableList<BeatmapSetInfo> BeatmapSets
        {
            get
            {
                if (LoadState < LoadState.Ready)
                    throw new InvalidOperationException($"{nameof(BeatmapSets)} should not be accessed before the music controller is loaded.");

                return beatmapSets;
            }
        }

        private readonly BindableList<BeatmapSetInfo> beatmapSets = new BindableList<BeatmapSetInfo>();

        public bool IsUserPaused { get; private set; }

        /// <summary>
        /// Fired when the global <see cref="WorkingBeatmap"/> has changed.
        /// Includes direction information for display purposes.
        /// </summary>
        public event Action<WorkingBeatmap, TrackChangeDirection> TrackChanged;

        [Resolved]
        private IBindable<WorkingBeatmap> beatmap { get; set; }

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

        /// <summary>
        /// Change the position of a <see cref="BeatmapSetInfo"/> in the current playlist.
        /// </summary>
        /// <param name="beatmapSetInfo">The beatmap to move.</param>
        /// <param name="index">The new position.</param>
        public void ChangeBeatmapSetPosition(BeatmapSetInfo beatmapSetInfo, int index)
        {
            beatmapSets.Remove(beatmapSetInfo);
            beatmapSets.Insert(index, beatmapSetInfo);
        }

        /// <summary>
        /// Returns whether the current beatmap track is playing.
        /// </summary>
        public bool IsPlaying => current?.Track.IsRunning ?? false;

        private void handleBeatmapAdded(BeatmapSetInfo set) => Schedule(() =>
        {
            if (!beatmapSets.Contains(set))
                beatmapSets.Add(set);
        });

        private void handleBeatmapRemoved(BeatmapSetInfo set) => Schedule(() =>
        {
            beatmapSets.RemoveAll(s => s.ID == set.ID);
        });

        private ScheduledDelegate seekDelegate;

        public void SeekTo(double position)
        {
            seekDelegate?.Cancel();
            seekDelegate = Schedule(() =>
            {
                if (!beatmap.Disabled)
                    current?.Track.Seek(position);
            });
        }

        /// <summary>
        /// Start playing the current track (if not already playing).
        /// </summary>
        /// <returns>Whether the operation was successful.</returns>
        public bool Play(bool restart = false)
        {
            var track = current?.Track;

            IsUserPaused = false;

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

        /// <summary>
        /// Stop playing the current track and pause at the current position.
        /// </summary>
        public void Stop()
        {
            var track = current?.Track;

            IsUserPaused = true;
            if (track?.IsRunning == true)
                track.Stop();
        }

        /// <summary>
        /// Toggle pause / play.
        /// </summary>
        /// <returns>Whether the operation was successful.</returns>
        public bool TogglePause()
        {
            var track = current?.Track;

            if (track?.IsRunning == true)
                Stop();
            else
                Play();

            return true;
        }

        /// <summary>
        /// Play the previous track.
        /// </summary>
        /// <returns>Whether the operation was successful.</returns>
        public bool PreviousTrack()
        {
            queuedDirection = TrackChangeDirection.Prev;

            var playable = BeatmapSets.TakeWhile(i => i.ID != current.BeatmapSetInfo.ID).LastOrDefault() ?? BeatmapSets.LastOrDefault();

            if (playable != null)
            {
                if (beatmap is Bindable<WorkingBeatmap> working)
                    working.Value = beatmaps.GetWorkingBeatmap(playable.Beatmaps.First(), beatmap.Value);
                beatmap.Value.Track.Restart();

                return true;
            }

            return false;
        }

        /// <summary>
        /// Play the next random or playlist track.
        /// </summary>
        /// <returns>Whether the operation was successful.</returns>
        public bool NextTrack() => next();

        private bool next(bool instant = false)
        {
            if (!instant)
                queuedDirection = TrackChangeDirection.Next;

            var playable = BeatmapSets.SkipWhile(i => i.ID != current.BeatmapSetInfo.ID).ElementAtOrDefault(1) ?? BeatmapSets.FirstOrDefault();

            if (playable != null)
            {
                if (beatmap is Bindable<WorkingBeatmap> working)
                    working.Value = beatmaps.GetWorkingBeatmap(playable.Beatmaps.First(), beatmap.Value);
                beatmap.Value.Track.Restart();
                return true;
            }

            return false;
        }

        private WorkingBeatmap current;

        private TrackChangeDirection? queuedDirection;

        private void beatmapChanged(ValueChangedEvent<WorkingBeatmap> beatmap)
        {
            TrackChangeDirection direction = TrackChangeDirection.None;

            if (current != null)
            {
                bool audioEquals = beatmap.NewValue?.BeatmapInfo?.AudioEquals(current.BeatmapInfo) ?? false;

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
                    var last = BeatmapSets.TakeWhile(b => b.ID != current.BeatmapSetInfo?.ID).Count();
                    var next = beatmap.NewValue == null ? -1 : BeatmapSets.TakeWhile(b => b.ID != beatmap.NewValue.BeatmapSetInfo?.ID).Count();

                    direction = last > next ? TrackChangeDirection.Prev : TrackChangeDirection.Next;
                }
            }

            current = beatmap.NewValue;
            TrackChanged?.Invoke(current, direction);

            ResetTrackAdjustments();

            queuedDirection = null;
        }

        private bool allowRateAdjustments;

        /// <summary>
        /// Whether mod rate adjustments are allowed to be applied.
        /// </summary>
        public bool AllowRateAdjustments
        {
            get => allowRateAdjustments;
            set
            {
                if (allowRateAdjustments == value)
                    return;

                allowRateAdjustments = value;
                ResetTrackAdjustments();
            }
        }

        public void ResetTrackAdjustments()
        {
            var track = current?.Track;
            if (track == null)
                return;

            track.ResetSpeedAdjustments();
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
    }
    
    public enum TrackChangeDirection
    {
        None,
        Next,
        Prev
    }
}