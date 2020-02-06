﻿using NUnit.Framework;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Testing;
using osuTK.Graphics;
 using Tachyon.Game.Screens;
using Tachyon.Game.Screens.Backgrounds;
 using Tachyon.Game.Screens.Menu;
 using Tachyon.Game.Screens.Placeholder;
 using Tachyon.Game.Testing.Visual;

 namespace Tachyon.Game.Tests.Visual.Screens
{
    [TestFixture]
    public class TestSceneScreenStack : TachyonTestScene
    {
        private TachyonScreenStack screenStack;
        
        [SetUpSteps]
        public void SetUpSteps()
        {
            AddStep("Create new screen stack", () =>
            {
                Child = screenStack = new TachyonScreenStack { RelativeSizeAxes = Axes.Both };
            });
        }

        [Test]
        public void PushScreenTest()
        {
            PlaceholderScreen placeholderScreen = null;
            IntroScreen introScreen = null;

            AddStep("Push default background screen", () => screenStack.Push(placeholderScreen = new PlaceholderScreen("Placeholder for screen stack")));
            AddUntilStep("Wait for current", () => placeholderScreen.IsLoaded);
            AddStep("Push intro screen", () => screenStack.Push(introScreen = new IntroScreen()));
            AddUntilStep("Wait for current", () => introScreen.IsLoaded);
        }
    }
}