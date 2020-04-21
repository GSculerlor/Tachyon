using osu.Framework.Graphics;
using osu.Framework.Graphics.Cursor;
using osu.Framework.Platform;
using osu.Framework.Screens;
using osu.Framework.Testing;
using Tachyon.Game.Graphics;
using Tachyon.Game.Screens.Backgrounds;

namespace Tachyon.Game.Tests
{
    public class TachyonTestBrowser : TachyonGameBase
    {
        protected override void LoadComplete()
        {
            base.LoadComplete();
            
            AddRange(new Drawable[]
            {
                new TestBrowser("Tachyon"),
                new CursorContainer()
            });
        }

        public override void SetHost(GameHost host)
        {
            base.SetHost(host);
            host.Window.CursorState |= CursorState.Hidden;
        }
    }
}
