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
    public class SlideHasilUjiCobaGameplay : SlideWithTitle
    {
        public SlideHasilUjiCobaGameplay()
            : base("Hasil Uji Coba Gameplay") { }

        [BackgroundDependencyLoader]
        private void load(LargeTextureStore textures)
        {
            var texture = textures.Get(@"Presentation/ujicoba_gameplay");
            
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
                    
                    new ItemDrawable(new KeyValuePair<string, string>("Uji coba gameplay", "Dilakukan oleh 8 pemain dengan rentang 1 sangat tidak setuju dan 5 sangat setuju"), FontAwesome.Solid.Book)
                    {
                        Anchor = Anchor.TopCentre,
                        Origin = Anchor.TopCentre,
                    },
                    new ItemDrawable(new KeyValuePair<string, string>("Hasil uji coba", "Mechanics dual lane lebih mudah digunakan baik pada desktop maupun mobile. Serta gameplay sangat mudah untuk diadaptasi"), FontAwesome.Solid.Poll)
                    {
                        Anchor = Anchor.TopCentre,
                        Origin = Anchor.TopCentre,
                    },
                }
            });
        }
    }
}
