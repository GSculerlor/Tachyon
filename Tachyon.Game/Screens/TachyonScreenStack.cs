﻿using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Screens;

namespace Tachyon.Game.Screens
{
    public class TachyonScreenStack : ScreenStack
    {
        [Cached]
        private BackgroundScreenStack backgroundScreenStack;

        private Container childContainer;
        
        public TachyonScreenStack()
        {
            initializeStack();
        }
        
        public TachyonScreenStack(IScreen baseScreen)
            : base(baseScreen)
        {
            initializeStack();
        }
        
        private void initializeStack()
        {
            InternalChild = childContainer = new Container
            {
                RelativeSizeAxes = Axes.Both,
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                Child = backgroundScreenStack = new BackgroundScreenStack { RelativeSizeAxes = Axes.Both },
            };

            ScreenPushed += onScreenChange;
            ScreenExited += onScreenChange;
        }

        private void onScreenChange(IScreen prev, IScreen next)
        {
            
        }
    }
}