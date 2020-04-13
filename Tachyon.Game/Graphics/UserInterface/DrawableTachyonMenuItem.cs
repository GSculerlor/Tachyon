using osu.Framework.Allocation;
using osu.Framework.Audio;
using osu.Framework.Audio.Sample;
using osu.Framework.Extensions.Color4Extensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Graphics.UserInterface;
using osu.Framework.Input.Events;
using osuTK.Graphics;
using Tachyon.Game.Graphics.Sprites;

namespace Tachyon.Game.Graphics.UserInterface
{
    public class DrawableTachyonMenuItem : Menu.DrawableMenuItem
    {
        public const int MARGIN_HORIZONTAL = 17;
        public const int MARGIN_VERTICAL = 4;
        private const int text_size = 17;
        private const int transition_length = 80;

        private TextContainer text;

        public DrawableTachyonMenuItem(MenuItem item)
            : base(item)
        {
        }

        [BackgroundDependencyLoader]
        private void load(AudioManager audio)
        {
            BackgroundColour = Color4.Transparent;
            BackgroundColourHover = Color4Extensions.FromHex(@"172023");

            updateTextColour();
        }

        private void updateTextColour()
        {
            switch ((Item as TachyonMenuItem)?.Type)
            {
                default:
                    text.Colour = Color4.White;
                    break;

                case MenuItemType.Destructive:
                    text.Colour = Color4.Red;
                    break;

                case MenuItemType.Highlighted:
                    text.Colour = Color4Extensions.FromHex(@"ffcc22");
                    break;
            }
        }

        protected override bool OnHover(HoverEvent e)
        {
            text.BoldText.FadeIn(transition_length, Easing.OutQuint);
            text.NormalText.FadeOut(transition_length, Easing.OutQuint);
            return base.OnHover(e);
        }

        protected override void OnHoverLost(HoverLostEvent e)
        {
            text.BoldText.FadeOut(transition_length, Easing.OutQuint);
            text.NormalText.FadeIn(transition_length, Easing.OutQuint);
            base.OnHoverLost(e);
        }

        protected sealed override Drawable CreateContent() => text = CreateTextContainer();
        protected virtual TextContainer CreateTextContainer() => new TextContainer();

        protected class TextContainer : Container, IHasText
        {
            public string Text
            {
                get => NormalText.Text;
                set
                {
                    NormalText.Text = value;
                    BoldText.Text = value;
                }
            }

            public readonly SpriteText NormalText;
            public readonly SpriteText BoldText;

            public TextContainer()
            {
                Anchor = Anchor.CentreLeft;
                Origin = Anchor.CentreLeft;

                AutoSizeAxes = Axes.Both;

                Children = new Drawable[]
                {
                    NormalText = new TachyonSpriteText
                    {
                        Anchor = Anchor.CentreLeft,
                        Origin = Anchor.CentreLeft,
                        Font = TachyonFont.GetFont(size: text_size),
                        Margin = new MarginPadding { Horizontal = MARGIN_HORIZONTAL, Vertical = MARGIN_VERTICAL },
                    },
                    BoldText = new TachyonSpriteText
                    {
                        AlwaysPresent = true,
                        Alpha = 0,
                        Anchor = Anchor.CentreLeft,
                        Origin = Anchor.CentreLeft,
                        Font = TachyonFont.GetFont(size: text_size, weight: FontWeight.Bold),
                        Margin = new MarginPadding { Horizontal = MARGIN_HORIZONTAL, Vertical = MARGIN_VERTICAL },
                    }
                };
            }
        }
    }
}
