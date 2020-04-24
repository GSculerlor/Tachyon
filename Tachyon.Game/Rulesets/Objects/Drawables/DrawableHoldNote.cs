using System;
using System.Linq;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osuTK.Graphics;
using Tachyon.Game.Graphics;
using Tachyon.Game.Rulesets.Judgements;
using Tachyon.Game.Rulesets.Objects.Drawables.Pieces;
using Tachyon.Game.Rulesets.Scoring;

namespace Tachyon.Game.Rulesets.Objects.Drawables
{
    public class DrawableHoldNote : DrawableTachyonHitObject<HoldNote>
    {
        /// <summary>
        /// Number of rolling hits required to reach the dark/final color.
        /// </summary>
        private const int rolling_hits_for_engaged_color = 5;

        /// <summary>
        /// Rolling number of tick hits. This increases for hits and decreases for misses.
        /// </summary>
        private int rollingHits;

        private readonly Container<DrawableHoldNoteTick> tickContainer;

        private Color4 colourIdle;
        private Color4 colourEngaged;

        private ElongatedCirclePiece elongatedPiece;
        
        public DrawableHoldNote(HoldNote hitObject)
            : base(hitObject)
        {
            RelativeSizeAxes = Axes.Y;
            elongatedPiece.Add(tickContainer = new Container<DrawableHoldNoteTick> { RelativeSizeAxes = Axes.Both });
        }
        
        [BackgroundDependencyLoader]
        private void load(TachyonColor colors)
        {
            elongatedPiece.AccentColor = colourIdle = colors.YellowDark;
            colourEngaged = colors.YellowDarker;
        }

        protected override void LoadComplete()
        {
            base.LoadComplete();

            OnNewResult += onNewResult;
        }

        protected override void AddNestedHitObject(DrawableHitObject hitObject)
        {
            base.AddNestedHitObject(hitObject);

            switch (hitObject)
            {
                case DrawableHoldNoteTick tick:
                    tickContainer.Add(tick);
                    break;
            }
        }

        protected override void ClearNestedHitObjects()
        {
            base.ClearNestedHitObjects();
            tickContainer.Clear();
        }

        protected override DrawableHitObject CreateNestedHitObject(HitObject hitObject)
        {
            switch (hitObject)
            {
                case HoldNoteTick tick:
                    return new DrawableHoldNoteTick(tick);
            }

            return base.CreateNestedHitObject(hitObject);
        }

        protected override CompositeDrawable CreateMainPiece() => elongatedPiece = new ElongatedCirclePiece();

        public override bool OnPressed(TachyonAction action) => false;

        private void onNewResult(DrawableHitObject obj, JudgementResult result)
        {
            if (!(obj is DrawableHoldNoteTick))
                return;

            if (result.Type > HitResult.Miss)
                rollingHits++;
            else
                rollingHits--;

            rollingHits = Math.Clamp(rollingHits, 0, rolling_hits_for_engaged_color);
        }

        protected override void CheckForResult(bool userTriggered, double timeOffset)
        {
            if (userTriggered)
                return;

            if (timeOffset < 0)
                return;

            int countHit = NestedHitObjects.Count(o => o.IsHit);
            if (countHit >= HitObject.RequiredGoodHits)
                ApplyResult(r => r.Type = HitResult.Good);
            else
                ApplyResult(r => r.Type = HitResult.Miss);
        }

        protected override void UpdateStateTransforms(ArmedState state)
        {
            switch (state)
            {
                case ArmedState.Hit:
                case ArmedState.Miss:
                    this.Delay(HitObject.Duration).FadeOut(100);
                    break;
            }
        }
    }
}
