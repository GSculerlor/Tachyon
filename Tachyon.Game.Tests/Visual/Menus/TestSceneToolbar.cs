using System;
using System.Collections.Generic;
using osu.Framework.Graphics.Containers;
using Tachyon.Game.Graphics.UserInterface;
using Tachyon.Game.Overlays;

namespace Tachyon.Game.Tests.Visual.Menus
{
    public class TestSceneToolbar : TachyonTestScene
    {
        public override IReadOnlyList<Type> RequiredTypes => new[]
        {
            typeof(ToolbarButton),
            typeof(Toolbar)
        };

        public TestSceneToolbar()
        {
            var toolbar = new Toolbar { State = { Value = Visibility.Visible } };

            AddStep("Create toolbar", () => { Add(toolbar); });
        }
    }
}