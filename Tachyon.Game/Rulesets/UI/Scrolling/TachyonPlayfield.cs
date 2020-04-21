using osu.Framework.Allocation;
using osu.Framework.Extensions.Color4Extensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Effects;
using osu.Framework.Graphics.Shapes;
using osuTK.Graphics;
using Tachyon.Game.Beatmaps.ControlPoints;
using Tachyon.Game.Graphics;
using Tachyon.Game.Rulesets.Judgements;
using Tachyon.Game.Rulesets.Objects.Drawables;
using Tachyon.Game.Rulesets.UI.Components;

namespace Tachyon.Game.Rulesets.UI.Scrolling
{
    public class TachyonPlayfield : ScrollingPlayfield
    {
        /// <summary>
        /// Default height of a <see cref="TachyonPlayfield"/> when inside a <see cref="DrawableTachyonRuleset"/>.
        /// </summary>
        public const float DEFAULT_HEIGHT = 178;

        /// <summary>
        /// The offset from left border which the center of the hit target lies at.
        /// </summary>
        public const float HIT_TARGET_OFFSET = 100;
        
        private readonly JudgementContainer<DrawableTachyonJudgement> judgementContainer;
        private readonly HitTarget hitTarget;

        private readonly ProxyContainer topLevelHitContainer;
        
        private readonly Container backgroundContainer;
        private readonly Box background;

        public TachyonPlayfield(ControlPointInfo controlPoints)
        {
            InternalChildren = new Drawable[]
            {
                backgroundContainer = new Container
                {
                    Name = "Transparent playfield background",
                    RelativeSizeAxes = Axes.Both,
                    Masking = true,
                    EdgeEffect = new EdgeEffectParameters
                    {
                        Type = EdgeEffectType.Shadow,
                        Colour = Color4.Black.Opacity(0.2f),
                        Radius = 5,
                    },
                    Children = new Drawable[]
                    {
                        background = new Box
                        {
                            RelativeSizeAxes = Axes.Both,
                            Alpha = 0.6f
                        },
                    }
                },
                new Container
                {
                    Name = "Right area",
                    RelativeSizeAxes = Axes.Both,
                    Children = new Drawable[]
                    {
                        new Container
                        {
                            Name = "Masked elements before hit objects",
                            RelativeSizeAxes = Axes.Both,
                            Padding = new MarginPadding { Left = HIT_TARGET_OFFSET },
                            Masking = true,
                            Children = new Drawable[]
                            {
                                hitTarget = new HitTarget
                                {
                                    Anchor = Anchor.CentreLeft,
                                    Origin = Anchor.Centre,
                                    RelativeSizeAxes = Axes.Both,
                                    FillMode = FillMode.Fit
                                }
                            }
                        },
                        new Container
                        {
                            Name = "Hit objects",
                            RelativeSizeAxes = Axes.Both,
                            Padding = new MarginPadding { Left = HIT_TARGET_OFFSET },
                            Masking = true,
                            Child = HitObjectContainer
                        },
                        judgementContainer = new JudgementContainer<DrawableTachyonJudgement>
                        {
                            Name = "Judgements",
                            RelativeSizeAxes = Axes.Y,
                            Margin = new MarginPadding { Left = HIT_TARGET_OFFSET },
                            Blending = BlendingParameters.Additive
                        },
                    }
                },
                topLevelHitContainer = new ProxyContainer
                {
                    Name = "Top level hit objects",
                    RelativeSizeAxes = Axes.Both,
                }
            };
        }
        
        [BackgroundDependencyLoader]
        private void load(TachyonColor colors)
        {
            backgroundContainer.BorderColour = colors.Gray1;
            background.Colour = colors.Gray0;
        }
        
        public override void Add(DrawableHitObject h)
        {
            h.OnNewResult += OnNewResult;

            base.Add(h);

            switch (h)
            {
                case DrawableTachyonHitObject taikoObject:
                    topLevelHitContainer.Add(taikoObject.CreateProxiedContent());
                    break;
            }
        }

        internal void OnNewResult(DrawableHitObject judgedObject, JudgementResult result)
        {
            if (!DisplayJudgements.Value)
                return;

            if (!judgedObject.DisplayResult)
                return;

            judgementContainer.Add(new DrawableTachyonJudgement(result, judgedObject)
            {
                Anchor = result.IsHit ? Anchor.TopLeft : Anchor.CentreLeft,
                Origin = result.IsHit ? Anchor.BottomCentre : Anchor.Centre,
                RelativePositionAxes = Axes.X,
                X = result.IsHit ? judgedObject.Position.X : 0,
            });
        }

        private class ProxyContainer : LifetimeManagementContainer
        {
            public new MarginPadding Padding
            {
                set => base.Padding = value;
            }

            public void Add(Drawable proxy) => AddInternal(proxy);
        }
    }
}
