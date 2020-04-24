using System;
using Tachyon.Game.Graphics.UserInterface;

namespace Tachyon.Game.Screens.Play.HUD
{
    public class ComboCounter : RollingCounter<int>
    {
        protected override double RollingDuration => 750;

        public ComboCounter()
        {
            Current.Value = DisplayedCount = 0;
        }

        protected override double GetProportionalDuration(int currentValue, int newValue)
        {
            return Math.Abs(currentValue - newValue) * RollingDuration * 100.0f;
        }

        public override void Increment(int amount)
        {
            Current.Value += amount;
        }
    }
}
