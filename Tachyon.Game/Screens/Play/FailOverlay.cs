using osu.Framework.Allocation;
using osu.Framework.Graphics.Sprites;
using osuTK.Graphics;
using Tachyon.Game.Graphics;

namespace Tachyon.Game.Screens.Play
{
    public class FailOverlay : GameplayMenuOverlay
    {
        public override string Header => "FAILED";

        [BackgroundDependencyLoader]
        private void load(TachyonColor colors)
        {
            AddButton(colors.SecondaryDark, "Retry",  () => OnRetry?.Invoke());
            AddButton(colors.SecondaryDark, "Leave",  () => OnQuit?.Invoke());
        }
    }
}
