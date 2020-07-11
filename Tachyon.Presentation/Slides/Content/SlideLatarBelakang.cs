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
    public class SlideLatarBelakang : SlideWithTitle
    {
        public SlideLatarBelakang()
            : base("Latar Belakang") { }

        [BackgroundDependencyLoader]
        private void load(LargeTextureStore textures)
        {
            var texture = textures.Get(@"Presentation/rhythm_game");
            var textureframework = textures.Get(@"Etc/framework");
            
            Content.Add(new FillFlowContainer
            {
                RelativeSizeAxes = Axes.Both,
                Direction = FillDirection.Vertical,
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                Spacing = new Vector2(0, 10),
                Children = new Drawable[]
                {
                    new Container
                    {
                        RelativeSizeAxes = Axes.X,
                        Height = 5,
                        Origin = Anchor.Centre,
                        Anchor = Anchor.Centre
                    },
                    new GridContainer
                    {
                        RelativeSizeAxes = Axes.X,
                        Height = 300,
                        Origin = Anchor.Centre,
                        Anchor = Anchor.Centre,
                        Content = new[]
                        {
                            new Drawable[]
                            {
                                new Sprite
                                {
                                    Anchor = Anchor.Centre,
                                    Origin = Anchor.Centre,
                                    RelativeSizeAxes = Axes.Both,
                                    Texture = texture,
                                    FillMode = FillMode.Fit
                                },
                                new Sprite
                                {
                                    Anchor = Anchor.Centre,
                                    Origin = Anchor.Centre,
                                    RelativeSizeAxes = Axes.Both,
                                    Texture = textureframework,
                                    FillMode = FillMode.Fit
                                }
                            },
                        },
                        ColumnDimensions = new[]
                        {
                            new Dimension(),
                            new Dimension()
                        }
                    },
                    new ItemDrawable(new KeyValuePair<string, string>("Rhythm Game", "Populer semenjak munculnya Guitar Hero pada 2005, saat ini banyak sekali jenis rhythm game pada banyak platform seperti desktop, mobile, arcade, dsb. Namun masih jarang ditemukan rhythm game yang mendukung multi-platform karena harus menghandle mechanics pada setiap platformnya"), FontAwesome.Solid.Gamepad),
                    new ItemDrawable(new KeyValuePair<string, string>("osu!framework", "Framework ini merupakan framework game 2D (tidak hanya untuk rhythm game) yang memiliki support multi-platform."), FontAwesome.Solid.FileSignature),
                }
            });
            
        }
    }
}
