using System;
using osu.Framework.Bindables;
using osu.Framework.Extensions.Color4Extensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Graphics.UserInterface;
using osu.Framework.Input.Events;
using osu.Framework.Logging;
using osuTK;
using osuTK.Graphics;
using Tachyon.Game.Graphics.UserInterface;

namespace Tachyon.Game.Screens.Play.HUD
{
    public class PauseButton : Container
    {
        public override bool ReceivePositionalInputAt(Vector2 screenSpacePos) => true;

        public readonly Bindable<bool> IsPaused = new Bindable<bool>();
        
        private readonly PauseButtonDrawable button;

        public Action Action
        {
            set => button.Action = value;
        }

        public PauseButton()
        {
            AutoSizeAxes = Axes.Both;
            Child = button = new PauseButtonDrawable
            {
                Icon = FontAwesome.Solid.Pause
            };
        }

        private class PauseButtonDrawable : IconButton
        {
            public PauseButtonDrawable()
            {
                AutoSizeAxes = Axes.Both;
                IconColor = Color4.White;
            }

            protected override void LoadComplete()
            {
                base.LoadComplete();

                Content.AutoSizeAxes = Axes.None;
                Content.Size = new Vector2(DEFAULT_BUTTON_SIZE);
            }

            protected override bool OnClick(ClickEvent e)
            {
                Action?.Invoke();
                
                return true;
            }
        }
    }
}
