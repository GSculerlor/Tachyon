using System;
using System.Collections.Generic;
using NUnit.Framework;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using Tachyon.Game.Components;
using Tachyon.Game.Overlays;

namespace Tachyon.Game.Tests.Visual.UserInterface
{
    [TestFixture]
    public class TestSceneMusicPlayer : TachyonTestScene
    {
        public override IReadOnlyList<Type> RequiredTypes => new[]
        {
            typeof(MusicPlayer)
        };

        [Cached]
        private MusicController musicController = new MusicController();

        private MusicPlayer musicPlayer;

        [BackgroundDependencyLoader]
        private void load()
        {
            musicPlayer = new MusicPlayer
            {
                Origin = Anchor.Centre,
                Anchor = Anchor.Centre
            };
            
            Add(musicController);
            Add(musicPlayer);
        }
        
        [Test]
        public void TestShowHide()
        {
            AddStep(@"show", () => musicPlayer.Show());
            AddStep(@"hide", () => musicPlayer.Hide());
        }
    }
}