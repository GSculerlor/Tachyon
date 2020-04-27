using osu.Framework.Graphics;
using osuTK;
using Tachyon.Game.Graphics;
using Tachyon.Game.Graphics.UserInterface;

namespace Tachyon.Game.Screens.Result
{
    public class ScoreCounter : RollingCounter<long>
    {
        protected override double RollingDuration => 300;

        protected override Easing RollingEasing => Easing.OutPow10;

        public ScoreCounter()
        {
            AutoSizeAxes = Axes.Y;
            RelativeSizeAxes = Axes.X;
            DisplayedCountSpriteText.Anchor = Anchor.TopCentre;
            DisplayedCountSpriteText.Origin = Anchor.TopCentre;

            DisplayedCountSpriteText.Font = TachyonFont.Numeric.With(size: 30, fixedWidth: true);
        }

        protected override string FormatCount(long count) => count.ToString("N0");

        public override void Increment(long amount)
            => Current.Value += amount;
    }
}
