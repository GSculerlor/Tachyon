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
    public class SlideTujuan : SlideWithTitle
    {
        public SlideTujuan()
            : base("Tujuan Tugas Akhir") { }

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
                    new ItemDrawable(new KeyValuePair<string, string>("Rhythm game", "Rhythm game yang bisa dimainkan pada platform desktop dan mobile"), FontAwesome.Solid.Gamepad),
                    new ItemDrawable(new KeyValuePair<string, string>("Auto generated beatmap system", "Rhythm game yang memiliki fitur auto generated beatmap sehingga pemain tidak perlu melakukannya secara manual"), FontAwesome.Solid.FileSignature),
                }
            });
        }
    }
}
