// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osuTK;
using osuTK.Graphics;
using Tachyon.Game.Graphics;

namespace Tachyon.Game
{
    public class TachyonGame : TachyonGameBase
    {
        private Box box;

        [BackgroundDependencyLoader]
        private void load()
        {
            // Add your game components here.
            // The rotating box can be removed.

            Children = new Drawable[]
            {
                box = new Box
                {
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre,
                    Colour = Color4.Orange,
                    Size = new Vector2(200),
                },
                new SpriteText
                {
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre,
                    Y = 210,
                    Text = "Bjir muter",
                    Font = TachyonFont.GetFont(size: 20)
                }
            };
        }

        protected override void LoadComplete()
        {
            base.LoadComplete();

            box.Loop(b => b.RotateTo(0).RotateTo(360, 2500));
        }
    }
}
