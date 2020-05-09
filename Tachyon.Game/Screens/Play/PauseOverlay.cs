using System;
using System.Linq;
using osu.Framework.Allocation;
using osu.Framework.Graphics.Sprites;
using osuTK.Graphics;
using Tachyon.Game.Graphics;

namespace Tachyon.Game.Screens.Play
{
    public class PauseOverlay : GameplayMenuOverlay
    {
        public override string Header => "PAUSED";

        public Action OnResume;
        
        protected override Action BackAction => () => InternalButtons.Children.First().Click();

        [BackgroundDependencyLoader]
        private void load(TachyonColor colors)
        {
            AddButton(colors.SecondaryDark, "Resume", () => OnResume?.Invoke());
            AddButton(colors.SecondaryDark, "Retry", () => OnRetry?.Invoke());
            AddButton(colors.SecondaryDark, "Leave", () => OnQuit?.Invoke());
        }
    }
}
