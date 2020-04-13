using NUnit.Framework;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using Tachyon.Game.Beatmaps;
using Tachyon.Game.Components;
using Tachyon.Game.Overlays.Music;

namespace Tachyon.Game.Tests.Visual.UserInterface
{
    [TestFixture]
    public class TestSceneMusicPlayerOverlay : TachyonTestScene
    {
        [Cached]
        private MusicController musicController = new MusicController();

        private MusicPlayerOverlay musicPlayerOverlay;
        
        [BackgroundDependencyLoader]
        private void load()
        {
            Beatmap.Value = CreateWorkingBeatmap();
            
            musicPlayerOverlay = new MusicPlayerOverlay
            {
                Origin = Anchor.Centre,
                Anchor = Anchor.Centre
            };

            Add(musicController);
            Add(musicPlayerOverlay);
        }
        
        [Test]
        public void TestShowHideDisable()
        {
            AddStep(@"show", () => musicPlayerOverlay.Show());
            AddStep(@"hide", () => musicPlayerOverlay.Hide());
        }
    }
}