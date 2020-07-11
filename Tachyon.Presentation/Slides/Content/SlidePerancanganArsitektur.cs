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
    public class SlidePerancanganArsitektur : SlideWithTitle
    {
        public SlidePerancanganArsitektur()
            : base("Perancangan Arsitektur Sistem") { }

        [BackgroundDependencyLoader]
        private void load(LargeTextureStore textures)
        {
            var texture = textures.Get(@"Presentation/module");
            
            Content.Add(new FillFlowContainer
            {
                RelativeSizeAxes = Axes.Both,
                Direction = FillDirection.Vertical,
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                Spacing = new Vector2(0, 20),
                Children = new Drawable[]
                {
                    new Sprite
                    {
                        Anchor = Anchor.TopCentre,
                        Origin = Anchor.TopCentre,
                        Size = new Vector2(1200, 500),
                        Texture = texture,
                        FillMode = FillMode.Fit
                    },
                    new ItemDrawable(new KeyValuePair<string, string>("Game module dalam pengembangan", "Arsitektur dipisah menjadi beberapa module, dengan pembagian berdasarkan fungsi serta pembagian berdasarkan platform (desktop, Android dan iOS)"), FontAwesome.Solid.LayerGroup)
                    {
                        Anchor = Anchor.TopCentre,
                        Origin = Anchor.TopCentre,
                    },
                }
            });
        }
    }
}
