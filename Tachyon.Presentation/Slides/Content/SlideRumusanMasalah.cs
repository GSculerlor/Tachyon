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
    public class SlideRumusanMasalah : SlideWithTitle
    {
        public SlideRumusanMasalah()
            : base("Rumusan Masalah") { }

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
                    new ItemDrawable(new KeyValuePair<string, string>("Game mechanics pada multi-platform", "Bagaimana rancangan game mechanics untuk horizontal scrolling rhythm game pada multi-platform"), FontAwesome.Solid.Gamepad),
                    new ItemDrawable(
                        new KeyValuePair<string, string>("Rancangan game dan sistem", "Bagaimana rancangan game dengan menggunakan osu!framework"), FontAwesome.Solid.FileCode),
                    new ItemDrawable(new KeyValuePair<string, string>("Rancangan metode auto generated beatmap", "Bagaimana rancangan metode dan algoritma untuk auto generated beatmap"), FontAwesome.Solid.Terminal),
                }
            });
        }
    }
}
