using System;
using osu.Framework.Graphics;
using osuTK;
using Tachyon.Game.Rulesets.UI.Scrolling;

namespace Tachyon.Game.Rulesets.UI
{
    public class TachyonPlayfieldAdjustmentContainer : PlayfieldAdjustmentContainer
    {
        private const float default_relative_height = (Row.DEFAULT_HEIGHT * 2) / 768;
        private const float default_aspect = 16f / 9f;

        public TachyonPlayfieldAdjustmentContainer()
        {
            Anchor = Anchor.CentreLeft;
            Origin = Anchor.CentreLeft;
        }

        protected override void Update()
        {
            base.Update();

            float aspectAdjust = Math.Clamp(Parent.ChildSize.X / Parent.ChildSize.Y, 0.4f, 4) / default_aspect;
            Size = new Vector2(1, default_relative_height * aspectAdjust);
        }
    }
}
