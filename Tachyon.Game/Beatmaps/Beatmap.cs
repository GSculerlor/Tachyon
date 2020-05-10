using System.Collections.Generic;
using Newtonsoft.Json;
using Tachyon.Game.Beatmaps.ControlPoints;
using Tachyon.Game.IO.Serialization.Converters;
using Tachyon.Game.Rulesets.Objects;

namespace Tachyon.Game.Beatmaps
{
    public class Beatmap<T> : IBeatmap<T>
        where T : HitObject
    {
        public BeatmapInfo BeatmapInfo { get; set; } = new BeatmapInfo
        {
            Metadata = new BeatmapMetadata
            {
                Artist = @"Unknown",
                Title = @"Unknown",
                Author = @"Unknown"
            },
            Version = @"Normal",
            BaseDifficulty = new BeatmapDifficulty()
        };

        [JsonIgnore]
        public BeatmapMetadata Metadata => BeatmapInfo?.Metadata ?? BeatmapInfo?.BeatmapSet?.Metadata;

        public ControlPointInfo ControlPointInfo { get; set; } = new ControlPointInfo();

        [JsonConverter(typeof(TypedListConverter<HitObject>))]
        public List<T> HitObjects { get; set; } = new List<T>();

        IReadOnlyList<T> IBeatmap<T>.HitObjects => HitObjects;

        IReadOnlyList<HitObject> IBeatmap.HitObjects => HitObjects;
        
        IBeatmap IBeatmap.Clone() => Clone();

        public Beatmap<T> Clone() => (Beatmap<T>)MemberwiseClone();
    }
    
    public class Beatmap : Beatmap<HitObject>
    {
        public new Beatmap Clone() => (Beatmap)base.Clone();

        public override string ToString() => BeatmapInfo?.ToString() ?? base.ToString();
    }
}