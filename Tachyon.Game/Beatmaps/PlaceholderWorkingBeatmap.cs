using osu.Framework.Audio;
using osu.Framework.Audio.Track;
using osu.Framework.Graphics.Textures;

namespace Tachyon.Game.Beatmaps
{
    public class PlaceholderWorkingBeatmap : WorkingBeatmap
    {
        private readonly TextureStore textures;
        
        public PlaceholderWorkingBeatmap(AudioManager audio, TextureStore textures) : base(new BeatmapInfo
        {
            Metadata = new BeatmapMetadata
            {
                Artist = "please load a beatmap!",
                Title = "no beatmaps available!"
            },
            BeatmapSet = new BeatmapSetInfo(),
            BaseDifficulty = new BeatmapDifficulty
            {
                DrainRate = 0,
                CircleSize = 0,
                OverallDifficulty = 0,
            }
        }, audio)
        {
            this.textures = textures;
        }

        protected override IBeatmap GetBeatmap() => new Beatmap();

        protected override Texture GetBackground() => textures?.Get(@"Backgrounds/background_kemono");

        protected override Track GetTrack() => GetVirtualTrack();
    }
}