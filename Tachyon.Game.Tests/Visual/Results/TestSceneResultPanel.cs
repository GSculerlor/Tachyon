using System;
using System.Collections.Generic;
using NUnit.Framework;
using osu.Framework.Graphics;
using Tachyon.Game.Rulesets;
using Tachyon.Game.Rulesets.Scoring;
using Tachyon.Game.Scoring;
using Tachyon.Game.Screens.Result;
using Tachyon.Game.Tests.Beatmaps;

namespace Tachyon.Game.Tests.Visual.Results
{
    public class TestSceneResultPanel : TachyonTestScene
    {
        public override IReadOnlyList<Type> RequiredTypes => new[]
        {
            typeof(ResultPanel),
            typeof(ScoreCounter)
        };

        [Test]
        public void addPanel()
        {
            var score = createScore();
            score.Accuracy = 0.75;
            score.Rank = ScoreRank.C;

            addPanelStep(score);
        }
        
        private void addPanelStep(ScoreInfo score) => AddStep("add panel", () =>
        {
            Child = new ResultPanel(score)
            {
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
            };
        });
        
        private ScoreInfo createScore() => new ScoreInfo
        {
            Beatmap = new TestBeatmap().BeatmapInfo,
            TotalScore = 2845370,
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
    }
}
