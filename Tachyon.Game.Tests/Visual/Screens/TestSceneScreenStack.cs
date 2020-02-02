﻿using NUnit.Framework;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Testing;
using osuTK.Graphics;
using Tachyon.Game.Screens;
using Tachyon.Game.Screens.Backgrounds;

namespace Tachyon.Game.Tests.Visual.Screens
{
    [TestFixture]
    public class TestSceneScreenStack : TestScene
    {
        private TestTachyonScreenStack screenStack;
        
        [SetUpSteps]
        public void SetUpSteps()
        {
            AddStep("Create new screen stack", () =>
            {
                Child = screenStack = new TestTachyonScreenStack { RelativeSizeAxes = Axes.Both };
            });
        }

        [Test]
        public void PushScreenTest()
        {
            TestTachyonScreen screen = null;
            
            AddStep("Push default background screen", () => screenStack.Push(screen = new TestTachyonScreen("Default Background Screen")));
            AddUntilStep("Wait for current", () => screen.IsLoaded);
        }

        private class TestTachyonScreen : BackgroundScreenDefault
        {
            private readonly string screenText;
            
            public TestTachyonScreen(string screenText)
            {
                this.screenText = screenText;
            }
            
            [BackgroundDependencyLoader]
            private void load()
            {
                AddInternal(new SpriteText
                {
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre,
                    Text = screenText,
                    Colour = Color4.White
                });
            }
        }
        
        private class TestTachyonScreenStack : TachyonScreenStack
        {
            
        }
    }
}