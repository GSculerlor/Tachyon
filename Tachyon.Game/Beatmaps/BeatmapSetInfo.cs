using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using Tachyon.Game.Database;

namespace Tachyon.Game.Beatmaps
{
    public class BeatmapSetInfo : IHasPrimaryKey, IHasFiles<BeatmapSetFileInfo>, IEquatable<BeatmapSetInfo>
    {
        public int ID { get; set; }
        
        private int? onlineBeatmapSetID;

        public int? OnlineBeatmapSetID
        {
            get => onlineBeatmapSetID;
            set => onlineBeatmapSetID = value > 0 ? value : null;
        }

        public DateTimeOffset DateAdded { get; set; }

        public BeatmapMetadata Metadata { get; set; }

        public List<BeatmapInfo> Beatmaps { get; set; }

        public double MaxStarDifficulty => Beatmaps?.Max(b => b.StarDifficulty) ?? 0;

        public double MaxLength => Beatmaps?.Max(b => b.Length) ?? 0;

        public double MaxBPM => Beatmaps?.Max(b => b.BPM) ?? 0;

        public string Hash { get; set; }
        
        public List<BeatmapSetFileInfo> Files { get; set; }

        public override string ToString() => Metadata?.ToString() ?? base.ToString();

        public bool Protected { get; set; }

        public bool Equals(BeatmapSetInfo other)
        {
            if (other == null)
                return false;

            if (ID != 0 && other.ID != 0)
                return ID == other.ID;
            
            if (OnlineBeatmapSetID.HasValue && other.OnlineBeatmapSetID.HasValue)
                return OnlineBeatmapSetID == other.OnlineBeatmapSetID;

            if (!string.IsNullOrEmpty(Hash) && !string.IsNullOrEmpty(other.Hash))
                return Hash == other.Hash;

            return ReferenceEquals(this, other);
        }
    }
}