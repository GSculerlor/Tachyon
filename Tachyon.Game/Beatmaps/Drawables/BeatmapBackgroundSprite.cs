using System;
using osu.Framework.Allocation;
using osu.Framework.Graphics.Sprites;

namespace Tachyon.Game.Beatmaps.Drawables
{
    public class BeatmapBackgroundSprite : Sprite
    {
        private readonly WorkingBeatmap workingBeatmap;

        public BeatmapBackgroundSprite(WorkingBeatmap workingBeatmap)
        {
            this.workingBeatmap = workingBeatmap ?? throw new ArgumentNullException(nameof(workingBeatmap));
        }

        [BackgroundDependencyLoader]
        private void load()
        {
            if (workingBeatmap.Background != null)
                Texture = workingBeatmap.Background;
        }
    }
}