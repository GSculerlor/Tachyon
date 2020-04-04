using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using Newtonsoft.Json;
using Tachyon.Game.Database;

namespace Tachyon.Game.Beatmaps
{
    [Serializable]
    public class BeatmapMetadata : IEquatable<BeatmapMetadata>, IHasPrimaryKey
    {
        public int ID { get; set; }

        public string Title { get; set; }
        public string TitleUnicode { get; set; }
        public string Artist { get; set; }
        public string ArtistUnicode { get; set; }

        [JsonIgnore]
        public List<BeatmapInfo> Beatmaps { get; set; }

        [JsonIgnore]
        public List<BeatmapSetInfo> BeatmapSets { get; set; }

        public string Source { get; set; }

        [JsonProperty(@"tags")]
        public string Tags { get; set; }

        public int PreviewTime { get; set; }
        public string AudioFile { get; set; }
        public string BackgroundFile { get; set; }

        public override string ToString() => $"{Artist} - {Title})";

        [JsonIgnore]
        public string[] SearchableTerms => new[]
        {
            Artist,
            ArtistUnicode,
            Title,
            TitleUnicode,
            Source,
            Tags
        }.Where(s => !string.IsNullOrEmpty(s)).ToArray();

        public bool Equals(BeatmapMetadata other)
        {
            if (other == null)
                return false;

            return Title == other.Title
                   && TitleUnicode == other.TitleUnicode
                   && Artist == other.Artist
                   && ArtistUnicode == other.ArtistUnicode
                   && Source == other.Source
                   && Tags == other.Tags
                   && PreviewTime == other.PreviewTime
                   && AudioFile == other.AudioFile
                   && BackgroundFile == other.BackgroundFile;
        }
    }
}