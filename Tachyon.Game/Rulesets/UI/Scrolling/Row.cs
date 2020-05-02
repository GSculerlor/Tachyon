using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using Tachyon.Game.Graphics;
using Tachyon.Game.Rulesets.Judgements;
using Tachyon.Game.Rulesets.Objects.Drawables;
using Tachyon.Game.Rulesets.UI.Components;

namespace Tachyon.Game.Rulesets.UI.Scrolling
{
    [Cached]
    public class Row : ScrollingPlayfield
    {
        public const float DEFAULT_HEIGHT = 200;
        public const float HIT_TARGET_OFFSET = 100;
        public readonly int Index;
        
        private readonly JudgementContainer<DrawableTachyonJudgement> judgementContainer;

        private readonly ProxyContainer topLevelHitContainer;
        
        private readonly Container backgroundContainer;
        private readonly Box background;

        public Row(int index)
        {
            Index = index;

            RelativeSizeAxes = Axes.X;
            Height = DEFAULT_HEIGHT;
            
            InternalChildren = new Drawable[]
            {
                backgroundContainer = new Container
                {
                    Name = "Transparent playfield background",
                    RelativeSizeAxes = Axes.Both,
                    Masking = true,
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
                                new HitTarget
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
                            Origin = Anchor.CentreLeft,
                            Anchor = Anchor.CentreLeft,
                            RelativeSizeAxes = Axes.Y,
                            Width = HIT_TARGET_OFFSET,
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
        
        public override bool Remove(DrawableHitObject h)
        {
            if (!base.Remove(h))
                return false;

            h.OnNewResult -= OnNewResult;
            return true;
        }
        
        internal void OnNewResult(DrawableHitObject judgedObject, JudgementResult result)
        {
            if (!DisplayJudgements.Value)
                return;

            if (!judgedObject.DisplayResult)
                return;

            judgementContainer.Add(new DrawableTachyonJudgement(result, judgedObject)
            {
                Anchor = Anchor.CentreLeft,
                Origin = Anchor.CentreLeft,
                RelativePositionAxes = Axes.Both,
            });
        }
        
        private class ProxyContainer : LifetimeManagementContainer
        {
            public void Add(Drawable proxy) => AddInternal(proxy);
        }
    }
}
