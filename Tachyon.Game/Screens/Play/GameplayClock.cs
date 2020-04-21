using osu.Framework.Bindables;
using osu.Framework.Timing;

namespace Tachyon.Game.Screens.Play
{
    public class GameplayClock : IFrameBasedClock
    {
        private readonly IFrameBasedClock underlyingClock;

        public readonly BindableBool IsPaused = new BindableBool();

        public GameplayClock(IFrameBasedClock underlyingClock)
        {
            this.underlyingClock = underlyingClock;
        }

        public double CurrentTime => underlyingClock.CurrentTime;

        public double Rate => underlyingClock.Rate;

        public bool IsRunning => underlyingClock.IsRunning;

        public void ProcessFrame()
        {
            // we do not want to process the underlying clock.
        }

        public double ElapsedFrameTime => underlyingClock.ElapsedFrameTime;

        public double FramesPerSecond => underlyingClock.FramesPerSecond;

        public FrameTimeInfo TimeInfo => underlyingClock.TimeInfo;

        public IClock Source => underlyingClock;
    }
}
