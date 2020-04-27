using System;
using System.Collections.Generic;
using NUnit.Framework;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Extensions.Color4Extensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osuTK;
using Tachyon.Game.Beatmaps;
using Tachyon.Game.Graphics.UserInterface;
using Tachyon.Game.Rulesets.Scoring;
using Tachyon.Game.Scoring;
using Tachyon.Game.Screens.Result;
using Tachyon.Game.Tests.Beatmaps;

namespace Tachyon.Game.Tests.Visual.Results
{
    public class TestSceneResultDetailPanel : TachyonTestScene
    {
        public override IReadOnlyList<Type> RequiredTypes => new[]
        {
            typeof(ResultDetailPanel),
            typeof(StatisticDisplay),
            typeof(ScoreCounter)
        };
        
        [Test]
        public void TestAddResultDetailPanel()
        {
            AddStep("show panel", () => showPanel(createTestBeatmap(), createTestScore()));
        }
        
        private void showPanel(WorkingBeatmap workingBeatmap, ScoreInfo score)
        {
            Child = new ResultDetailPanelContainer(workingBeatmap, score);
        }
        
        private WorkingBeatmap createTestBeatmap()
        {
            var beatmap = new TestBeatmap();

            return new TestWorkingBeatmap(beatmap);
        }

        private ScoreInfo createTestScore() => new ScoreInfo
        {
            Beatmap = new TestBeatmap().BeatmapInfo,
            TotalScore = 999999,
            Accuracy = 0.95,
            MaxCombo = 999,
            Rank = ScoreRank.S,
            Date = DateTimeOffset.Now,
            Statistics =
            {
                { HitResult.Miss, 1 },
                { HitResult.Good, 100 },
                { HitResult.Perfect, 300 },
            }
        };
        
        private class ResultDetailPanelContainer : Container
        {
            [Cached]
            private Bindable<WorkingBeatmap> workingBeatmap { get; set; }

            public ResultDetailPanelContainer(WorkingBeatmap beatmap, ScoreInfo score)
            {
                workingBeatmap = new Bindable<WorkingBeatmap>(beatmap);

                Anchor = Anchor.Centre;
                Origin = Anchor.Centre;
                Size = new Vector2(600, 400);
                Children = new Drawable[]
                {
                    new Box
                    {
                        RelativeSizeAxes = Axes.Both,
                        Colour = Color4Extensions.FromHex("#444"),
                    },
                    new ResultDetailPanel(score), 
                };
            }
        }
    }
}
