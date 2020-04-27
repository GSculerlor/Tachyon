using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Tachyon.Game.Beatmaps;
using Tachyon.Game.Database;
using Tachyon.Game.Rulesets.Scoring;
using Tachyon.Game.Utils;

namespace Tachyon.Game.Scoring
{
    public class ScoreInfo : IHasFiles<ScoreFileInfo>, IHasPrimaryKey, ISoftDelete, IEquatable<ScoreInfo>
    {
        public int ID { get; set; }

        [JsonProperty("rank")]
        [JsonConverter(typeof(StringEnumConverter))]
        public ScoreRank Rank { get; set; }

        [JsonProperty("total_score")]
        public long TotalScore { get; set; }

        [JsonProperty("accuracy")]
        [Column(TypeName = "DECIMAL(1,4)")]
        public double Accuracy { get; set; }

        [JsonIgnore]
        public string DisplayAccuracy => Accuracy.FormatAccuracy();
        
        [JsonProperty("max_combo")]
        public int MaxCombo { get; set; }

        [JsonIgnore]
        public int Combo { get; set; }
        
        [JsonProperty("passed")]
        [NotMapped]
        public bool Passed { get; set; } = true;
        
        [JsonIgnore]
        public int BeatmapInfoID { get; set; }

        [JsonIgnore]
        public virtual BeatmapInfo Beatmap { get; set; }
        
        [JsonIgnore]
        public DateTimeOffset Date { get; set; }

        [JsonProperty("statistics")]
        public Dictionary<HitResult, int> Statistics = new Dictionary<HitResult, int>();
        
        public IOrderedEnumerable<KeyValuePair<HitResult, int>> SortedStatistics => Statistics.OrderByDescending(pair => pair.Key);
        
        [JsonIgnore]
        public List<ScoreFileInfo> Files { get; set; }

        [JsonIgnore]
        public string Hash { get; set; }

        [JsonIgnore]
        public bool DeletePending { get; set; }
        
        public override string ToString() => $"Player playing {Beatmap}";

        public bool Equals(ScoreInfo other)
        {
            if (other == null)
                return false;

            if (ID != 0 && other.ID != 0)
                return ID == other.ID;

            if (!string.IsNullOrEmpty(Hash) && !string.IsNullOrEmpty(other.Hash))
                return Hash == other.Hash;

            return ReferenceEquals(this, other);
        }
    }
}
