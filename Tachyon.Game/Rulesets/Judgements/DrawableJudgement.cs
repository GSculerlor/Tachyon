using osu.Framework.Allocation;
using osu.Framework.Extensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Sprites;
using osuTK;
using Tachyon.Game.Graphics;
using Tachyon.Game.Graphics.Sprites;
using Tachyon.Game.Rulesets.Objects.Drawables;
using Tachyon.Game.Rulesets.Scoring;

namespace Tachyon.Game.Rulesets.Judgements
{
    /// <summary>
    /// A drawable object which visualises the hit result of a <see cref="Judgements.Judgement"/>.
    /// </summary>
    public class DrawableJudgement : CompositeDrawable
    {
        private const float judgement_size = 128;

        protected readonly JudgementResult Result;

        public readonly DrawableHitObject JudgedObject;

        protected Container JudgementBody;
        protected SpriteText JudgementText;

        /// <summary>
        /// Duration of initial fade in.
        /// </summary>
        protected virtual double FadeInDuration => 100;

        /// <summary>
        /// Duration to wait until fade out begins. Defaults to <see cref="FadeInDuration"/>.
        /// </summary>
        protected virtual double FadeOutDelay => FadeInDuration;

        /// <summary>
        /// Creates a drawable which visualises a <see cref="Judgements.Judgement"/>.
        /// </summary>
        /// <param name="result">The judgement to visualise.</param>
        /// <param name="judgedObject">The object which was judged.</param>
        public DrawableJudgement(JudgementResult result, DrawableHitObject judgedObject)
        {
            Result = result;
            JudgedObject = judgedObject;

            Size = new Vector2(judgement_size);
        }

        [BackgroundDependencyLoader]
        private void load(TachyonColor colors)
        {
            InternalChild = JudgementBody = new Container
            {
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                RelativeSizeAxes = Axes.Both,
                Child = new TachyonSpriteText
                {
                    Text = Result.Type.GetDescription().ToUpperInvariant(),
                    Font = TachyonFont.Numeric.With(size: 20),
                    Colour = colors.ForHitResult(Result.Type),
                    Scale = new Vector2(0.85f, 1),
                }
            };
        }

        protected virtual void ApplyHitAnimations()
        {
            JudgementBody.ScaleTo(0.9f);
            JudgementBody.ScaleTo(1, 500, Easing.OutElastic);

            this.Delay(FadeOutDelay).FadeOut(400);
        }

        protected override void LoadComplete()
        {
            base.LoadComplete();

            this.FadeInFromZero(FadeInDuration, Easing.OutQuint);

            switch (Result.Type)
            {
                case HitResult.None:
                    break;

                case HitResult.Miss:
                    JudgementBody.ScaleTo(1.6f);
                    JudgementBody.ScaleTo(1, 100, Easing.In);

                    JudgementBody.MoveToOffset(new Vector2(0, 100), 800, Easing.InQuint);
                    JudgementBody.RotateTo(40, 800, Easing.InQuint);

                    this.Delay(600).FadeOut(200);
                    break;

                default:
                    ApplyHitAnimations();
                    break;
            }

            Expire(true);
        }
    }
}
