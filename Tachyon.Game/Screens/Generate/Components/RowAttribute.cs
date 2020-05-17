using System;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Cursor;
using osu.Framework.Graphics.Shapes;
using osuTK.Graphics;
using Tachyon.Game.Graphics;
using Tachyon.Game.Graphics.Sprites;

namespace Tachyon.Game.Screens.Generate.Components
{
    public class RowAttribute : CompositeDrawable, IHasTooltip
    {
        private readonly string header;
        private readonly Func<string> content;

        public RowAttribute(string header, Func<string> content)
        {
            this.header = header;
            this.content = content;
        }

        [BackgroundDependencyLoader]
        private void load(TachyonColor colors)
        {
            AutoSizeAxes = Axes.X;

            Height = 20;

            Anchor = Anchor.CentreLeft;
            Origin = Anchor.CentreLeft;

            Masking = true;
            CornerRadius = 5;

            InternalChildren = new Drawable[]
            {
                new Box
                {
                    Colour = header.Contains("Upper") ? colors.UpperHitObject : colors.LowerHitObject,
                    RelativeSizeAxes = Axes.Both,
                },
                new TachyonSpriteText
                {
                    Padding = new MarginPadding(2),
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre,
                    Font = TachyonFont.Default.With(weight: FontWeight.SemiBold, size: 18),
                    Text = header,
                    Colour = Color4.White
                },
            };
        }

        public string TooltipText => content();
    }
}
