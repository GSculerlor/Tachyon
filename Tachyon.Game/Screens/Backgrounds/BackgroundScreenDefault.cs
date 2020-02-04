﻿using osu.Framework.Allocation;
using osu.Framework.Extensions.Color4Extensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Colour;
using osu.Framework.Graphics.Shapes;
using osuTK.Graphics;
using Tachyon.Game.Graphics;

namespace Tachyon.Game.Screens.Backgrounds
{
    public class BackgroundScreenDefault : BackgroundScreen
    {
        private Background background;
        
        private static string backgroundName => @"Characters/Exusiai_1";

        [BackgroundDependencyLoader]
        private void load()
        {
            display(new Background(backgroundName));
            AddInternal(new Box
            {
                RelativeSizeAxes = Axes.Both,
                Colour = ColourInfo.GradientVertical(Color4.Black.Opacity(0.1f), Color4.Black.Opacity(0.75f))
            });
        }
        
        private void display(Background newBackground)
        {
            background?.FadeOut(800, Easing.InOutSine);
            background?.Expire();

            AddInternal(background = newBackground);
        }
    }
}