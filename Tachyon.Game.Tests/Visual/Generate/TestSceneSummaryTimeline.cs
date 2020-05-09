using System;
using System.Collections.Generic;
using NUnit.Framework;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osuTK;
using Tachyon.Game.Screens.Generate.Components;

namespace Tachyon.Game.Tests.Visual.Generate
{
    [TestFixture]
    public class TestSceneSummaryTimeline : EditorClockTestScene
    {
        public override IReadOnlyList<Type> RequiredTypes => new[] { typeof(SummaryTimeline) };

        [BackgroundDependencyLoader]
        private void load()
        {
            Beatmap.Value = CreateWorkingBeatmap();

            Add(new SummaryTimeline
            {
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                Size = new Vector2(500, 50)
            });
        }
    }
}
