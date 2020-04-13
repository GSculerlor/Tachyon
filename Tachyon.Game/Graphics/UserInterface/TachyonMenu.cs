﻿using osu.Framework.Extensions.Color4Extensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.UserInterface;
using osuTK;
using osuTK.Graphics;
using Tachyon.Game.Graphics.Containers;

namespace Tachyon.Game.Graphics.UserInterface
{
    public class TachyonMenu : Menu
    {
        public TachyonMenu(Direction direction, bool topLevelMenu = false)
            : base(direction, topLevelMenu)
        {
            BackgroundColour = Color4.Black.Opacity(0.5f);

            MaskingContainer.CornerRadius = 4;
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

        protected override DrawableMenuItem CreateDrawableMenuItem(MenuItem item)
        {
            switch (item)
            {
                case StatefulMenuItem stateful:
                    return new DrawableStatefulMenuItem(stateful);
            }

            return new DrawableTachyonMenuItem(item);
        }

        protected override ScrollContainer<Drawable> CreateScrollContainer(Direction direction) => new TachyonScrollContainer(direction);

        protected override Menu CreateSubMenu() => new TachyonMenu(Direction.Vertical)
        {
            Anchor = Direction == Direction.Horizontal ? Anchor.BottomLeft : Anchor.TopRight
        };
    }
}
