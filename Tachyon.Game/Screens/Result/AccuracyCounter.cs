using osu.Framework.Graphics;
using osuTK;
using Tachyon.Game.Graphics;
using Tachyon.Game.Graphics.UserInterface;
using Tachyon.Game.Utils;

namespace Tachyon.Game.Screens.Result
{
    public class AccuracyCounter : StatisticDisplay
    {
        private readonly double count;

        private RollingCounter<double> counter;

        public AccuracyCounter(double count)
            : base("Accuracy")
        {
            this.count = count;
        }

        public override void Appear()
        {
            base.Appear();
            counter.Current.Value = count;
        }

        protected override Drawable CreateContent() => counter = new Counter();

        private class Counter : RollingCounter<double>
        {
            protected override double RollingDuration => 300;

            protected override Easing RollingEasing => Easing.OutPow10;

            public Counter()
            {
                DisplayedCountSpriteText.Font = TachyonFont.Default.With(size: 28, weight: FontWeight.SemiBold, fixedWidth: true);
            }
            
            protected override string FormatCount(double count) => count.FormatAccuracy();

            public override void Increment(double amount)
                => Current.Value += amount;
        }
    }
}
