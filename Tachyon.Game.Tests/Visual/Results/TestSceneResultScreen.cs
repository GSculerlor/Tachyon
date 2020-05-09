using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Screens;
using Tachyon.Game.Beatmaps;
using Tachyon.Game.Graphics.UserInterface;
using Tachyon.Game.Rulesets;
using Tachyon.Game.Rulesets.Scoring;
using Tachyon.Game.Scoring;
using Tachyon.Game.Screens;
using Tachyon.Game.Screens.Play;
using Tachyon.Game.Screens.Result;
using Tachyon.Game.Tests.Beatmaps;

namespace Tachyon.Game.Tests.Visual.Results
{
    [TestFixture]
    public class TestSceneResultScreen : ScreenTestScene
    {
        private BeatmapManager beatmaps;

        public override IReadOnlyList<Type> RequiredTypes => new[]
        {
            typeof(ResultScreen),
            typeof(ResultPanel),
            typeof(OverlayButton),
            typeof(TestPlayer)
        };

        [BackgroundDependencyLoader]
        private void load(BeatmapManager beatmaps)
        {
            this.beatmaps = beatmaps;
        }
        
        protected override void LoadComplete()
        {
            base.LoadComplete();

            var beatmapInfo = beatmaps.QueryBeatmap(b => b.ID != 0);
            if (beatmapInfo != null)
                Beatmap.Value = beatmaps.GetWorkingBeatmap(beatmapInfo);
        }
        
        [Test]
        public void ResultsWithoutPlayer()
        {
            TestSoloResults screen = null;
            TachyonScreenStack stack;

            AddStep("load results", () =>
            {
                Child = stack = new TachyonScreenStack
                {
                    RelativeSizeAxes = Axes.Both
                };

                stack.Push(screen = createResultsScreen());
            });
            AddUntilStep("wait for loaded", () => screen.IsLoaded);
            AddAssert("retry overlay not present", () => screen.RetryButton == null);
        }

        [Test]
        public void ResultsWithPlayer()
        {
            TestSoloResults screen = null;

            AddStep("load results", () => Child = new TestResultsContainer(screen = createResultsScreen()));
            AddUntilStep("wait for loaded", () => screen.IsLoaded);
            AddAssert("retry overlay present", () => screen.RetryButton != null);
        }
        
        private TestSoloResults createResultsScreen() => new TestSoloResults(new ScoreInfo
        {
            TotalScore = 2845370,
            Accuracy = 0.98,
            MaxCombo = 123,
            Rank = ScoreRank.A,
            Date = DateTimeOffset.Now,
            Statistics = new Dictionary<HitResult, int>
            {
                { HitResult.Perfect, 50 },
                { HitResult.Good, 20 },
                { HitResult.Miss, 1 }
            },
            Beatmap = new TestBeatmap().BeatmapInfo,
        });
        
        private class TestResultsContainer : Container
        {
            [Cached(typeof(Player))]
            private readonly Player player = new TestPlayer();

            public TestResultsContainer(IScreen screen)
            {
                RelativeSizeAxes = Axes.Both;
                TachyonScreenStack stack;

                InternalChild = stack = new TachyonScreenStack
                {
                    RelativeSizeAxes = Axes.Both,
                };

                stack.Push(screen);
            }
        }
        
        private class TestSoloResults : ResultScreen
        {
            public OverlayButton RetryButton;
            
            public TestSoloResults(ScoreInfo score)
                : base(score)
            {
            }

            protected override void LoadComplete()
            {
                base.LoadComplete();
                
                RetryButton = InternalChildren.OfType<OverlayButton>().SingleOrDefault();
            }
        }
    }
}
