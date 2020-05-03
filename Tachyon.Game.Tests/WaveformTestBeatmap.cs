using System.IO;
using System.Linq;
using osu.Framework.Audio;
using osu.Framework.Audio.Track;
using osu.Framework.Graphics.Textures;
using Tachyon.Game.Beatmaps;
using Tachyon.Game.Beatmaps.Formats;
using Tachyon.Game.IO;
using Tachyon.Game.IO.Archives;
using Tachyon.Game.Tests.Beatmaps;
using Tachyon.Game.Tests.TestUtils;

namespace Tachyon.Game.Tests
{
    public class WaveformTestBeatmap : WorkingBeatmap
    {
        private readonly Beatmap beatmap;
        private readonly ITrackStore trackStore;

        public WaveformTestBeatmap(AudioManager audioManager) : this(new WaveformBeatmap(), audioManager)
        {
        }

        public WaveformTestBeatmap(Beatmap beatmap, AudioManager audioManager) : base(beatmap.BeatmapInfo, audioManager)
        {
            this.beatmap = beatmap;
            trackStore = audioManager.GetTrackStore(getZipReader());
        }

        ~WaveformTestBeatmap()
        {
            trackStore?.Dispose();
        }

        private static Stream getStream() => TestResources.GetBeatmapsetForTest();

        private static ZipArchiveReader getZipReader() => new ZipArchiveReader(getStream());

        protected override IBeatmap GetBeatmap() => beatmap;

        protected override Texture GetBackground() => null;

        protected override Waveform GetWaveform() => new Waveform(trackStore.GetStream(firstAudioFile));

        protected override Track GetTrack() => trackStore.Get(firstAudioFile);

        private string firstAudioFile
        {
            get
            {
                using (var reader = getZipReader())
                    return reader.Filenames.First(f => f.EndsWith(".mp3"));
            }
        }
        
        private class WaveformBeatmap : TestBeatmap
        {
            protected override Beatmap CreateBeatmap()
            {
                using (var reader = getZipReader())
                using (var beatmapStream = reader.GetStream(reader.Filenames.First(f => f.EndsWith(".osu"))))
                using (var beatmapReader = new LineBufferedReader(beatmapStream))
                    return Decoder.GetDecoder<Beatmap>(beatmapReader).Decode(beatmapReader);
            }
        }
    }
}