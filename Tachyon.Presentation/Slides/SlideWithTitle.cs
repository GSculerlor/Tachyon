using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Graphics.Textures;
using osuTK.Graphics;
using Tachyon.Game.Graphics;
using Tachyon.Game.Graphics.Sprites;
using Tachyon.Game.Screens;

namespace Tachyon.Presentation.Slides
{
    public abstract class SlideWithTitle : TachyonScreen
    {
        protected Container Content;

        private const float header_text_size = 120;
        private const float padding = 30;

        private readonly MarginPadding adjustedPadding = new MarginPadding(padding) { Top = header_text_size + padding, Bottom = 0};
        private readonly string title;

        protected SlideWithTitle(string title)
        {
            this.title = title;
        }

        [BackgroundDependencyLoader]
        private void load(LargeTextureStore textures)
        {
            InternalChildren = new Drawable[]
            {
                new Container
                {
                    RelativeSizeAxes = Axes.X,
                    Height = 200,
                    Children = new Drawable[]
                    {
                        new Sprite
                        {
                            RelativeSizeAxes = Axes.Both,
                            Anchor = Anchor.Centre,
                            Origin = Anchor.Centre,
                            FillMode = FillMode.Fill,
                            Texture = textures.Get(@"Presentation/header")
                        },
                    }
                },
                new Container
                {
                    AutoSizeAxes = Axes.Both,
                    Margin = new MarginPadding(30),
                    Children = new Drawable[]
                    {
                        new TachyonSpriteText
                        {
                            Margin = new MarginPadding { Top = 30 },
                            Text = title,
                            Font = TachyonFont.Default.With(size: 48, weight: FontWeight.Bold),
                        },
                        new Box
                        {
                            Anchor = Anchor.BottomCentre,
                            Origin = Anchor.TopCentre,
                            Margin = new MarginPadding { Top = 6 },
                            RelativeSizeAxes = Axes.X,
                            Height = 4,
                            Colour = Color4.White
                        }
                    }
                },
                new Container
                {
                    RelativeSizeAxes = Axes.Both,
                    Padding = adjustedPadding,
                    Children = new Drawable[]
                    {
                        Content = new ConfinedInputContainer
                        {
                            Masking = true,
                            Anchor = Anchor.Centre,
                            Origin = Anchor.Centre,
                            RelativeSizeAxes = Axes.Both,
                        }
                    }
                },
            };
        }

        private class ConfinedInputContainer : Container
        {
            public override bool PropagateNonPositionalInputSubTree =>
                ReceivePositionalInputAt(GetContainingInputManager().CurrentState.Mouse.Position);
        }
    }
}
