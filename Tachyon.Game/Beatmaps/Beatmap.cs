using System.Collections.Generic;
using Newtonsoft.Json;
using Tachyon.Game.Beatmaps.ControlPoints;
using Tachyon.Game.GameModes.Objects;
using Tachyon.Game.IO.Serialization.Converters;

namespace Tachyon.Game.Beatmaps
{
    public class Beatmap : IBeatmap<HitObject>
    {
        public BeatmapInfo BeatmapInfo { get; set; } = new BeatmapInfo
        {
            Metadata = new BeatmapMetadata
            {
                Artist = @"Unknown",
                Title = @"Unknown",
            },
            Version = @"Normal",
            BaseDifficulty = new BeatmapDifficulty()
        };

        [JsonIgnore]
        public BeatmapMetadata Metadata => BeatmapInfo?.Metadata ?? BeatmapInfo?.BeatmapSet?.Metadata;

        public ControlPointInfo ControlPointInfo { get; set; } = new ControlPointInfo();

        [JsonConverter(typeof(TypedListConverter<HitObject>))]
        public List<HitObject> HitObjects { get; set; } = new List<HitObject>();

        IReadOnlyList<HitObject> IBeatmap<HitObject>.HitObjects => HitObjects;

        IReadOnlyList<HitObject> IBeatmap.HitObjects => HitObjects;
        
        IBeatmap IBeatmap.Clone() => Clone();

        public Beatmap Clone() => (Beatmap)MemberwiseClone();
        
        public override string ToString() => BeatmapInfo?.ToString() ?? base.ToString();
    }
}