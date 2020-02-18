// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Framework.Graphics;
using osu.Framework.Graphics.Cursor;
using osu.Framework.Platform;
using osu.Framework.Screens;
using osu.Framework.Testing;
using Tachyon.Game.Graphics;
using Tachyon.Game.Screens.Backgrounds;

namespace Tachyon.Game.Testing
{
    public class TachyonTestBrowser : TachyonGameBase
    {
        protected override void LoadComplete()
        {
            base.LoadComplete();

            LoadComponentAsync(new ScreenStack(new TextureBackgroundScreen { Colour = TachyonColor.Gray(0.5f) })
            {
                Depth = 10,
                RelativeSizeAxes = Axes.Both,
            }, AddInternal);
            
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
