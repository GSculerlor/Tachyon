using System;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.UserInterface;
using osu.Framework.Utils;
using Tachyon.Game.Graphics;

namespace Tachyon.Game.Screens.Play.HUD
{
    public class BeatmapProgressBar : SliderBar<double>
    {
        private readonly Box fill;
        
        public double StartTime
        {
            set => CurrentNumber.MinValue = value;
        }

        public double EndTime
        {
            set => CurrentNumber.MaxValue = value;
        }
        
        public double CurrentTime
        {
            set => CurrentNumber.Value = value;
        }

        public BeatmapProgressBar()
        {
            CurrentNumber.MinValue = 0;
            CurrentNumber.MaxValue = 1;

            RelativeSizeAxes = Axes.X;
            Height = 5;

            Children = new Drawable[]
            {
                fill = new Box
                {
                    Name = "Fill",
                    Anchor = Anchor.BottomLeft,
                    Origin = Anchor.BottomLeft,
                    Height = 10
                },
            };
        }

        [BackgroundDependencyLoader]
        private void load(TachyonColor colors)
        {
            fill.Colour = colors.BlueLight;
        }
        
        protected override void UpdateValue(float value)
        {
            // handled in update
        }

        protected override void Update()
        {
            base.Update();

            float newX = (float)Interpolation.Lerp(fill.Width, NormalizedValue * UsableWidth, Math.Clamp(Time.Elapsed / 40, 0, 1));
            fill.Width = newX;
        }
    }
}
