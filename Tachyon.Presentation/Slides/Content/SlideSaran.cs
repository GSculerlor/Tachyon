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
    public class SlideSaran : SlideWithTitle
    {
        public SlideSaran()
            : base("Saran") { }

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
                    new ItemDrawable(new KeyValuePair<string, string>("User Interface dan User Experience", "Ditambahkan fitur yang menunjang user interface dan user experience seperti shortcut dan key bindings. Serta pemilihan font yang lebih bold terutama untuk mobile"), FontAwesome.Solid.Gamepad),
                    new ItemDrawable(new KeyValuePair<string, string>("Auto-Generated Beatmap", "Pembacaan fitur audio yang lebih imersif agar proses pendeteksian intensitas dan proses decision hit object bisa lebih baik"), FontAwesome.Solid.Stopwatch),
                }
            });
        }
    }
}
