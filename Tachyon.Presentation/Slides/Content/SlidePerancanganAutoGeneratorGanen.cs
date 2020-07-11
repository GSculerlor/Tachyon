using System.Collections.Generic;
using osu.Framework.Allocation;
using osu.Framework.Audio;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Graphics.Textures;
using osuTK;
using Tachyon.Game.Beatmaps;
using Tachyon.Game.Graphics;
using Tachyon.Game.Graphics.Containers;
using Tachyon.Game.Rulesets;
using Tachyon.Game.Screens.Generate;
using Tachyon.Game.Screens.Generate.Components;
using Tachyon.Game.Tests;
using Tachyon.Presentation.Graphics;

namespace Tachyon.Presentation.Slides.Content
{
    public class SlidePerancanganAutoGeneratorGanen : SlideWithTitle
    {
        public SlidePerancanganAutoGeneratorGanen()
            : base("Perancangan Beatmap Auto Generator Metode Ganen") { }

        [BackgroundDependencyLoader]
        private void load(LargeTextureStore textures)
        {
            var texture = textures.Get(@"Presentation/beatmap_ganen");
            
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
                    new ItemDrawable(new KeyValuePair<string, string>("Metode Ganen", "Penentuan hit object berdasarkan pembacaan TachyonWaveform. Clock seeking berdasarkan value dari beat divisor yang sudah tentukan pemain"), FontAwesome.Solid.WaveSquare)
                    {
                        Anchor = Anchor.TopCentre,
                        Origin = Anchor.TopCentre,
                    },
                }
            });
        }
    }
}
