using System;
using System.Collections.Generic;
using osu.Framework.Graphics.Containers;
using Tachyon.Game.Graphics;
using Tachyon.Game.Graphics.UserInterface;
using Tachyon.Game.Overlays.Toolbar;
using Tachyon.Game.Testing.Visual;

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
            Add(new Background("Characters/Exusiai_2"));
            
            var toolbar = new Toolbar { State = { Value = Visibility.Visible } };

            AddStep("Create toolbar", () => { Add(toolbar); });
        }
    }
}