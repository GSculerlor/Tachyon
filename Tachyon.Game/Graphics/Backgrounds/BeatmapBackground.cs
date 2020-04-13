using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Extensions.Color4Extensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Textures;
using osuTK.Graphics;
using Tachyon.Game.Beatmaps;

namespace Tachyon.Game.Graphics.Backgrounds
{
    public class BeatmapBackground : Background
    {
        public readonly WorkingBeatmap Beatmap;
        private readonly bool shouldDim;
        private readonly string fallbackTextureName;

        public BeatmapBackground(WorkingBeatmap beatmap, bool shouldDim = true ,string fallbackTextureName = @"Characters/Exusiai_1")
        {
            Beatmap = beatmap;
            this.fallbackTextureName = fallbackTextureName;
            this.shouldDim = shouldDim;
        }
        
        [BackgroundDependencyLoader]
        private void load(TextureStore textures)
        {
            Sprite.Texture = Beatmap?.Background ?? textures.Get(fallbackTextureName);

            if (shouldDim)
            {
                AddInternal(new Box
                {
                    RelativeSizeAxes = Axes.Both,
                    Colour = Color4.Black.Opacity(0.6f)
                });
            }
        }
    }
}
