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
    public class SlideManfaat : SlideWithTitle
    {
        public SlideManfaat()
            : base("Manfaat Tugas Akhir") { }

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
                    new ItemDrawable(new KeyValuePair<string, string>("Multi-platform gameplay experience", "Memberikan pengalaman bermain yang sama antara PC dan mobile"), FontAwesome.Solid.Gamepad),
                    new ItemDrawable(new KeyValuePair<string, string>("Inovasi dalam pengembangan rhythm game", "Melihat kematangan osu!framework dan inovasi dalam auto beatmapping dengan BASS Audio Library"), FontAwesome.Solid.FileSignature),
                    new ItemDrawable(new KeyValuePair<string, string>("Hiburan untuk pemain", "Mengasah reflek pemain dalam rhythm game dan sarana hiburan"), FontAwesome.Solid.FistRaised),
                }
            });
        }
    }
}
