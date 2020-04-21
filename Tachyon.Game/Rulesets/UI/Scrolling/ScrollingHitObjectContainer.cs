using System.Collections.Generic;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Caching;
using osu.Framework.Graphics;
using osu.Framework.Layout;
using Tachyon.Game.Rulesets.Objects.Drawables;
using Tachyon.Game.Rulesets.Objects.Types;

namespace Tachyon.Game.Rulesets.UI.Scrolling
{
    public class ScrollingHitObjectContainer : HitObjectContainer
    {
        private readonly IBindable<double> timeRange = new BindableDouble();
        
        [Resolved]
        private IScrollingInfo scrollingInfo { get; set; }
        
        private readonly LayoutValue initialStateCache = new LayoutValue(Invalidation.RequiredParentSizeToFit | Invalidation.DrawInfo);
        
        public ScrollingHitObjectContainer()
        {
            RelativeSizeAxes = Axes.Both;

            AddLayout(initialStateCache);
        }
        
        [BackgroundDependencyLoader]
        private void load()
        {
            timeRange.BindTo(scrollingInfo.TimeRange);
            timeRange.ValueChanged += _ => initialStateCache.Invalidate();
        }
        public override void Add(DrawableHitObject hitObject)
        {
            initialStateCache.Invalidate();
            base.Add(hitObject);
        }
        
        private readonly Dictionary<DrawableHitObject, Cached> hitObjectInitialStateCache = new Dictionary<DrawableHitObject, Cached>();
        
        public override bool Remove(DrawableHitObject hitObject)
        {
            var result = base.Remove(hitObject);

            if (result)
            {
                initialStateCache.Invalidate();
                hitObjectInitialStateCache.Remove(hitObject);
            }

            return result;
        }
        
        private float scrollLength;
        
        protected override void Update()
        {
            base.Update();

            if (!initialStateCache.IsValid)
            {
                foreach (var cached in hitObjectInitialStateCache.Values)
                    cached.Invalidate();

                scrollLength = DrawSize.X;

                scrollingInfo.Algorithm.Reset();

                foreach (var obj in Objects)
                {
                    computeLifetimeStartRecursive(obj);
                    computeInitialStateRecursive(obj);
                }

                initialStateCache.Validate();
            }
        }
        
        private void computeLifetimeStartRecursive(DrawableHitObject hitObject)
        {
            hitObject.LifetimeStart = computeOriginAdjustedLifetimeStart(hitObject);

            foreach (var obj in hitObject.NestedHitObjects)
                computeLifetimeStartRecursive(obj);
        }
        
        private double computeOriginAdjustedLifetimeStart(DrawableHitObject hitObject)
        {
            float originAdjustment = hitObject.OriginPosition.X;

            return scrollingInfo.Algorithm.GetDisplayStartTime(hitObject.HitObject.StartTime, originAdjustment, timeRange.Value, scrollLength);
        }
        
        private void computeInitialStateRecursive(DrawableHitObject hitObject) => hitObject.Schedule(() =>
        {
            if (!hitObjectInitialStateCache.TryGetValue(hitObject, out var cached))
                cached = hitObjectInitialStateCache[hitObject] = new Cached();

            if (cached.IsValid)
                return;

            if (hitObject.HitObject is IHasEndTime e)
                hitObject.Width = scrollingInfo.Algorithm.GetLength(hitObject.HitObject.StartTime, e.EndTime, timeRange.Value, scrollLength);

            foreach (var obj in hitObject.NestedHitObjects)
            {
                computeInitialStateRecursive(obj);

                // Nested hitobjects don't need to scroll, but they do need accurate positions
                updatePosition(obj, hitObject.HitObject.StartTime);
            }

            cached.Validate();
        });

        protected override void UpdateAfterChildrenLife()
        {
            base.UpdateAfterChildrenLife();

            // We need to calculate hitobject positions as soon as possible after lifetimes so that hitobjects get the final say in their positions
            foreach (var obj in AliveObjects)
                updatePosition(obj, Time.Current);
        }

        private void updatePosition(DrawableHitObject hitObject, double currentTime)
        {
            hitObject.X = scrollingInfo.Algorithm.PositionAt(hitObject.HitObject.StartTime, currentTime, timeRange.Value, scrollLength);
        }
    }
}
