using osu.Framework.Graphics;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Screens;
using osuTK.Graphics;

namespace Tachyon.Game.Screens.Backgrounds
{
    public class TransparentScreen : BackgroundScreen
    {
        public TransparentScreen()
        {
            InternalChild = new Box
            {
                Colour = Color4.Transparent,
                RelativeSizeAxes = Axes.Both,
            };
        }

        public override void OnEntering(IScreen last)
        {
            Show();
        }
    }
}