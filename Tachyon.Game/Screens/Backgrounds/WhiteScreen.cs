﻿using osu.Framework.Graphics;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Screens;
using osuTK.Graphics;

namespace Tachyon.Game.Screens.Backgrounds
{
    public class WhiteScreen : BackgroundScreen
    {
        public WhiteScreen()
        {
            InternalChild = new Box
            {
                Colour = Color4.White,
                RelativeSizeAxes = Axes.Both,
            };
        }

        public override void OnEntering(IScreen last)
        {
            Show();
        }
    }
}