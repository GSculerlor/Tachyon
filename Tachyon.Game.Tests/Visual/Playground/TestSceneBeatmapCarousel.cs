using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using NUnit.Framework;
using osu.Framework.Extensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using Tachyon.Game.Beatmaps;
using Tachyon.Game.Screens.Select;
using Tachyon.Game.Screens.Select.Carousel;

namespace Tachyon.Game.Tests.Visual.Playground
{
    public class TestSceneBeatmapCarousel : TachyonTestScene
    {
        private TestBeatmapCarousel carousel;

        public override IReadOnlyList<Type> RequiredTypes => new[]
        {
            typeof(CarouselItem),
            typeof(CarouselGroup),
            typeof(CarouselGroupEagerSelect),
            typeof(CarouselBeatmap),
            typeof(CarouselBeatmapSet),

            typeof(DrawableCarouselItem),
            typeof(CarouselItemState),

            typeof(DrawableCarouselBeatmap),
            typeof(DrawableCarouselBeatmapSet),
        };

        private const int set_count = 5;
        
        [Test]
        public void TestTraversal()
        {
            loadBeatmaps();

            waitForSelection(1, 1);

            advanceSelection(direction: 1, diff: true);
            waitForSelection(1, 2);

            advanceSelection(direction: -1, diff: false);
            waitForSelection(set_count, 1);

            advanceSelection(direction: -1, diff: true);
            waitForSelection(set_count - 1, 3);

            advanceSelection(false);
            advanceSelection(false);
            waitForSelection(1, 2);

            advanceSelection(direction: -1, diff: true);
            advanceSelection(direction: -1, diff: true);
            waitForSelection(set_count, 3);
        }
        
        [TestCase(true)]
        [TestCase(false)]
        public void TestTraversalBeyondVisible(bool forwards)
        {
            var sets = new List<BeatmapSetInfo>();

            const int total_set_count = 200;

            for (int i = 0; i < total_set_count; i++)
                sets.Add(createTestBeatmapSet(i + 1));

            loadBeatmaps(sets);

            for (int i = 1; i < total_set_count; i += i)
                selectNextAndAssert(i);

            void selectNextAndAssert(int amount)
            {
                setSelected(forwards ? 1 : total_set_count, 1);

                AddStep($"{(forwards ? "Next" : "Previous")} beatmap {amount} times", () =>
                {
                    for (int i = 0; i < amount; i++)
                    {
                        carousel.SelectNext(forwards ? 1 : -1);
                    }
                });

                waitForSelection(forwards ? amount + 1 : total_set_count - amount);
            }
        }
        
        [Test]
        public void TestTraversalBeyondVisibleDifficulties()
        {
            var sets = new List<BeatmapSetInfo>();

            const int total_set_count = 20;

            for (int i = 0; i < total_set_count; i++)
                sets.Add(createTestBeatmapSet(i + 1));

            loadBeatmaps(sets);

            selectNextAndAssert(3, true, 2, 1);
            selectNextAndAssert(50, true, 17, 3);
            selectNextAndAssert(200, true, 7, 3);
            selectNextAndAssert(3, false, 19, 3);
            selectNextAndAssert(50, false, 4, 1);
            selectNextAndAssert(200, false, 14, 1);

            void selectNextAndAssert(int amount, bool forwards, int expectedSet, int expectedDiff)
            {
                setSelected(forwards ? 1 : 20, forwards ? 1 : 3);

                AddStep($"{(forwards ? "Next" : "Previous")} difficulty {amount} times", () =>
                {
                    for (int i = 0; i < amount; i++)
                        carousel.SelectNext(forwards ? 1 : -1, false);
                });

                waitForSelection(expectedSet, expectedDiff);
            }
        }
        
        private void createCarousel(Container target = null)
        {
            AddStep("Create carousel", () =>
            {
                (target ?? this).Child = carousel = new TestBeatmapCarousel
                {
                    RelativeSizeAxes = Axes.Both,
                };
            });
        }
        
        private void loadBeatmaps(List<BeatmapSetInfo> beatmapSets = null)
        {
            createCarousel();

            if (beatmapSets == null)
            {
                beatmapSets = new List<BeatmapSetInfo>();

                for (int i = 1; i <= set_count; i++)
                    beatmapSets.Add(createTestBeatmapSet(i));
            }

            bool changed = false;
            AddStep($"Load {(beatmapSets.Count > 0 ? beatmapSets.Count.ToString() : "some")} beatmaps", () =>
            {
                carousel.BeatmapSetsChanged = () => changed = true;
                carousel.BeatmapSets = beatmapSets;
            });

            AddUntilStep("Wait for load", () => changed);
        }
        
        private void setSelected(int set, int diff) =>
            AddStep($"select set{set} diff{diff}", () =>
                carousel.SelectBeatmap(carousel.BeatmapSets.Skip(set - 1).First().Beatmaps.Skip(diff - 1).First()));
        
        private void advanceSelection(bool diff, int direction = 1, int count = 1)
        {
            if (count == 1)
            {
                AddStep($"select {(direction > 0 ? "next" : "prev")} {(diff ? "diff" : "set")}", () =>
                    carousel.SelectNext(direction, !diff));
            }
            else
            {
                AddRepeatStep($"select {(direction > 0 ? "next" : "prev")} {(diff ? "diff" : "set")}", () =>
                    carousel.SelectNext(direction, !diff), count);
            }
        }
        
        private void waitForSelection(int set, int? diff = null) =>
            AddUntilStep($"selected is set{set}{(diff.HasValue ? $" diff{diff.Value}" : "")}", () =>
            {
                if (diff != null)
                    return carousel.SelectedBeatmap == carousel.BeatmapSets.Skip(set - 1).First().Beatmaps.Skip(diff.Value - 1).First();

                return carousel.BeatmapSets.Skip(set - 1).First().Beatmaps.Contains(carousel.SelectedBeatmap);
            });
        
        private BeatmapSetInfo createTestBeatmapSet(int id)
        {
            return new BeatmapSetInfo
            {
                ID = id,
                OnlineBeatmapSetID = id,
                Hash = new MemoryStream(Encoding.UTF8.GetBytes(Guid.NewGuid().ToString())).ComputeMD5Hash(),
                Metadata = new BeatmapMetadata
                {
                    Artist = @"GSculerlor",
                    Title = $"Beatmap set number {id}"
                },
                Beatmaps = new List<BeatmapInfo>(new[]
                {
                    new BeatmapInfo
                    {
                        OnlineBeatmapID = id * 10,
                        Path = "normal.osu",
                        Version = "Looks normal to me",
                        StarDifficulty = 2,
                        BaseDifficulty = new BeatmapDifficulty
                        {
                            OverallDifficulty = 3.5f,
                        }
                    },
                    new BeatmapInfo
                    {
                        OnlineBeatmapID = id * 10 + 1,
                        Path = "hard.osu",
                        Version = "Wow so damn hard boi",
                        StarDifficulty = 5,
                        BaseDifficulty = new BeatmapDifficulty
                        {
                            OverallDifficulty = 5,
                        }
                    },
                    new BeatmapInfo
                    {
                        OnlineBeatmapID = id * 10 + 2,
                        Path = "insane.osu",
                        Version = "Wtf this is insane",
                        StarDifficulty = 6,
                        BaseDifficulty = new BeatmapDifficulty
                        {
                            OverallDifficulty = 7,
                        }
                    },
                }),
            };
        }
        
        private class TestBeatmapCarousel : BeatmapCarousel
        {
            protected override IEnumerable<BeatmapSetInfo> GetLoadableBeatmaps() => Enumerable.Empty<BeatmapSetInfo>();
        }
    }
}
