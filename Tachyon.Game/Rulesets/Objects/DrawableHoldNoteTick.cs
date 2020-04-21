using System;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using Tachyon.Game.Rulesets.Objects.Drawables;
using Tachyon.Game.Rulesets.Objects.Drawables.Pieces;
using Tachyon.Game.Rulesets.Scoring;

namespace Tachyon.Game.Rulesets.Objects
{
    public class DrawableHoldNoteTick  : DrawableTachyonHitObject<HoldNoteTick>
    {
        public DrawableHoldNoteTick(HoldNoteTick hitObject)
            : base(hitObject)
        {
            FillMode = FillMode.Fit;
        }

        public override bool DisplayResult => false;

        protected override CompositeDrawable CreateMainPiece() => new HoldNoteTickPiece
        {
            Filled = HitObject.FirstTick
        };

        protected override void CheckForResult(bool userTriggered, double timeOffset)
        {
            if (!userTriggered)
            {
                if (timeOffset > HitObject.HitWindow)
                    ApplyResult(r => r.Type = HitResult.Miss);
                return;
            }

            if (Math.Abs(timeOffset) > HitObject.HitWindow)
                return;

            ApplyResult(r => r.Type = HitResult.Perfect);
        }

        protected override void UpdateStateTransforms(ArmedState state)
        {
            switch (state)
            {
                case ArmedState.Hit:
                    this.ScaleTo(0, 100, Easing.OutQuint);
                    break;
            }
        }

        public override bool OnPressed(TachyonAction action) => UpdateResult(true);
    }
}
