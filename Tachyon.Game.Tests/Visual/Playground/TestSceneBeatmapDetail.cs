using System.Collections.Generic;
using JetBrains.Annotations;
using NUnit.Framework;
using osu.Framework.Graphics;
using osuTK;
using Tachyon.Game.Beatmaps;
using Tachyon.Game.Rulesets.Converters;
using Tachyon.Game.Rulesets.Objects;
using Tachyon.Game.Screens.Playground.Detail;

namespace Tachyon.Game.Tests.Visual.Playground
{
    public class TestSceneBeatmapDetail : TachyonTestScene
    {
        private TestBeatmapDetail beatmapDetail;
        private readonly List<IBeatmap> beatmaps = new List<IBeatmap>();

        protected override void LoadComplete()
        {
            base.LoadComplete();
            
            Add(beatmapDetail = new TestBeatmapDetail
            {
                Size = new Vector2(0.5f, 245),
                RelativeSizeAxes = Axes.X,
                Margin = new MarginPadding { Top = 20 }
            });
            
            AddStep("show", () =>
            {
                beatmapDetail.Show();
                beatmapDetail.Beatmap = Beatmap.Value;
                
            });
            
            selectBeatmap(Beatmap.Value.Beatmap);
            AddWaitStep("wait for select", 3);

            AddStep("hide", () => { beatmapDetail.Hide(); });
            AddWaitStep("wait for hide", 3);

            AddStep("show", () => { beatmapDetail.Show(); });
        }

        [Test]
        public void TestNotNullBeatmap()
        {
            var testBeatmap = createTestBeatmap();
            beatmaps.Add(testBeatmap);

            selectBeatmap(testBeatmap);
            testBeatmapLabel();
        }
        
        [Test]
        public void TestNullBeatmap()
        {
            selectBeatmap(null);
            AddAssert("check default title", () => beatmapDetail.DetailContent.TitleLabel.Text == $"{Beatmap.Default.BeatmapInfo.Metadata.Artist} - {Beatmap.Default.BeatmapInfo.Metadata.Title}");
        }

        private void selectBeatmap([CanBeNull] IBeatmap beatmap)
        {
            BeatmapDetail.BeatmapDetailContent previousContent = null;

            AddStep($"select {beatmap?.Metadata.Title ?? "null"} beatmap", () =>
            {
                previousContent = beatmapDetail.DetailContent;
                beatmapDetail.Beatmap = Beatmap.Value = beatmap == null ? Beatmap.Default : CreateWorkingBeatmap(beatmap);
            });

            AddUntilStep("wait for async load", () => beatmapDetail.DetailContent != previousContent);
        }

        private IBeatmap createTestBeatmap()
        {
            List<HitObject> objects = new List<HitObject>();
            for (double i = 0; i < 50000; i += 1000)
                objects.Add(new TestHitObject { StartTime = i });

            return new Beatmap
            {
                BeatmapInfo = new BeatmapInfo
                {
                    Metadata = new BeatmapMetadata
                    {
                        Artist = @"YOASOBI",
                        Source = @"J-Pop",
                        Title = @"あの夢をなぞって",
                    },
                    StarDifficulty = 6,
                    Version = @"Yume",
                    BaseDifficulty = new BeatmapDifficulty(),
                    Length = 1239f,
                    BPM = 727
                },
                HitObjects = objects
            };
        }
        
        private class TestHitObject : ConvertHitObject
        {
        }

        private void testBeatmapLabel()
        {
            AddAssert("check title", () => beatmapDetail.DetailContent.TitleLabel.Text == @"YOASOBI - あの夢をなぞって");
        }
        
        private class TestBeatmapDetail : BeatmapDetail
        {
            public new BeatmapDetailContent DetailContent => base.DetailContent;
        }
    }
}
