using System.Collections.Generic;
using Tachyon.Game.Beatmaps.ControlPoints;
using Tachyon.Game.Rulesets.Objects;
using Tachyon.Game.IO.Serialization;

namespace Tachyon.Game.Beatmaps
{
    public interface IBeatmap : IJsonSerializable
    {
        BeatmapInfo BeatmapInfo { get; set; }
        
        BeatmapMetadata Metadata { get; }
        
        ControlPointInfo ControlPointInfo { get; }

        IReadOnlyList<HitObject> HitObjects { get; }
        
        IBeatmap Clone();
    }

    public interface IBeatmap<out T> : IBeatmap
        where T : HitObject
    {
        /// <summary>
        /// The hitobjects contained by this beatmap.
        /// </summary>
        new IReadOnlyList<T> HitObjects { get; }
    }
}