using System.ComponentModel;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Shapes;
using osuTK;
using osuTK.Graphics;
using Tachyon.Game.Graphics.UserInterface;

namespace Tachyon.Game.Tests.Visual.UserInterface
{
    public class TestSceneTabControl : TachyonTestScene
    {
        public TestSceneTabControl()
        {
            TachyonTabControl<GroupMode> tab;
            AddRange(new Drawable[]
            {
                tab = new TachyonTabControl<GroupMode>
                {
                    Margin = new MarginPadding(4),
                    Size = new Vector2(240, 26),
                    AutoSort = true,
                    Spacing = new Vector2(20f, 0)
                },
            });
        }
    }

    public enum GroupMode
    {
        [Description("Home")]
        Home,

        [Description("Editor")]
        Editor,

        [Description("Settings")]
        Settings,
    }
}
