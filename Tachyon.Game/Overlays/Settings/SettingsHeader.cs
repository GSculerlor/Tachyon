using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using Tachyon.Game.Graphics;
using Tachyon.Game.Graphics.Sprites;

namespace Tachyon.Game.Overlays.Settings
{
    public class SettingsHeader : Container
    {
        private readonly string heading;
        private readonly string subheading;

        public SettingsHeader(string heading, string subheading)
        {
            this.heading = heading;
            this.subheading = subheading;
        }

        [BackgroundDependencyLoader]
        private void load(TachyonColor colors)
        {
            RelativeSizeAxes = Axes.X;
            AutoSizeAxes = Axes.Y;

            Children = new Drawable[]
            {
                new FillFlowContainer
                {
                    AutoSizeAxes = Axes.Y,
                    RelativeSizeAxes = Axes.X,
                    Direction = FillDirection.Vertical,
                    Children = new Drawable[]
                    {
                        new TachyonSpriteText
                        {
                            Text = heading,
                            Font = TachyonFont.GetFont(size: 40),
                            Margin = new MarginPadding
                            {
                                Left = SettingsPanel.CONTENT_MARGINS,
                                Top = 10f
                            },
                        },
                        new TachyonSpriteText
                        {
                            Colour = colors.BlueLight,
                            Text = subheading,
                            Font = TachyonFont.GetFont(size: 18).With(weight: FontWeight.SemiBold),
                            Margin = new MarginPadding
                            {
                                Left = SettingsPanel.CONTENT_MARGINS,
                                Bottom = 30
                            },
                        },
                    }
                }
            };
        }
    }
}
