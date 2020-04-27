using osu.Framework.Allocation;
using osu.Framework.Extensions;
using osu.Framework.Graphics;
using osuTK;
using Tachyon.Game.Graphics;
using Tachyon.Game.Graphics.UserInterface;
using Tachyon.Game.Rulesets.Scoring;

namespace Tachyon.Game.Screens.Result
{
    public class HitResultStatistic : StatisticDisplay
    {
        private readonly int count;
        private readonly HitResult result;

        private RollingCounter<int> counter;

        public HitResultStatistic(HitResult result, int count)
            : base(result.GetDescription())
        {
            this.count = count;
            this.result = result;
        }
        
        [BackgroundDependencyLoader]
        private void load(TachyonColor colors)
        {
            HeaderText.Colour = colors.ForHitResult(result);
        }

        public override void Appear()
        {
            base.Appear();
            counter.Current.Value = count;
        }

        protected override Drawable CreateContent() => counter = new Counter();

        private class Counter : RollingCounter<int>
        {
            protected override double RollingDuration => 300;

            protected override Easing RollingEasing => Easing.OutPow10;

            public Counter()
            {
                DisplayedCountSpriteText.Font = TachyonFont.Default.With(size: 24, weight: FontWeight.SemiBold, fixedWidth: true);
                DisplayedCountSpriteText.Spacing = new Vector2(-2, 0);
            }

            public override void Increment(int amount)
                => Current.Value += amount;
        }
    }
}
