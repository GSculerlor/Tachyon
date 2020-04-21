using System.Diagnostics;
using System.Linq;
using osu.Framework.Graphics;
using Tachyon.Game.Rulesets.Scoring;

namespace Tachyon.Game.Rulesets.Objects.Drawables
{
    public abstract class DrawableNote : DrawableTachyonHitObject<Note>
    {
        /// <summary>
        /// A list of keys which can result in hits for this HitObject.
        /// </summary>
        public abstract TachyonAction[] HitActions { get; }

        /// <summary>
        /// The action that caused this <see cref="DrawableNote"/> to be hit.
        /// </summary>
        public TachyonAction? HitAction { get; private set; }

        private bool validActionPressed;

        private bool pressHandledThisFrame;

        protected DrawableNote(Note note)
            : base(note)
        {
            FillMode = FillMode.Fit;
        }

        protected override void CheckForResult(bool userTriggered, double timeOffset)
        {
            Debug.Assert(HitObject.HitWindows != null);

            if (!userTriggered)
            {
                if (!HitObject.HitWindows.CanBeHit(timeOffset))
                    ApplyResult(r => r.Type = HitResult.Miss);
                return;
            }

            var result = HitObject.HitWindows.ResultFor(timeOffset);
            if (result == HitResult.None)
                return;

            if (!validActionPressed)
                ApplyResult(r => r.Type = HitResult.Miss);
            else
                ApplyResult(r => r.Type = result);
        }

        public override bool OnPressed(TachyonAction action)
        {
            if (pressHandledThisFrame)
                return true;

            if (Judged)
                return false;

            validActionPressed = HitActions.Contains(action);

            // Only count this as handled if the new judgement is a hit
            var result = UpdateResult(true);

            if (IsHit)
                HitAction = action;

            // Regardless of whether we've hit or not, any secondary key presses in the same frame should be discarded
            // E.g. hitting a non-strong centre as a strong should not fall through and perform a hit on the next note
            pressHandledThisFrame = true;

            return result;
        }

        public override void OnReleased(TachyonAction action)
        {
            if (action == HitAction)
                HitAction = null;

            base.OnReleased(action);
        }

        protected override void Update()
        {
            base.Update();

            // The input manager processes all input prior to us updating, so this is the perfect time
            // for us to remove the extra press blocking, before input is handled in the next frame
            pressHandledThisFrame = false;

            Size = BaseSize * Parent.RelativeChildSize;
        }

        protected override void UpdateStateTransforms(ArmedState state)
        {
            Debug.Assert(HitObject.HitWindows != null);

            switch (state)
            {
                case ArmedState.Idle:
                    validActionPressed = false;

                    UnproxyContent();
                    break;

                case ArmedState.Miss:
                    this.FadeOut(100);
                    break;

                case ArmedState.Hit:
                    // If we're far enough away from the left stage, we should bring outselves in front of it
                    ProxyContent();

                    const float gravity_time = 300;
                    const float gravity_travel_height = 200;

                    this.ScaleTo(0.8f, gravity_time * 2, Easing.OutQuad);

                    this.MoveToY(-gravity_travel_height, gravity_time, Easing.Out)
                        .Then()
                        .MoveToY(gravity_travel_height * 2, gravity_time * 2, Easing.In);

                    this.FadeOut(800);
                    break;
            }
        }
    }
}
