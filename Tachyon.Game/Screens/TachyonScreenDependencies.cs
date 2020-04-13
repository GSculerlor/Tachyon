using osu.Framework.Allocation;
using osu.Framework.Bindables;
using Tachyon.Game.Beatmaps;

namespace Tachyon.Game.Screens
{
    public class TachyonScreenDependencies : DependencyContainer
    {
        public Bindable<WorkingBeatmap> Beatmap { get; }
        
        public TachyonScreenDependencies(bool requireLease, IReadOnlyDependencyContainer parent)
            : base(parent)
        {
            if (requireLease)
            {
                Beatmap = parent.Get<LeasedBindable<WorkingBeatmap>>()?.GetBoundCopy();

                if (Beatmap == null)
                {
                    Cache(Beatmap = parent.Get<Bindable<WorkingBeatmap>>().BeginLease(false));
                    CacheAs(Beatmap);
                }
            }
            else
            {
                Beatmap = (parent.Get<LeasedBindable<WorkingBeatmap>>() ?? parent.Get<Bindable<WorkingBeatmap>>()).GetBoundCopy();
            }
        }
    }
}
