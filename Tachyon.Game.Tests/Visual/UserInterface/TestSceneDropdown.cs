using System.ComponentModel;
using osu.Framework.Graphics;
using Tachyon.Game.Graphics.UserInterface;

namespace Tachyon.Game.Tests.Visual.UserInterface
{
    public class TestSceneDropdown : TachyonTestScene
    {
        public TestSceneDropdown()
        {
            Add(new TachyonEnumDropdown<UserAction>
            {
                Origin = Anchor.Centre,
                Anchor = Anchor.Centre,
                Width = 600
            });
        }
        
        private enum UserAction
        {
            Online,

            [Description(@"Do not disturb")]
            DoNotDisturb,

            [Description(@"Appear offline")]
            AppearOffline,

            [Description(@"Sign out")]
            SignOut,
        }
    }
}
