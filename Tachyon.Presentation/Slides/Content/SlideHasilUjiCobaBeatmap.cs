using System.Collections.Generic;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Graphics.Textures;
using osuTK;
using Tachyon.Presentation.Graphics;

namespace Tachyon.Presentation.Slides.Content
{
    public class SlideHasilUjiCobaBeatmap : SlideWithTitle
    {
        public SlideHasilUjiCobaBeatmap()
            : base("Hasil Uji Coba Auto Generated Beatmap") { }

        [BackgroundDependencyLoader]
        private void load(LargeTextureStore textures)
        {
            var texture = textures.Get(@"Presentation/ujicoba_beatmap");
            
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
                    
                    new ItemDrawable(new KeyValuePair<string, string>("Uji coba auto generated beatmap", "Dilakukan oleh 8 pemain, 4 diantaranya memiliki latar belakang sebagai mapper dengan rentang 1 sangat tidak setuju dan 5 sangat setuju"), FontAwesome.Solid.Book)
                    {
                        Anchor = Anchor.TopCentre,
                        Origin = Anchor.TopCentre,
                    },
                    new ItemDrawable(new KeyValuePair<string, string>("Hasil uji coba", "Tingkat kesulitan dan tingkat variasi pattern yang dihasilkan cukup tinggi, dengan kata lain beatmap yang dihasilkan tidak membosankan. Sedangkan untuk timing terdapat beberapa flaw terutama sering terjadi pada bagian akhir dari audio"), FontAwesome.Solid.Poll)
                    {
                        Anchor = Anchor.TopCentre,
                        Origin = Anchor.TopCentre,
                    }
                }
            });
        }
    }
}
