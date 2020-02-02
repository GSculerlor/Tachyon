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
    }
}