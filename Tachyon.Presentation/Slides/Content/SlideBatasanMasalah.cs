using System.Collections.Generic;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Sprites;
using osuTK;
using Tachyon.Game.Graphics;
using Tachyon.Game.Graphics.Containers;
using Tachyon.Presentation.Graphics;

namespace Tachyon.Presentation.Slides.Content
{
    public class SlideBatasanMasalah : SlideWithTitle
    {
        public SlideBatasanMasalah()
            : base("Batasan Masalah") { }

        [BackgroundDependencyLoader]
        private void load()
        {
            Content.Add(new FillFlowContainer
            {
                RelativeSizeAxes = Axes.Both,
                Direction = FillDirection.Vertical,
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                Spacing = new Vector2(0, 8),
                Children = new Drawable[]
                {
                    new ItemDrawable(new KeyValuePair<string, string>("osu!framework version", "Dalam pengembangannya akan digunakan banyak versi karena osu!framework sangat cepat dalam melakukan rilis"), FontAwesome.Solid.LayerGroup),
                    new ItemDrawable(new KeyValuePair<string, string>("Lingkungan Pengembangan", "JetBrain Rider pada lingkungan .NET Core SDK 3.0 dengan sistem operasi Windows 10, macOS Catalina, Android N"), FontAwesome.Solid.Laptop),
                    new ItemDrawable(new KeyValuePair<string, string>("Sistem operasi yang didukung Tachyon", "Windows 10 x64, Linux Debian-based, macOS Sierra keatas (10.12+), iOS 10 keatas, Android 5 keatas"), FontAwesome.Solid.TabletAlt),
                    new ItemDrawable(new KeyValuePair<string, string>("Bahasa Pemrograman", "Bahasa pemrograman C#"), FontAwesome.Solid.Code),
                    new ItemDrawable(new KeyValuePair<string, string>("Output auto generated beatmap", "Mengikuti standar osu! file format v14"), FontAwesome.Solid.FileSignature),
                }
            });
        }
    }
}
