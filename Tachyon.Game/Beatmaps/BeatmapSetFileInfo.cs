using System.ComponentModel.DataAnnotations;
using Tachyon.Game.Database;
using Tachyon.Game.IO;

namespace Tachyon.Game.Beatmaps
{
    public class BeatmapSetFileInfo : INamedFileInfo, IHasPrimaryKey
    {
        public int ID { get; set; }

        public int FileInfoID { get; set; }

        public FileInfo FileInfo { get; set; }

        [Required]
        public string Filename { get; set; }
    }
}