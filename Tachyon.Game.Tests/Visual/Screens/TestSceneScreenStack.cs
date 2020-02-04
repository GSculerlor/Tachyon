﻿using NUnit.Framework;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Testing;
using osuTK.Graphics;
using Tachyon.Game.Screens;
using Tachyon.Game.Screens.Backgrounds;
 using Tachyon.Game.Screens.Menu;
 using Tachyon.Game.Testing.Visual;

 namespace Tachyon.Game.Tests.Visual.Screens
{
    [TestFixture]
    public class TestSceneScreenStack : TachyonTestScene
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
            TestTachyonScreen backgroundScreenDefault = null;
            IntroScreen introScreen = null;
            
            
            AddStep("Push default background screen", () => screenStack.Push(backgroundScreenDefault = new TestTachyonScreen("Default Background Screen")));
            AddUntilStep("Wait for current", () => backgroundScreenDefault.IsLoaded);
            AddStep("Push intro screen", () => screenStack.Push(introScreen = new IntroScreen()));
            AddUntilStep("Wait for current", () => introScreen.IsLoaded);
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