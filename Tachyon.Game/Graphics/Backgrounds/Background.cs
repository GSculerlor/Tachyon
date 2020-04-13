using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Graphics.Textures;

namespace Tachyon.Game.Graphics.Backgrounds
{
    public class Background : CompositeDrawable
    {
        protected readonly Sprite Sprite;
        
        private readonly string textureName;

        public Background(string textureName = @"")
        {
            this.textureName = textureName;
            RelativeSizeAxes = Axes.Both;

            AddInternal(Sprite = new Sprite
            {
                RelativeSizeAxes = Axes.Both,
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                FillMode = FillMode.Fill,
            });
        }

        [BackgroundDependencyLoader]
        private void load(LargeTextureStore textures)
        {
            if (!string.IsNullOrEmpty(textureName))
                Sprite.Texture = textures.Get(textureName);
        }
    }
}