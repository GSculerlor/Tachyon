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
    public class SlideHasilUjiCobaPerformaTambahan : SlideWithTitle
    {
        public SlideHasilUjiCobaPerformaTambahan()
            : base("Hasil Uji Coba Performa (Lingungan Uji Coba Tambahan)") { }

        [BackgroundDependencyLoader]
        private void load(LargeTextureStore textures)
        {
            var texture = textures.Get(@"Presentation/ujicoba_performa_tambahan");
            
            Content.Add(new FillFlowContainer
            {
                RelativeSizeAxes = Axes.Both,
                Direction = FillDirection.Vertical,
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                Spacing = new Vector2(0, 10),
                Children = new Drawable[]
                {
                    new Sprite
                    {
                        Anchor = Anchor.TopCentre,
                        Origin = Anchor.TopCentre,
                        Size = new Vector2(1000, 400),
                        Texture = texture,
                        FillMode = FillMode.Fit
                    },
                    
                    new ItemDrawable(new KeyValuePair<string, string>("Uji coba performa", "Dilakukan pada 4 devices dengan spesifikasi tinggi. Poin yand dilihat adalah frame rate dan delay pada 3 stage, yaitu idle, gameplay, dan minimized"), FontAwesome.Solid.Book)
                    {
                        Anchor = Anchor.TopCentre,
                        Origin = Anchor.TopCentre,
                    },
                    new ItemDrawable(new KeyValuePair<string, string>("Hasil uji coba", "Pada lingkungan dengan spesifikasi tinggi, performa Tachyon sangat tinggi jika dibandingkan pada lingkungan uji coba utama. Frame rate dan delay diatas batas acceptable (60 fps)"), FontAwesome.Solid.Poll)
                    {
                        Anchor = Anchor.TopCentre,
                        Origin = Anchor.TopCentre,
                    },
                }
            });
        }
    }
}
