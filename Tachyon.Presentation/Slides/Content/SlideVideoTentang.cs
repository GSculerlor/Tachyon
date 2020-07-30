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
    public class SlideVideoTentang : SlideWithTitle
    {
        public SlideVideoTentang()
            : base("Tentang Tugas Akhir") { }

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
                    new ItemDrawable(new KeyValuePair<string, string>("Tachyon", "Tachyon merupakan horizontal scrolling rhythm game"), FontAwesome.Solid.Gamepad),
                    new ItemDrawable(
                        new KeyValuePair<string, string>("Teknologi yang Digunakan", "Tachyon menggunakan osu!framework sebagai game framework dan BASS Audio Library sebagai audio decoder"), FontAwesome.Solid.FileCode),
                    new ItemDrawable(new KeyValuePair<string, string>("Fitur-Fitur", "Fitur utama dari Tachyon adalah multi-platform dan auto generated beatmap"), FontAwesome.Solid.Terminal),
                }
            });
        }
    }
}
