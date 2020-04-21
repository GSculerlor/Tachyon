using Tachyon.Game.Rulesets.Objects.Types;

namespace Tachyon.Game.Rulesets.Converters
{
    internal sealed class ConvertSpinner : ConvertHitObject, IHasEndTime
    {
        public double EndTime { get; set; }

        public double Duration => EndTime - StartTime;
    }
}