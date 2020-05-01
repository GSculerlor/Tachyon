using System;
using System.Linq;
using osu.Framework.Allocation;
using osu.Framework.Graphics.Sprites;
using Tachyon.Game.Graphics;

namespace Tachyon.Game.Screens.Play
{
    public class PauseOverlay : GameplayMenuOverlay
    {
        public Action OnResume;
        
        protected override Action BackAction => () => InternalButtons.Children.First().Click();

        [BackgroundDependencyLoader]
        private void load(TachyonColor colors)
        {
            AddButton("Resume", FontAwesome.Solid.Play, () => OnResume?.Invoke());
            AddButton("Retry", FontAwesome.Solid.UndoAlt, () => OnRetry?.Invoke());
            AddButton("Leave", FontAwesome.Solid.Running, () => OnQuit?.Invoke(), true);
        }
    }
}
