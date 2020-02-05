using System;
using System.Collections.Generic;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using Tachyon.Game.Graphics;
using Tachyon.Game.Graphics.UserInterface;
using Tachyon.Game.Testing.Visual;

namespace Tachyon.Game.Tests.Visual.UserInterface
{
    public class TestSceneBackButton : TachyonTestScene
    {
        public override IReadOnlyList<Type> RequiredTypes => new[]
        {
            typeof(IconButton)
        };

        public TestSceneBackButton()
        {
            BackButton button;
            
            Child = new Container
            {
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                RelativeSizeAxes = Axes.Both,
                Masking = true,
                Children = new Drawable[]
                {
                    new Background("Characters/Exusiai_1")
                    {
                        RelativeSizeAxes = Axes.Both,
                    },
                    button = new BackButton
                    {
                        Anchor = Anchor.TopLeft,
                        Origin = Anchor.TopLeft,
                    }
                }
            };

            button.Action = () => button.Hide();
            
            AddStep("show button", () => button.Show());
            AddStep("hide button", () => button.Hide());
        }
    }
}