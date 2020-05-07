using osu.Framework.Allocation;
using osu.Framework.Graphics.Sprites;
using osuTK.Graphics;
using Tachyon.Game.Graphics;

namespace Tachyon.Game.Screens.Play
{
    public class FailOverlay : GameplayMenuOverlay
    {
        [BackgroundDependencyLoader]
        private void load(TachyonColor colors)
        {
            AddButton("Retry", FontAwesome.Solid.UndoAlt, () => OnRetry?.Invoke());
            AddButton("Leave", FontAwesome.Solid.Running, () => OnQuit?.Invoke());
        }
    }
}
