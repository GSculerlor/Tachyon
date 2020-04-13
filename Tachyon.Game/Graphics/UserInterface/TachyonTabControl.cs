using System;
using osu.Framework.Bindables;
using osu.Framework.Extensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Graphics.UserInterface;
using osuTK;
using osuTK.Graphics;
using Tachyon.Game.Graphics.Sprites;

namespace Tachyon.Game.Graphics.UserInterface
{
    public class TachyonTabControl<T> : TabControl<T>
    {
        private Vector2? spacing;

        public Vector2 Spacing
        {
            get => spacing ?? new Vector2(10, 0);
            set
            {
                spacing = value;
                TabContainer.Spacing = value;
            }
        }
        
        public TachyonTabControl()
        {
            TabContainer.Spacing = Spacing;
            
            if (isEnumType && AddEnumEntriesAutomatically)
            {
                foreach (var val in (T[])Enum.GetValues(typeof(T)))
                    AddItem(val);
            }
        }
        
        protected virtual bool AddEnumEntriesAutomatically => true;

        private static bool isEnumType => typeof(T).IsEnum;
        
        protected override Dropdown<T> CreateDropdown() => new TachyonTabDropdown<T>();

        protected override TabItem<T> CreateTabItem(T value) => new TachyonTabItem(value);
        
        public class TachyonTabItem : TabItem<T>
        {
            private const float transition_length = 500;
            
            protected readonly SpriteText Text;
            protected readonly CircularContainer BottomIndicator;
            
            public TachyonTabItem(T value)
                : base(value)
            {
                AutoSizeAxes = Axes.X;
                RelativeSizeAxes = Axes.Y;
                
                Children = new Drawable[]
                {
                    Text = new TachyonSpriteText
                    {
                        Margin = new MarginPadding { Top = 5, Bottom = 6 },
                        Origin = Anchor.BottomLeft,
                        Anchor = Anchor.BottomLeft,
                        Text = (value as IHasDescription)?.Description ?? (value as Enum)?.GetDescription() ?? value.ToString(),
                        Font = TachyonFont.GetFont(size: 24, weight: FontWeight.Bold),
                    },
                    BottomIndicator = new CircularContainer()
                    {
                        Size = new Vector2(6f),
                        Alpha = 0,
                        Masking = true,
                        Origin = Anchor.BottomCentre,
                        Anchor = Anchor.BottomCentre,
                        Child = new Box
                        {
                            RelativeSizeAxes = Axes.Both,
                            Colour = Color4.White,
                        }
                    },
                };
            }

            protected override void OnActivated()
            {
                BottomIndicator.FadeIn(transition_length, Easing.OutQuint);
            }

            protected override void OnDeactivated()
            {
                BottomIndicator.FadeOut(transition_length, Easing.OutQuint);
            }
        }
    }
}
