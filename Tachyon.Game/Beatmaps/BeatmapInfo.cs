using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;
using Tachyon.Game.Database;
using Tachyon.Game.IO.Serialization;

namespace Tachyon.Game.Beatmaps
{
    [Serializable]
    public class BeatmapInfo : IEquatable<BeatmapInfo>, IJsonSerializable, IHasPrimaryKey
    {
        public int ID { get; set; }
        
        private int? onlineBeatmapID;

        [JsonProperty("id")]
        public int? OnlineBeatmapID
        {
            get => onlineBeatmapID;
            set => onlineBeatmapID = value > 0 ? value : null;
        }
        
        [JsonIgnore]
        public int BeatmapSetInfoID { get; set; }

        [Required]
        public BeatmapSetInfo BeatmapSet { get; set; }

        public BeatmapMetadata Metadata { get; set; }

        [JsonIgnore]
        public int BaseDifficultyID { get; set; }

        public BeatmapDifficulty BaseDifficulty { get; set; }

        [NotMapped]
        public int? MaxCombo { get; set; }

        public double Length { get; set; }

        public double BPM { get; set; }

        public string Path { get; set; }

        [JsonProperty("file_sha2")]
        public string Hash { get; set; }

        [JsonProperty("file_md5")]
        public string MD5Hash { get; set; }

        public double AudioLeadIn { get; set; }
        public bool Countdown { get; set; } = true;
        public float StackLeniency { get; set; } = 0.7f;
        public bool SpecialStyle { get; set; }

        public string Version { get; set; }

        [JsonProperty("difficulty_rating")]
        public double StarDifficulty { get; set; }

        public override string ToString() => $"{Metadata} [{Version}]".Trim();
        
        [JsonIgnore]
        public BeatmapDifficultyRating BeatmapDifficultyRating
        {
            get
            {
                var rating = StarDifficulty;

                if (rating < 2.0) return BeatmapDifficultyRating.Easy;
                if (rating < 2.7) return BeatmapDifficultyRating.Normal;
                if (rating < 4.0) return BeatmapDifficultyRating.Hard;
                if (rating < 5.3) return BeatmapDifficultyRating.Insane;
                if (rating < 6.5) return BeatmapDifficultyRating.Expert;

                return BeatmapDifficultyRating.ExpertPlus;
            }
        }

        public bool Equals(BeatmapInfo other)
        {
            if (ID == 0 || other?.ID == 0)
                return ReferenceEquals(this, other);

            return ID == other?.ID;
        }

        public bool AudioEquals(BeatmapInfo other) => other != null && BeatmapSet != null && other.BeatmapSet != null &&
                                                      BeatmapSet.Hash == other.BeatmapSet.Hash &&
                                                      (Metadata ?? BeatmapSet.Metadata).AudioFile == (other.Metadata ?? other.BeatmapSet.Metadata).AudioFile;

        public bool BackgroundEquals(BeatmapInfo other) => other != null && BeatmapSet != null && other.BeatmapSet != null &&
                                                           BeatmapSet.Hash == other.BeatmapSet.Hash &&
                                                           (Metadata ?? BeatmapSet.Metadata).BackgroundFile == (other.Metadata ?? other.BeatmapSet.Metadata).BackgroundFile;

        public BeatmapInfo Clone() => (BeatmapInfo)MemberwiseClone();
    }
}