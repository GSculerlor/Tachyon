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
            InternalChild = new Container
            {
                RelativeSizeAxes = Axes.Both,
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                Child = backgroundScreenStack = new BackgroundScreenStack { RelativeSizeAxes = Axes.Both },
            };

            ScreenPushed += screenPushed;
            ScreenExited += onScreenChange;
        }
        
        private void screenPushed(IScreen prev, IScreen next)
        {
            if (LoadState < LoadState.Ready)
            {
                Schedule(() => screenPushed(prev, next));
                return;
            }

            ((TachyonScreen) next).CreateLeasedDependencies((prev as TachyonScreen)?.Dependencies ?? Dependencies);

            onScreenChange(prev, next);
        }

        private void onScreenChange(IScreen prev, IScreen next)
        {
            
        }
    }
}