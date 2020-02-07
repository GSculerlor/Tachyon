using System;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osuTK;

namespace Tachyon.Game.Graphics.Containers
{
    public class TachyonIconContainer : CompositeDrawable
    {
        public Drawable Icon
        {
            get => InternalChild;
            set => InternalChild = value;
        }

        public TachyonIconContainer()
        {
            Masking = true;
        }

        protected override void Update()
        {
            base.Update();

            if (InternalChildren.Count <= 0 || !(InternalChild.DrawSize.X > 0)) return;
            
            var scale = Math.Min(DrawSize.X / InternalChild.DrawSize.X, DrawSize.Y / InternalChild.DrawSize.Y);
            InternalChild.Scale = new Vector2(scale);
            InternalChild.Anchor = Anchor.Centre;
            InternalChild.Origin = Anchor.Centre;
        }
    }
}