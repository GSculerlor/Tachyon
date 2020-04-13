 using osu.Framework.Audio.Track;
 using osu.Framework.Graphics.Textures;
 using Tachyon.Game.Beatmaps;

 namespace Tachyon.Game.Tests.Beatmaps
{
    public class TestWorkingBeatmap : WorkingBeatmap
    {
        private readonly IBeatmap beatmap;

        public TestWorkingBeatmap(IBeatmap beatmap)
            : base(beatmap.BeatmapInfo, null)
        {
            this.beatmap = beatmap;
        }

        protected override IBeatmap GetBeatmap() => beatmap;

        protected override Texture GetBackground() => null;

        protected override Track GetTrack() => null;
    }
}
