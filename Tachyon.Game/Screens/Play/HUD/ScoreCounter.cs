using Tachyon.Game.Graphics.UserInterface;

namespace Tachyon.Game.Screens.Play.HUD
{
    public class ScoreCounter : RollingCounter<double>
    {
        public ScoreCounter()
        {
            DisplayedCountSpriteText.Font = DisplayedCountSpriteText.Font.With(fixedWidth: true);
        }
        
        protected override double GetProportionalDuration(double currentValue, double newValue)
        {
            return currentValue > newValue ? currentValue - newValue : newValue - currentValue;
        }
        
        protected override string FormatCount(double count)
        {
            string format = new string('0', 0);

            return ((long)count).ToString(format);
        }
        
        public override void Increment(double amount)
        {
            Current.Value += amount;
        }
    }
}
