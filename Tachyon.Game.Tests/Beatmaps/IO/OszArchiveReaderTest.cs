using System.IO;
using System.Linq;
using NUnit.Framework;
using Tachyon.Game.Beatmaps;
using Tachyon.Game.Beatmaps.Formats;
using Tachyon.Game.IO;
using Tachyon.Game.IO.Archives;
using Tachyon.Game.Tests.TestUtils;

namespace Tachyon.Game.Tests.Beatmaps.IO
{
    [TestFixture]
    public class OszArchiveReaderTest
    {
        [Test]
        public void TestReadBeatmapFiles()
        {
            using (var osz = TestResources.GetBeatmapsetForTest())
            {
                var reader = new ZipArchiveReader(osz);
                string[] expected =
                {
                    "O2i3 - Ooi (Capu) [reealy_'s Kantan].osu",
                    "O2i3 - Ooi (Capu) [reealy_'s Futsuu].osu",
                    "O2i3 - Ooi (Capu) [Muzukashii].osu",
                    "O2i3 - Ooi (Capu) [Oni].osu",
                    "O2i3 - Ooi (Capu) [Inner Ooni].osu",
                    "5678213.jpg",
                    "O2i3_-_Ooi.mp3"
                };
                var maps = reader.Filenames.ToArray();
                foreach (var map in expected)
                    Assert.Contains(map, maps);
            }
        }
        
        [Test]
        public void TestReadOsuFile()
        {
            using (var osz = TestResources.GetBeatmapsetForTest())
            {
                var reader = new ZipArchiveReader(osz);

                using (var stream = new StreamReader(
                    reader.GetStream("O2i3 - Ooi (Capu) [reealy_'s Kantan].osu")))
                {
                    Assert.AreEqual("osu file format v14", stream.ReadLine()?.Trim());
                }
            }
        }
        
        [Test]
        public void TestReadMetadataFromOsuFile()
        {
            using (var osz = TestResources.GetBeatmapsetForTest())
            {
                var reader = new ZipArchiveReader(osz);

                Beatmap beatmap;

                using (var stream = new LineBufferedReader(reader.GetStream("O2i3 - Ooi (Capu) [reealy_'s Kantan].osu")))
                    beatmap = Decoder.GetDecoder<Beatmap>(stream).Decode(stream);

                var meta = beatmap.Metadata;

                Assert.AreEqual("O2i3", meta.Artist);
                Assert.AreEqual("O2i3", meta.ArtistUnicode);
                Assert.AreEqual("O2i3_-_Ooi.mp3", meta.AudioFile);
                Assert.AreEqual("5678213.jpg", meta.BackgroundFile);
                Assert.AreEqual(74615, meta.PreviewTime);
                Assert.AreEqual(string.Empty, meta.Source);
                Assert.AreEqual("instrumental electronic jaxalate records happy hardcore realy0_ [game edit] ver version", meta.Tags);
                Assert.AreEqual("Ooi", meta.Title);
                Assert.AreEqual("Ooi", meta.TitleUnicode);
            }
        }
    }
}