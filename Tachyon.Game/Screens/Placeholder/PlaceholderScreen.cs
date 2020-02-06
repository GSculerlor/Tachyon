using osu.Framework.Allocation;
using osu.Framework.Extensions.Color4Extensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osuTK;
using osuTK.Graphics;
using Tachyon.Game.Graphics;
using Tachyon.Game.Screens.Backgrounds;

namespace Tachyon.Game.Screens.Placeholder
{
    public class PlaceholderScreen : TachyonScreen
    {
        private const string placeholderTitle = @"Placeholder Screen";
        private readonly Box box;

        public PlaceholderScreen()
        {
            Anchor = Anchor.Centre;
            Origin = Anchor.Centre;
            RelativeSizeAxes = Axes.Both;
            Size = new Vector2(0.3f);
            
            AddInternal(new Container
            {
                Origin = Anchor.Centre,
                Anchor = Anchor.Centre,
                RelativeSizeAxes = Axes.Both,
                CornerRadius = 20,
                Masking = true,
                Children = new Drawable[]
                {
                    box = new Box
                    {
                        Origin = Anchor.Centre,
                        Anchor = Anchor.Centre,
                        RelativeSizeAxes = Axes.Both,
                        Alpha = 0.5f,
                        Blending = BlendingParameters.Mixture,
                    },
                    new SpriteText
                    {
                        Origin = Anchor.Centre,
                        Anchor = Anchor.Centre,
                        Text = placeholderTitle,
                        Font = TachyonFont.GetFont(size: 24),
                        Colour = Color4.White
                    }
                }
            });
        }

        [BackgroundDependencyLoader]
        private void load(TachyonColor color)
        {
            box.Colour = color.YellowDarker.Opacity(0.5f);
        }

        protected override BackgroundScreen CreateBackground() => new BackgroundScreenDefault();
    }
}