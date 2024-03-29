using NUnit.Framework;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Shapes;
using osuTK;
using osuTK.Graphics;
using Tachyon.Game.Beatmaps.ControlPoints;
using Tachyon.Game.Rulesets.Beatmaps;
using Tachyon.Game.Screens.Generate;
using Tachyon.Game.Screens.Generate.Components;

namespace Tachyon.Game.Tests.Visual.Generate
{
    public class TestSceneDistanceSnapGrid : EditorClockTestScene
    {
        private const double beat_length = 100;
        private static readonly Vector2 grid_position = new Vector2(512, 384);

        [Cached(typeof(EditorBeatmap))]
        private readonly EditorBeatmap editorBeatmap;

        [Cached(typeof(IDistanceSnapProvider))]
        private readonly SnapProvider snapProvider = new SnapProvider();

        public TestSceneDistanceSnapGrid()
        {
            editorBeatmap = new EditorBeatmap(new TachyonBeatmap());
            editorBeatmap.ControlPointInfo.Add(0, new TimingControlPoint { BeatLength = beat_length });
        }

        [SetUp]
        public void Setup() => Schedule(() =>
        {
            Children = new Drawable[]
            {
                new Box
                {
                    RelativeSizeAxes = Axes.Both,
                    Colour = Color4.SlateGray
                },
                new TestDistanceSnapGrid()
            };
        });

        [TestCase(1)]
        [TestCase(2)]
        [TestCase(3)]
        [TestCase(4)]
        [TestCase(6)]
        [TestCase(8)]
        [TestCase(12)]
        [TestCase(16)]
        public void TestBeatDivisor(int divisor)
        {
            AddStep($"set beat divisor = {divisor}", () => BeatDivisor.Value = divisor);
        }

        [Test]
        public void TestLimitedDistance()
        {
            AddStep("create limited grid", () =>
            {
                Children = new Drawable[]
                {
                    new Box
                    {
                        RelativeSizeAxes = Axes.Both,
                        Colour = Color4.SlateGray
                    },
                    new TestDistanceSnapGrid(100)
                };
            });
        }

        private class TestDistanceSnapGrid : DistanceSnapGrid
        {
            public new float DistanceSpacing => base.DistanceSpacing;

            public TestDistanceSnapGrid(double? endTime = null)
                : base(grid_position, 0, endTime)
            {
            }

            protected override void CreateContent()
            {
                AddInternal(new Circle
                {
                    Origin = Anchor.Centre,
                    Size = new Vector2(5),
                    Position = StartPosition
                });

                int indexFromPlacement = 0;

                for (float s = StartPosition.X + DistanceSpacing; s <= DrawWidth && indexFromPlacement < MaxIntervals; s += DistanceSpacing, indexFromPlacement++)
                {
                    AddInternal(new Circle
                    {
                        Origin = Anchor.Centre,
                        Size = new Vector2(5, 10),
                        Position = new Vector2(s, StartPosition.Y),
                        Colour = GetColourForIndexFromPlacement(indexFromPlacement)
                    });
                }

                indexFromPlacement = 0;

                for (float s = StartPosition.X - DistanceSpacing; s >= 0 && indexFromPlacement < MaxIntervals; s -= DistanceSpacing, indexFromPlacement++)
                {
                    AddInternal(new Circle
                    {
                        Origin = Anchor.Centre,
                        Size = new Vector2(5, 10),
                        Position = new Vector2(s, StartPosition.Y),
                        Colour = GetColourForIndexFromPlacement(indexFromPlacement)
                    });
                }

                indexFromPlacement = 0;

                for (float s = StartPosition.Y + DistanceSpacing; s <= DrawHeight && indexFromPlacement < MaxIntervals; s += DistanceSpacing, indexFromPlacement++)
                {
                    AddInternal(new Circle
                    {
                        Origin = Anchor.Centre,
                        Size = new Vector2(10, 5),
                        Position = new Vector2(StartPosition.X, s),
                        Colour = GetColourForIndexFromPlacement(indexFromPlacement)
                    });
                }

                indexFromPlacement = 0;

                for (float s = StartPosition.Y - DistanceSpacing; s >= 0 && indexFromPlacement < MaxIntervals; s -= DistanceSpacing, indexFromPlacement++)
                {
                    AddInternal(new Circle
                    {
                        Origin = Anchor.Centre,
                        Size = new Vector2(10, 5),
                        Position = new Vector2(StartPosition.X, s),
                        Colour = GetColourForIndexFromPlacement(indexFromPlacement)
                    });
                }
            }

            public override (Vector2 position, double time) GetSnappedPosition(Vector2 screenSpacePosition)
                => (Vector2.Zero, 0);
        }

        private class SnapProvider : IDistanceSnapProvider
        {
            public (Vector2 position, double time) GetSnappedPosition(Vector2 position, double time) => (position, time);

            public float GetBeatSnapDistanceAt(double referenceTime) => 10;

            public float DurationToDistance(double referenceTime, double duration) => (float)duration;

            public double DistanceToDuration(double referenceTime, float distance) => distance;

            public double GetSnappedDurationFromDistance(double referenceTime, float distance) => 0;

            public float GetSnappedDistanceFromDistance(double referenceTime, float distance) => 0;
        }
    }
}