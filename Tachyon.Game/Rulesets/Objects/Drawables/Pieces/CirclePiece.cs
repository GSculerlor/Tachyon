using osu.Framework.Extensions.Color4Extensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Effects;
using osu.Framework.Graphics.Shapes;
using osuTK.Graphics;

namespace Tachyon.Game.Rulesets.Objects.Drawables.Pieces
{
    public abstract class CirclePiece : Container
    {
        public const float SYMBOL_SIZE = 0.45f;
        public const float SYMBOL_BORDER = 8;
        
        private Color4 accentColor;

        /// <summary>
        /// The colour of the inner circle and outer glows.
        /// </summary>
        public Color4 AccentColor
        {
            get => accentColor;
            set
            {
                accentColor = value;

                background.Colour = AccentColor;
            }
        }
        
        protected override Container<Drawable> Content => content;

        private readonly Container content;

        private readonly Container background;

        protected CirclePiece()
        {
            RelativeSizeAxes = Axes.Both;
            
            AddRangeInternal(new Drawable[]
            {
                background = new CircularContainer
                {
                    Name = "Background",
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre,
                    RelativeSizeAxes = Axes.Both,
                    Masking = true,
                    Children = new Drawable[]
                    {
                        new Box
                        {
                            Anchor = Anchor.Centre,
                            Origin = Anchor.Centre,
                            RelativeSizeAxes = Axes.Both,
                        },
                    }
                },
                new CircularContainer
                {
                    Name = "Ring",
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre,
                    RelativeSizeAxes = Axes.Both,
                    BorderThickness = 20,
                    BorderColour = Color4.White,
                    Masking = true,
                    Children = new[]
                    {
                        new Box
                        {
                            Anchor = Anchor.Centre,
                            Origin = Anchor.Centre,
                            RelativeSizeAxes = Axes.Both,
                            Colour = Color4.White,
                            Blending = BlendingParameters.Additive,
                            Alpha = 0,
                            AlwaysPresent = true
                        }
                    }
                },
                content = new Container
                {
                    Name = "Content",
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre,
                    RelativeSizeAxes = Axes.Both,
                }
            });
        }
    }
}
