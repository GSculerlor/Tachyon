using osu.Framework.Bindables;
using Tachyon.Game.Rulesets.UI.Scrolling.Algorithms;

namespace Tachyon.Game.Rulesets.UI.Scrolling
{
    public interface IScrollingInfo
    {
        IBindable<double> TimeRange { get; }

        IScrollAlgorithm Algorithm { get; }
    }
}
