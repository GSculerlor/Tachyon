using System;
using System.Linq;
using osu.Framework.Audio;
using osu.Framework.Audio.Track;
using osu.Framework.Graphics.Textures;
using osu.Framework.IO.Stores;
using osu.Framework.Logging;
using Tachyon.Game.Beatmaps.Formats;
using Tachyon.Game.Generator.Waveforms;
using Tachyon.Game.IO;

namespace Tachyon.Game.Beatmaps
{
    public partial class BeatmapManager
    {
        protected class BeatmapManagerWorkingBeatmap : WorkingBeatmap
        {
            private readonly IResourceStore<byte[]> store;
            private ITrackStore trackStore;
            private TextureStore textureStore;
            
            public BeatmapManagerWorkingBeatmap(IResourceStore<byte[]> store, TextureStore textureStore, BeatmapInfo beatmapInfo, AudioManager audioManager) : base(beatmapInfo, audioManager)
            {
                this.store = store;
                this.textureStore = textureStore;
            }

            protected override IBeatmap GetBeatmap()
            {
                try
                {
                    using (var stream = new LineBufferedReader(store.GetStream(getPathForFile(BeatmapInfo.Path))))
                        return Decoder.GetDecoder<Beatmap>(stream).Decode(stream);
                }
                catch (Exception e)
                {
                    Logger.Error(e, "Beatmap failed to load");
                    return null;
                }
            }

            protected override Texture GetBackground()
            {
                if (Metadata?.BackgroundFile == null)
                    return null;

                try
                {
                    return textureStore.Get(getPathForFile(Metadata.BackgroundFile));
                }
                catch (Exception e)
                {
                    Logger.Error(e, "Background failed to load");
                    return null;
                }
            }

            protected override Track GetTrack()
            {
                try
                {
                    return (trackStore ??= AudioManager.GetTrackStore(store)).Get(getPathForFile(Metadata.AudioFile));
                }
                catch (Exception e)
                {
                    Logger.Error(e, "Track failed to load");
                    return null;
                }
            }
            
            public override void RecycleTrack()
            {
                base.RecycleTrack();

                trackStore?.Dispose();
                trackStore = null;
            }
            
            public override void TransferTo(WorkingBeatmap other)
            {
                base.TransferTo(other);

                if (other is BeatmapManagerWorkingBeatmap owb && textureStore != null && BeatmapInfo.BackgroundEquals(other.BeatmapInfo))
                    owb.textureStore = textureStore;
            }
            
            protected override TachyonWaveform GetWaveform()
            {
                try
                {
                    var trackData = store.GetStream(getPathForFile(Metadata.AudioFile));
                    return trackData == null ? null : new TachyonWaveform(trackData);
                }
                catch (Exception e)
                {
                    Logger.Error(e, "Waveform failed to load");
                    return null;
                }
            }
            
            private string getPathForFile(string filename) => BeatmapSetInfo.Files.FirstOrDefault(f => string.Equals(f.Filename, filename, StringComparison.OrdinalIgnoreCase))?.FileInfo.StoragePath;
        }
    }
}