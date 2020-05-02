using System;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Input.Bindings;
using Tachyon.Game.Input;

namespace Tachyon.Game.Graphics.UserInterface
{
    public class BackButton : VisibilityContainer
    {
        public Action Action;

        private readonly HoverableBackButton button;

        public BackButton(Receptor receptor)
        {
            receptor.OnBackPressed = () => button.Click();

            Size = HoverableBackButton.SIZE_EXTENDED;

            Child = button = new HoverableBackButton
            {
                Anchor = Anchor.TopLeft,
                Origin = Anchor.TopLeft,
                Text = @"back",
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
            button.MoveToX(0, 400, Easing.OutQuint);
            button.FadeIn(150, Easing.OutQuint);
        }

        protected override void PopOut()
        {
            button.MoveToX(-HoverableBackButton.SIZE_EXTENDED.X / 2, 400, Easing.OutQuint);
            button.FadeOut(400, Easing.OutQuint);
        }

        public class Receptor : Drawable, IKeyBindingHandler<GlobalAction>
        {
            public Action OnBackPressed;

            public bool OnPressed(GlobalAction action)
            {
                switch (action)
                {
                    case GlobalAction.Back:
                        OnBackPressed?.Invoke();
                        return true;
                }

                return false;
            }

            public void OnReleased(GlobalAction action)
            {
            }
        }
    }
}
