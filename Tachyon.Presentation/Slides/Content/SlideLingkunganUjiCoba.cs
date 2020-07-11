using System.Collections.Generic;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Textures;
using Tachyon.Game.Graphics;
using Tachyon.Game.Graphics.Containers;
using Tachyon.Game.Graphics.Sprites;
using Tachyon.Presentation.Graphics;

namespace Tachyon.Presentation.Slides.Content
{
    public class SlideLingkunganUjiCoba : SlideWithTitle
    {
        public SlideLingkunganUjiCoba()
            : base("Lingkungan Uji Coba") { }

        private LingkunganUjiCobaTableContainer table;
        private LingkunganUjiCobaTambahanTableContainer tableTambahan;

        [BackgroundDependencyLoader]
        private void load(LargeTextureStore textures)
        {
            Content.Add(new FillFlowContainer
            {
                RelativeSizeAxes = Axes.Both,
                Origin = Anchor.Centre,
                Anchor = Anchor.Centre,
                Direction = FillDirection.Vertical,
                Children = new Drawable[]
                {
                    new TachyonSpriteText
                    {
                        Margin = new MarginPadding { Left = 20, Bottom = 20 },
                        Text = "Lingkungan Uji Coba Utama",
                        Font = TachyonFont.GetFont(size: 26, weight: FontWeight.Bold)
                    },
                    table = new LingkunganUjiCobaTableContainer(),
                    new TachyonSpriteText
                    {
                        Margin = new MarginPadding { Top = 40, Left = 20, Bottom = 20 },
                        Text = "Lingkungan Uji Coba Tambahan",
                        Font = TachyonFont.GetFont(size: 26, weight: FontWeight.Bold)
                    },
                    tableTambahan = new LingkunganUjiCobaTambahanTableContainer(),
                }
            });
        }
        
        protected override void LoadComplete()
        {
            base.LoadComplete();
            
            createContent();
        }

        private void createContent()
        {
            table.Items = new List<KeyValuePair<string, string>>(new []
            {
                new KeyValuePair<string, string>("Perangkat Keras", "1.	Asus X550VX: Intel Core i7-6700HQ 54, NVIDIA GeForce GTX 950M (2GB GDDR5) 79, 1x 8GB DDR4"), 
                new KeyValuePair<string, string>("", "2. Macbook Pro 2017: 3.1GHz Intel Core i5-7267U, Intel Iris Plus Graphics 650, 8GB (2,133MHz LPDDR3)"), 
                new KeyValuePair<string, string>("", "3. Xiaomi Redmi Note 5A Prime"), 
                new KeyValuePair<string, string>("", "4. iPhone 11"), 
                new KeyValuePair<string, string>("Perangkat Lunak (Sistem Operasi)", "1. Microsoft Windows 10 64-bit"), 
                new KeyValuePair<string, string>("", "2. Ubuntu 20.04 LTS amd64"), 
                new KeyValuePair<string, string>("", "3. macOS Catalina 10.15.3"), 
                new KeyValuePair<string, string>("", "4. Android 10"), 
                new KeyValuePair<string, string>("", "5. iOS 13.4.1"), 
                new KeyValuePair<string, string>("Perangkat Lunak (Aplikasi Pengembang)", "JetBrains Rider 2019 3.2"), 
                new KeyValuePair<string, string>("Perangkat Lunak (Aplikasi Pembantu)", "Visual Studio Code, xcode, AVD Manager, Discord"), 
            });
            
            tableTambahan.Items = new List<KeyValuePair<string, string>>(new []
            {
                new KeyValuePair<string, string>("Perangkat Keras", "• HP Pavilion x360 Convertible PC. 14 inch"), 
                new KeyValuePair<string, string>("", "• Macbook Pro 2017: 3.1GHz Intel Core i5-7267U, Intel Iris Plus Graphics 650, 8GB (2,133MHz LPDDR3)"), 
                new KeyValuePair<string, string>("", "• Intel(R) Core i7-7700HQ (Stock)  GB DDR4 2133 MHz  Intel(R) HD Graphic 630 (Stock) 75hz 5ms display"),
                new KeyValuePair<string, string>("Perangkat Lunak (Sistem Operasi)", "• Microsoft Windows 10 64-bit"), 
                new KeyValuePair<string, string>("", "• macOS Catalina 10.15.3"), 
                new KeyValuePair<string, string>("Perangkat Lunak (Aplikasi Pembantu)", "Discord"), 
            });
        }
    }
}
