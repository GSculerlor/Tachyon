using System;
using osu.Framework.Allocation;
using osu.Framework.Extensions.Color4Extensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Sprites;
using osuTK;

namespace Tachyon.Game.Graphics.UserInterface
{
    public class BackButton : VisibilityContainer
    {
        public Action Action;
        
        private readonly IconButton button;

        public BackButton()
        {
            Size = new Vector2(125, 50);
            Child = button = new IconButton
            {
                Origin = Anchor.BottomRight,
                Anchor = Anchor.BottomRight,
                Height = 0.8f,
                Width = 0.8f,
                Icon = FontAwesome.Solid.ArrowLeft,
                Action = () => Action?.Invoke()
            };
        }

        [BackgroundDependencyLoader]
        private void load(TachyonColor color)
        {
            button.NormalColor = color.BackButtonGray;
            button.HoverColor = color.BackButtonGray.Darken(0.5f);

        }
        
        protected override void PopIn()
        {
            button.MoveToX(0, 400, Easing.OutQuint);
            button.FadeIn(150, Easing.OutQuint);
        }

        protected override void PopOut()
        {
            button.MoveToX(-125, 400, Easing.OutQuint);
            button.FadeOut(400, Easing.OutQuint);
        }
    }
}