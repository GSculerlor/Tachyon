using Tachyon.Game.GameModes.Objects.Types;

namespace Tachyon.Game.GameModes.Objects.Converters
{
    internal sealed class ConvertSpinner : ConvertHitObject, IHasEndTime
    {
        public double EndTime { get; set; }

        public double Duration => EndTime - StartTime;
    }
}