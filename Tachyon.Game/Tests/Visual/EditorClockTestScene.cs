using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Input.Events;
using osu.Framework.Timing;
using Tachyon.Game.Beatmaps;
using Tachyon.Game.Beatmaps.ControlPoints;
using Tachyon.Game.Screens.Generate;

namespace Tachyon.Game.Tests.Visual
{
    public abstract class EditorClockTestScene : TachyonTestScene
    {
        protected readonly BindableBeatDivisor BeatDivisor = new BindableBeatDivisor();
        protected new readonly EditorClock Clock;

        protected EditorClockTestScene()
        {
            Clock = new EditorClock(new ControlPointInfo(), 5000, BeatDivisor) { IsCoupled = false };
        }

        protected override IReadOnlyDependencyContainer CreateChildDependencies(IReadOnlyDependencyContainer parent)
        {
            var dependencies = new DependencyContainer(base.CreateChildDependencies(parent));

            dependencies.Cache(BeatDivisor);
            dependencies.CacheAs<IFrameBasedClock>(Clock);
            dependencies.CacheAs<IAdjustableClock>(Clock);

            return dependencies;
        }

        [BackgroundDependencyLoader]
        private void load()
        {
            Beatmap.BindValueChanged(beatmapChanged, true);
        }

        private void beatmapChanged(ValueChangedEvent<WorkingBeatmap> e)
        {
            Clock.ControlPointInfo = e.NewValue.Beatmap.ControlPointInfo;
            Clock.ChangeSource((IAdjustableClock)e.NewValue.Track ?? new StopwatchClock());
            Clock.ProcessFrame();
        }

        protected override void Update()
        {
            base.Update();

            Clock.ProcessFrame();
        }

        protected override bool OnScroll(ScrollEvent e)
        {
            if (e.ScrollDelta.Y > 0)
                Clock.SeekBackward(true);
            else
                Clock.SeekForward(true);

            return true;
        }
    }
}