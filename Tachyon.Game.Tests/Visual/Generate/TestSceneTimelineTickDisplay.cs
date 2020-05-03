using NUnit.Framework;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osuTK;
using Tachyon.Game.Screens.Generate;
using Tachyon.Game.Screens.Generate.Components;

namespace Tachyon.Game.Tests.Visual.Generate
{
    [TestFixture]
    public class TestSceneTimelineTickDisplay : TimelineTestScene
    {
        public override Drawable CreateTestComponent() => new TimelineTickDisplay();

        [BackgroundDependencyLoader]
        private void load()
        {
            BeatDivisor.Value = 4;

            Add(new BeatDivisorControl(BeatDivisor)
            {
                Anchor = Anchor.TopRight,
                Origin = Anchor.TopRight,
                Margin = new MarginPadding(30),
                Size = new Vector2(90)
            });
        }
    }
}