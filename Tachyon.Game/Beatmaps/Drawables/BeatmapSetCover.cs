using System;
using System.Linq;
using osu.Framework.Allocation;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Graphics.Textures;

namespace Tachyon.Game.Beatmaps.Drawables
{
    [LongRunningLoad]
    public class BeatmapSetCover : Sprite
    {
        private readonly BeatmapSetInfo set;

        public BeatmapSetCover(BeatmapSetInfo set)
        {
            if (set == null)
                throw new ArgumentNullException(nameof(set));

            this.set = set;
        }

        [BackgroundDependencyLoader]
        private void load(LargeTextureStore textures)
        {
            //TODO: Change it to check beatmapset instead of hardcoded texty
            Texture = textures.Get(@"Characters/Exusiai_1");
        }
    }
}
