using System.Linq;
using osu.Framework.Allocation;
using osu.Framework.Extensions.Color4Extensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Graphics.UserInterface;
using osuTK;
using osuTK.Graphics;
using Tachyon.Game.Graphics.Containers;
using Tachyon.Game.Graphics.Sprites;

namespace Tachyon.Game.Graphics.UserInterface
{
    public class TachyonDropdown<T> : Dropdown<T>
    {
        private Color4 accentColour;

        public Color4 AccentColour
        {
            get => accentColour;
            set
            {
                accentColour = value;
                updateAccentColour();
            }
        }

        [BackgroundDependencyLoader]
        private void load(TachyonColor colors)
        {
            if (accentColour == default)
                accentColour = colors.PinkDarker;
            updateAccentColour();
        }

        private void updateAccentColour()
        {
            /*if (Header is IHasAccentColour header) header.AccentColour = accentColour;

            if (Menu is IHasAccentColour menu) menu.AccentColour = accentColour;*/
        }

        protected override DropdownHeader CreateHeader() => new TachyonDropdownHeader();

        protected override DropdownMenu CreateMenu() => new TachyonDropdownMenu();

        #region TachyonDropdownMenu

        protected class TachyonDropdownMenu : DropdownMenu
        {
            public override bool HandleNonPositionalInput => State == MenuState.Open;
            
            public TachyonDropdownMenu()
            {
                CornerRadius = 4;
                BackgroundColour = Color4.Black.Opacity(0.5f);
                ItemsContainer.Padding = new MarginPadding(5);
            }
            
            protected override void AnimateOpen() => this.FadeIn(300, Easing.OutQuint);
            protected override void AnimateClose() => this.FadeOut(300, Easing.OutQuint);
            
            protected override void UpdateSize(Vector2 newSize)
            {
                if (Direction == Direction.Vertical)
                {
                    Width = newSize.X;
                    this.ResizeHeightTo(newSize.Y, 300, Easing.OutQuint);
                }
                else
                {
                    Height = newSize.Y;
                    this.ResizeWidthTo(newSize.X, 300, Easing.OutQuint);
                }
            }

            private Color4 accentColour;

            public Color4 AccentColour
            {
                get => accentColour;
                set
                {
                    accentColour = value;
                    /*foreach (var c in Children.OfType<IHasAccentColour>())
                        c.AccentColour = value;*/
                }
            }

            protected override Menu CreateSubMenu() => new TachyonMenu(Direction.Vertical);

            protected override DrawableDropdownMenuItem CreateDrawableDropdownMenuItem(MenuItem item) => new DrawableTachyonDropdownMenuItem(item);

            protected override ScrollContainer<Drawable> CreateScrollContainer(Direction direction) => new TachyonScrollContainer(direction);

            #region DrawableOsuDropdownMenuItem

            public class DrawableTachyonDropdownMenuItem : DrawableDropdownMenuItem
            {
                // IsHovered is used
                public override bool HandlePositionalInput => true;

                private Color4? accentColour;

                public Color4 AccentColour
                {
                    get => accentColour ?? nonAccentSelectedColour;
                    set
                    {
                        accentColour = value;
                        updateColours();
                    }
                }

                private void updateColours()
                {
                    BackgroundColourHover = accentColour ?? nonAccentHoverColour;
                    BackgroundColourSelected = accentColour ?? nonAccentSelectedColour;
                    UpdateBackgroundColour();
                    UpdateForegroundColour();
                }

                private Color4 nonAccentHoverColour;
                private Color4 nonAccentSelectedColour;

                public DrawableTachyonDropdownMenuItem(MenuItem item)
                    : base(item)
                {
                    Foreground.Padding = new MarginPadding(2);

                    Masking = true;
                    CornerRadius = 6;
                }

                [BackgroundDependencyLoader]
                private void load(TachyonColor colors)
                {
                    BackgroundColour = Color4.Transparent;

                    nonAccentHoverColour = colors.BlueDarker;
                    nonAccentSelectedColour = Color4.Black.Opacity(0.5f);
                    updateColours();
                }

                protected override Drawable CreateContent() => new Content();

                protected new class Content : FillFlowContainer, IHasText
                {
                    public string Text
                    {
                        get => Label.Text;
                        set => Label.Text = value;
                    }

                    public readonly TachyonSpriteText Label;

                    public Content()
                    {
                        RelativeSizeAxes = Axes.X;
                        AutoSizeAxes = Axes.Y;
                        Direction = FillDirection.Horizontal;

                        Children = new Drawable[]
                        {
                            Label = new TachyonSpriteText
                            {
                                Font = TachyonFont.Default.With(size: 18, weight: FontWeight.SemiBold),
                                Origin = Anchor.CentreLeft,
                                Anchor = Anchor.CentreLeft,
                            },
                        };
                    }
                }
            }

            #endregion
        }

        #endregion

        public class TachyonDropdownHeader : DropdownHeader
        {
            protected readonly SpriteText Text;

            protected override string Label
            {
                get => Text.Text;
                set => Text.Text = value;
            }

            protected readonly SpriteIcon Icon;

            private Color4 accentColour;

            public virtual Color4 AccentColour
            {
                get => accentColour;
                set
                {
                    accentColour = value;
                    BackgroundColourHover = accentColour;
                }
            }

            public TachyonDropdownHeader()
            {
                Foreground.Padding = new MarginPadding(4);

                AutoSizeAxes = Axes.None;
                Margin = new MarginPadding { Bottom = 4 };
                CornerRadius = 4;
                Height = 40;

                Foreground.Children = new Drawable[]
                {
                    Text = new TachyonSpriteText
                    {
                        Font = TachyonFont.Default.With(size: 20, weight: FontWeight.SemiBold),
                        Anchor = Anchor.CentreLeft,
                        Origin = Anchor.CentreLeft,
                    },
                    Icon = new SpriteIcon
                    {
                        Icon = FontAwesome.Solid.ChevronDown,
                        Anchor = Anchor.CentreRight,
                        Origin = Anchor.CentreRight,
                        Margin = new MarginPadding { Right = 5 },
                        Size = new Vector2(12),
                    },
                };
            }

            [BackgroundDependencyLoader]
            private void load(TachyonColor colors)
            {
                BackgroundColour = Color4.Black.Opacity(0.5f);
                BackgroundColourHover = colors.BlueDarker;
            }
        }
    }
}
