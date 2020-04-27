﻿using JetBrains.Annotations;
using osu.Framework.Allocation;
using osu.Framework.Extensions.Color4Extensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Colour;
using osu.Framework.Graphics.Shapes;
using osuTK.Graphics;
using Tachyon.Game.Graphics;
 using Tachyon.Game.Graphics.Backgrounds;

 namespace Tachyon.Game.Screens.Backgrounds
{
    public class TextureBackgroundScreen : BackgroundScreen
    {
        private Background background;
        private readonly string backgroundName;

        public TextureBackgroundScreen([CanBeNull] string backgroundName = @"Backgrounds/background_unbecoming")
        {
            this.backgroundName = backgroundName;
        }

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