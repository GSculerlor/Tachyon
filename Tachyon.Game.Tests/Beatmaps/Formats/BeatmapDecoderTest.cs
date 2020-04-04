using System.IO;
using System.Linq;
using NUnit.Framework;
using Tachyon.Game.Audio;
using Tachyon.Game.Beatmaps;
using Tachyon.Game.Beatmaps.Formats;
using Tachyon.Game.Beatmaps.Timing;
using Tachyon.Game.IO;
using Tachyon.Game.Tests.TestUtils;

namespace Tachyon.Game.Tests.Beatmaps.Formats
{
    [TestFixture]
    public class BeatmapDecoderTest
    {
        [Test]
        public void TestDecodeBeatmapGeneral()
        {
            var decoder = new BeatmapDecoder();

            using (var resStream = TestResources.GetBeatmapForTest("Chroma - Made In Love (Nifty) [Thank you].osu"))
            using (var stream = new LineBufferedReader(resStream))
            {
                var beatmap = decoder.Decode(stream);
                var beatmapInfo = beatmap.BeatmapInfo;
                var metadata = beatmap.Metadata;

                Assert.AreEqual("Made In Love.mp3", metadata.AudioFile);
                Assert.AreEqual(0, beatmapInfo.AudioLeadIn);
                Assert.AreEqual(89010, metadata.PreviewTime);
                Assert.IsFalse(beatmapInfo.Countdown);
                Assert.AreEqual(0.7f, beatmapInfo.StackLeniency);
            }
        }
        
        [Test]
        public void TestDecodeBeatmapMetadata()
        {
            var decoder = new BeatmapDecoder();

            using (var resStream = TestResources.GetBeatmapForTest("Chroma - Made In Love (Nifty) [Thank you].osu"))
            using (var stream = new LineBufferedReader(resStream))
            {
                var beatmap = decoder.Decode(stream);
                var beatmapInfo = beatmap.BeatmapInfo;
                var metadata = beatmap.Metadata;

                Assert.AreEqual("Made In Love", metadata.Title);
                Assert.AreEqual("Made In Love", metadata.TitleUnicode);
                Assert.AreEqual("Chroma", metadata.Artist);
                Assert.AreEqual("黒魔", metadata.ArtistUnicode);
                Assert.AreEqual("Thank you", beatmapInfo.Version);
                Assert.AreEqual("SOUND VOLTEX IV HEAVENLY HAVEN", metadata.Source);
                Assert.AreEqual("sdvx electronic instrumental happy music sv robot & human", metadata.Tags);
            }
        }
        
        [Test]
        public void TestDecodeBeatmapDifficulty()
        {
            var decoder = new BeatmapDecoder();

            using (var resStream = TestResources.GetBeatmapForTest("Chroma - Made In Love (Nifty) [Thank you].osu"))
            using (var stream = new LineBufferedReader(resStream))
            {
                var difficulty = decoder.Decode(stream).BeatmapInfo.BaseDifficulty;

                Assert.AreEqual(5, difficulty.DrainRate);
                Assert.AreEqual(2, difficulty.CircleSize);
                Assert.AreEqual(6.5, difficulty.OverallDifficulty);
                Assert.AreEqual(10, difficulty.ApproachRate);
                Assert.AreEqual(1.4, difficulty.SliderMultiplier);
                Assert.AreEqual(1, difficulty.SliderTickRate);
            }
        }
        
        [Test]
        public void TestDecodeBeatmapEvents()
        {
            var decoder = new BeatmapDecoder();

            using (var resStream = TestResources.GetBeatmapForTest("Chroma - Made In Love (Nifty) [Thank you].osu"))
            using (var stream = new LineBufferedReader(resStream))
            {
                var beatmap = decoder.Decode(stream);
                var metadata = beatmap.Metadata;

                Assert.AreEqual("O52ps5m.jpg", metadata.BackgroundFile);
            }
        }
        
        [Test]
        public void TestDecodeBeatmapTimingPoints()
        {
            var decoder = new BeatmapDecoder();

            using (var resStream = TestResources.GetBeatmapForTest("Chroma - Made In Love (Nifty) [Thank you].osu"))
            using (var stream = new LineBufferedReader(resStream))
            {
                var beatmap = decoder.Decode(stream);
                var controlPoints = beatmap.ControlPointInfo;

                Assert.AreEqual(1, controlPoints.TimingPoints.Count);
                Assert.AreEqual(145, controlPoints.DifficultyPoints.Count);
                Assert.AreEqual(36, controlPoints.SamplePoints.Count);
                Assert.AreEqual(4, controlPoints.EffectPoints.Count);

                var timingPoint = controlPoints.TimingPointAt(0);
                Assert.AreEqual(374, timingPoint.Time);
                Assert.AreEqual(681.818181818182, timingPoint.BeatLength);
                Assert.AreEqual(TimeSignatures.SimpleQuadruple, timingPoint.TimeSignature);
            }
        }
        
        [Test]
        public void TestDecodeBeatmapHitObjects()
        {
            var decoder = new BeatmapDecoder();

            using (var resStream = TestResources.GetBeatmapForTest("Chroma - Made In Love (Nifty) [Thank you].osu"))
            using (var stream = new LineBufferedReader(resStream))
            {
                var hitObjects = decoder.Decode(stream).HitObjects;

                var positionData = hitObjects[0];

                Assert.IsNotNull(positionData);
                Assert.AreEqual(3101, positionData.StartTime);
                Assert.IsTrue(positionData.Samples.Any(s => s.Name == HitSampleInfo.HIT_NORMAL));

                positionData = hitObjects[11];
                
                Assert.IsNotNull(positionData);
                Assert.AreEqual(9578, positionData.StartTime);
                Assert.IsTrue(positionData.Samples.Any(s => s.Name == HitSampleInfo.HIT_CLAP));
            }
        }
        
        [Test]
        public void TestDecodeEmptyFile()
        {
            using (var resStream = new MemoryStream())
            using (var stream = new LineBufferedReader(resStream))
            {
                Assert.Throws<IOException>(() => Decoder.GetDecoder<Beatmap>(stream));
            }
        }
    }
}