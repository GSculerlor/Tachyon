using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;

namespace Tachyon.Game.Graphics.Containers
{
    public class AspectRatioContainer : Container
    {
        private int multiplier = 1;

        public int Multiplier
        {
            set
            {
                if (multiplier.Equals(value))
                    return;

                multiplier = value;
            }
        }

        protected override void Update()
        {
            base.Update();

            if (RelativeSizeAxes == Axes.X)
                Height = DrawWidth * multiplier;
            else
                Width = DrawHeight * multiplier;
        }
    }
}