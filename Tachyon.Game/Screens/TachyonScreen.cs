﻿using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Screens;

 namespace Tachyon.Game.Screens
{
    public abstract class TachyonScreen : Screen, ITachyonScreen
    {
        public virtual bool AllowBackButton => true;
        
        public virtual bool ToolbarVisible => true;
        
        public bool AllowExternalScreenChange => false;

        public bool CursorVisible => true;

        protected BackgroundScreen Background => backgroundStack?.CurrentScreen as BackgroundScreen;

        private BackgroundScreen localBackground;

        [Resolved(canBeNull: true)]
        private BackgroundScreenStack backgroundStack { get; set; }
        
        protected TachyonScreen()
        {
            Anchor = Anchor.Centre;
            Origin = Anchor.Centre;
        }
        
        public override void OnEntering(IScreen last)
        {
            backgroundStack?.Push(localBackground = CreateBackground());
            base.OnEntering(last);
        }

        public override bool OnExiting(IScreen next)
        {
            if (base.OnExiting(next))
                return true;

            if (localBackground != null && backgroundStack?.CurrentScreen == localBackground)
                backgroundStack?.Exit();

            return false;
        }
        
        protected virtual BackgroundScreen CreateBackground() => null;
    }
}