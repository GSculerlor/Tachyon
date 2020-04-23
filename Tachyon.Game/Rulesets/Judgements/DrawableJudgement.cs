using osu.Framework.Allocation;
using osu.Framework.Extensions;
using osu.Framework.Extensions.Color4Extensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Colour;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osuTK;
using osuTK.Graphics;
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
        protected readonly JudgementResult Result;

        public readonly DrawableHitObject JudgedObject;

        protected Container JudgementBody;

        /// <summary>
        /// Duration of initial fade in.
        /// </summary>
        private double fadeInDuration => 100;

        /// <summary>
        /// Duration to wait until fade out begins. Defaults to <see cref="fadeInDuration"/>.
        /// </summary>
        private double fadeOutDelay => fadeInDuration;

        /// <summary>
        /// Creates a drawable which visualises a <see cref="Judgements.Judgement"/>.
        /// </summary>
        /// <param name="result">The judgement to visualise.</param>
        /// <param name="judgedObject">The object which was judged.</param>
        public DrawableJudgement(JudgementResult result, DrawableHitObject judgedObject)
        {
            Result = result;
            JudgedObject = judgedObject;
            RelativeSizeAxes = Axes.Both;
        }

        [BackgroundDependencyLoader]
        private void load(TachyonColor colors)
        {
            InternalChild = JudgementBody = new Container
            {
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                RelativeSizeAxes = Axes.Both,
                Child = new FillFlowContainer
                {
                    RelativeSizeAxes = Axes.Both,
                    Direction = FillDirection.Horizontal,
                    Children = new[]
                    {
                        new Box
                        {
                            RelativeSizeAxes = Axes.Both,
                            Colour = colors.ForHitResult(Result.Type).Opacity(0.5f),
                            Width = 0.2f,
                        },
                        new Box
                        {
                            RelativeSizeAxes = Axes.Both,
                            Colour = ColourInfo.GradientHorizontal(colors.ForHitResult(Result.Type).Opacity(0.5f), colors.ForHitResult(Result.Type).Opacity(0.2f)),
                            Width = 0.2f,
                        },
                        new Box
                        {
                            RelativeSizeAxes = Axes.Both,
                            Colour = ColourInfo.GradientHorizontal(colors.ForHitResult(Result.Type).Opacity(0.2f), colors.ForHitResult(Result.Type).Opacity(0.1f)),
                            Width = 0.2f,
                        },
                        new Box
                        {
                            RelativeSizeAxes = Axes.Both,
                            Colour = ColourInfo.GradientHorizontal(colors.ForHitResult(Result.Type).Opacity(0.1f), colors.ForHitResult(Result.Type).Opacity(0f)),
                            Width = 0.2f,
                        },
                    }
                },
            };
        }

        protected virtual void ApplyHitAnimations()
        {
            JudgementBody.FadeInFromZero(400, Easing.OutElastic);

            this.Delay(fadeOutDelay).FadeOut(400);
        }

        protected override void LoadComplete()
        {
            base.LoadComplete();

            this.FadeInFromZero(fadeInDuration, Easing.OutQuint);

            switch (Result.Type)
            {
                case HitResult.None:
                    break;
                
                default:
                    ApplyHitAnimations();
                    break;
            }

            Expire(true);
        }
    }
}
