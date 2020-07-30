using System.Collections.Generic;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Graphics.Textures;
using osuTK;
using Tachyon.Game.Graphics;
using Tachyon.Game.Graphics.Containers;
using Tachyon.Presentation.Graphics;

namespace Tachyon.Presentation.Slides.Content
{
    public class SlidePerancanganAutoGeneratorRandom : SlideWithTitle
    {
        public SlidePerancanganAutoGeneratorRandom()
            : base("Beatmap Auto Generator Metode Random") { }

        [BackgroundDependencyLoader]
        private void load(LargeTextureStore textures)
        {
            var texture = textures.Get(@"Presentation/beatmap_random");
            
            Content.Add(new FillFlowContainer
            {
                RelativeSizeAxes = Axes.Both,
                Direction = FillDirection.Vertical,
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                Spacing = new Vector2(0, 20),
                Children = new Drawable[]
                {
                    new Sprite
                    {
                        Anchor = Anchor.TopCentre,
                        Origin = Anchor.TopCentre,
                        Size = new Vector2(1200, 500),
                        Texture = texture,
                        FillMode = FillMode.Fill
                    },
                    new ItemDrawable(new KeyValuePair<string, string>("Metode Random", "Penentuan pattern berdasarkan RNG, tidak dengan pembacaan audio. Clock seeking berdasarkan value dari beat divisor yang sudah tentukan pemain"), FontAwesome.Solid.Random)
                    {
                        Anchor = Anchor.TopCentre,
                        Origin = Anchor.TopCentre,
                    },
                }
            });
        }
    }
}
