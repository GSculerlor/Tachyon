﻿using System;
using osu.Framework.Graphics;
using osu.Framework.Input.Events;
using osu.Framework.Screens;

namespace Tachyon.Game.Screens
{
    public abstract class BackgroundScreen : Screen, IEquatable<BackgroundScreen>
    {
        protected BackgroundScreen()
        {
            Anchor = Anchor.Centre;
            Origin = Anchor.Centre;
        }
        
        protected override bool OnKeyDown(KeyDownEvent e)
        {
            return false;
        }
        
        public bool Equals(BackgroundScreen other)
        {
            return other?.GetType() == GetType();
        }
        
        public override void OnEntering(IScreen last)
        {
            this.FadeOut();

            this.FadeIn(400, Easing.OutQuint);
            
            base.OnEntering(last);
        }


        public override bool OnExiting(IScreen next)
        {
            this.FadeOut(400, Easing.OutQuint);

            return base.OnExiting(next);
        }
    }
}