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
    public class SlideKesimpulan : SlideWithTitle
    {
        public SlideKesimpulan()
            : base("Kesimpulan") { }

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
                    new ItemDrawable(new KeyValuePair<string, string>("osu!framework", "Merupakan framework yang cukup robust dan bisa digunakan sebagai alternatif framework untuk game 2D. Memiliki Performa yang tinggi pada platform desktop dan performa yang cukup pada platform mobile"), FontAwesome.Solid.Fire),
                    new ItemDrawable(new KeyValuePair<string, string>("Auto-Generated Beatmap", "Beatmap yang dihasilkan secara otomatis dengan bantuan BASS Audio Library sebagai audio decoder sudah sangat bagus dalam konteks tingkat kesulitan dan variasi pattern yang dihasilkan, namun perlu improvement pada konteks timing"), FontAwesome.Solid.EllipsisH),
                    new ItemDrawable(new KeyValuePair<string, string>("Game mechanics", "Game mechanics yang ditawarkan, yaitu dual lane horizontal scrolling rhythm game sangat mudah diadaptasi oleh pemain baik pada platform desktop maupun mobile"), FontAwesome.Solid.Gamepad),
                }
            });
        }
    }
}
