using System;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using Tachyon.Game.Graphics;
using Tachyon.Game.Graphics.UserInterface;

namespace Tachyon.Game.Screens.Select
{
    public class PlayButton : VisibilityContainer
    {
        public Action Action;

        private readonly HoverableBackButton button;

        public PlayButton()
        {
            Size = HoverableBackButton.SIZE_EXTENDED;

            Child = button = new HoverableBackButton
            {
                Anchor = Anchor.TopRight,
                Origin = Anchor.TopRight,
                Text = @"start",
                Action = () => Action?.Invoke()
            };
        }

        [BackgroundDependencyLoader]
        private void load(TachyonColor colors)
        {
            button.BackgroundColour = colors.Secondary;
            button.HoverColour = colors.SecondaryDark;
        }

        protected override void PopIn()
        {
            button.FadeIn(150, Easing.OutQuint);
        }

        protected override void PopOut()
        {
            button.FadeOut(400, Easing.OutQuint);
        }
    }
}
