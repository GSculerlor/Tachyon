using osu.Framework.Graphics;
using osu.Framework.Graphics.Sprites;

namespace Tachyon.Game.Graphics.UserInterface
{
    public class ToolbarBackButton : ToolbarButton
    {
        public ToolbarBackButton()
        {
            AutoSizeAxes = Axes.X;

            Icon = FontAwesome.Solid.ChevronLeft;
            Text = @"Back";
        }
    }
}