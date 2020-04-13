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
        protected override DropdownHeader CreateHeader() => new TachyonDropdownHeader();

        protected override DropdownMenu CreateMenu() => new TachyonDropdownMenu();

        public class TachyonDropdownMenu : DropdownMenu
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
            
            protected override Menu CreateSubMenu() => new TachyonMenu(Direction.Vertical);

            protected override DrawableDropdownMenuItem CreateDrawableDropdownMenuItem(MenuItem item) => new TachyonDrawableDropdownMenuItem(item);

            protected override ScrollContainer<Drawable> CreateScrollContainer(Direction direction) => new TachyonScrollContainer(direction);

            public class TachyonDrawableDropdownMenuItem : DrawableDropdownMenuItem
            {
                public override bool HandlePositionalInput => true;
                
                private Color4 nonAccentHoverColour;
                private Color4 nonAccentSelectedColour;

                public TachyonDrawableDropdownMenuItem(MenuItem item)
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

                    nonAccentHoverColour = colors.PinkDarker;
                    nonAccentSelectedColour = Color4.Black.Opacity(0.5f);
                    updateColours();
                }
                
                protected override void UpdateForegroundColour()
                {
                    base.UpdateForegroundColour();

                    if (Foreground.Children.FirstOrDefault() is Content content) content.Chevron.Alpha = IsHovered ? 1 : 0;
                }

                protected override Drawable CreateContent() => new Content();
                
                private void updateColours()
                {
                    BackgroundColourHover = nonAccentHoverColour;
                    BackgroundColourSelected = nonAccentSelectedColour;
                    UpdateBackgroundColour();
                    UpdateForegroundColour();
                }

                protected new class Content : FillFlowContainer, IHasText
                {
                    public string Text
                    {
                        get => Label.Text;
                        set => Label.Text = value;
                    }

                    public readonly TachyonSpriteText Label;
                    public readonly SpriteIcon Chevron;

                    public Content()
                    {
                        RelativeSizeAxes = Axes.X;
                        AutoSizeAxes = Axes.Y;
                        Direction = FillDirection.Horizontal;

                        Children = new Drawable[]
                        {
                            Chevron = new SpriteIcon
                            {
                                AlwaysPresent = true,
                                Icon = FontAwesome.Solid.ChevronRight,
                                Colour = Color4.Black,
                                Alpha = 0.5f,
                                Size = new Vector2(8),
                                Margin = new MarginPadding { Left = 3, Right = 3 },
                                Origin = Anchor.CentreLeft,
                                Anchor = Anchor.CentreLeft,
                            },
                            Label = new TachyonSpriteText
                            {
                                Origin = Anchor.CentreLeft,
                                Anchor = Anchor.CentreLeft,
                            },
                        };
                    }
                }
            }
        }

        public class TachyonDropdownHeader : DropdownHeader
        {
            protected readonly SpriteText Text;

            protected override string Label
            {
                get => Text.Text;
                set => Text.Text = value;
            }

            protected readonly SpriteIcon Icon;

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
                BackgroundColourHover = colors.PinkDarker;
            }
        }
    }
}
