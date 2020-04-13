 using System;
 using System.Collections.Generic;
 using System.Diagnostics;
 using System.Linq;
 using osu.Framework.Allocation;
 using osu.Framework.Bindables;
 using osu.Framework.Caching;
 using osu.Framework.Extensions.IEnumerableExtensions;
 using osu.Framework.Graphics;
 using osu.Framework.Graphics.Containers;
 using osu.Framework.Input.Bindings;
 using osu.Framework.Input.Events;
 using osu.Framework.Threading;
 using osu.Framework.Utils;
 using osuTK;
 using osuTK.Input;
 using Tachyon.Game.Beatmaps;
 using Tachyon.Game.Graphics.Containers;
 using Tachyon.Game.Screens.Playground.Carousel;

 namespace Tachyon.Game.Screens.Playground
{
    public class BeatmapCarousel : CompositeDrawable
    {
        private const float bleed_top = 100;
        private const float bleed_bottom = 50;
        
        public Action BeatmapSetsChanged;

        public BeatmapInfo SelectedBeatmap => selectedBeatmap?.Beatmap;

        private CarouselBeatmap selectedBeatmap => selectedBeatmapSet?.Beatmaps.FirstOrDefault(s => s.State.Value == CarouselItemState.Selected);

        public BeatmapSetInfo SelectedBeatmapSet => selectedBeatmapSet?.BeatmapSet;

        private CarouselBeatmapSet selectedBeatmapSet;

        public Action<BeatmapInfo> SelectionChanged;

        public override bool HandleNonPositionalInput => AllowSelection;
        public override bool HandlePositionalInput => AllowSelection;

        public override bool PropagatePositionalInputSubTree => AllowSelection;
        public override bool PropagateNonPositionalInputSubTree => AllowSelection;

        public bool BeatmapSetsLoaded { get; private set; }

        private readonly CarouselScrollContainer scroll;

        private IEnumerable<CarouselBeatmapSet> beatmapSets => root.Children.OfType<CarouselBeatmapSet>();

        public IEnumerable<BeatmapSetInfo> BeatmapSets
        {
            get => beatmapSets.Select(g => g.BeatmapSet);
            set => loadBeatmapSets(value);
        }

        private void loadBeatmapSets(IEnumerable<BeatmapSetInfo> beatmapSets)
        {
            CarouselRoot newRoot = new CarouselRoot(this);

            beatmapSets.Select(createCarouselSet).Where(g => g != null).ForEach(newRoot.AddChild);

            _ = newRoot.Drawables;

            root = newRoot;
            if (selectedBeatmapSet != null && !beatmapSets.Contains(selectedBeatmapSet.BeatmapSet))
                selectedBeatmapSet = null;

            scrollableContent.Clear(false);
            itemsCache.Invalidate();
            scrollPositionCache.Invalidate();

            SchedulerAfterChildren.Add(() =>
            {
                BeatmapSetsChanged?.Invoke();
                BeatmapSetsLoaded = true;
            });
        }

        private readonly List<float> yPositions = new List<float>();
        private readonly Cached itemsCache = new Cached();
        private readonly Cached scrollPositionCache = new Cached();

        private readonly Container<DrawableCarouselItem> scrollableContent;

        protected List<DrawableCarouselItem> Items = new List<DrawableCarouselItem>();
        private CarouselRoot root;

        public BeatmapCarousel()
        {
            root = new CarouselRoot(this);
            InternalChild = new Container
            {
                RelativeSizeAxes = Axes.Both,
                Child = scroll = new CarouselScrollContainer
                {
                    Masking = false,
                    RelativeSizeAxes = Axes.Both,
                    Child = scrollableContent = new Container<DrawableCarouselItem>
                    {
                        RelativeSizeAxes = Axes.X,
                    }
                }
            };
        }

        [Resolved]
        private BeatmapManager beatmaps { get; set; }

        [BackgroundDependencyLoader(permitNulls: true)]
        private void load()
        {
            beatmaps.ItemAdded += beatmapAdded;
            beatmaps.ItemRemoved += beatmapRemoved;

            loadBeatmapSets(GetLoadableBeatmaps());
        }

        protected virtual IEnumerable<BeatmapSetInfo> GetLoadableBeatmaps() => beatmaps.GetAllUsableBeatmapSetsEnumerable();

        public void RemoveBeatmapSet(BeatmapSetInfo beatmapSet) => Schedule(() =>
        {
            var existingSet = beatmapSets.FirstOrDefault(b => b.BeatmapSet.ID == beatmapSet.ID);

            if (existingSet == null)
                return;

            root.RemoveChild(existingSet);
            itemsCache.Invalidate();
        });

        public void UpdateBeatmapSet(BeatmapSetInfo beatmapSet) => Schedule(() =>
        {
            int? previouslySelectedID = null;
            CarouselBeatmapSet existingSet = beatmapSets.FirstOrDefault(b => b.BeatmapSet.ID == beatmapSet.ID);

            // If the selected beatmap is about to be removed, store its ID so it can be re-selected if required
            if (existingSet?.State?.Value == CarouselItemState.Selected)
                previouslySelectedID = selectedBeatmap?.Beatmap.ID;

            var newSet = createCarouselSet(beatmapSet);

            if (existingSet != null)
                root.RemoveChild(existingSet);

            if (newSet == null)
            {
                itemsCache.Invalidate();
                return;
            }

            root.AddChild(newSet);

            // only reset scroll position if already near the scroll target.
            // without this, during a large beatmap import it is impossible to navigate the carousel.
            applyActiveCriteria(false, alwaysResetScrollPosition: false);

            //check if we can/need to maintain our current selection.
            if (previouslySelectedID != null)
                select((CarouselItem)newSet.Beatmaps.FirstOrDefault(b => b.Beatmap.ID == previouslySelectedID) ?? newSet);

            itemsCache.Invalidate();
            Schedule(() => BeatmapSetsChanged?.Invoke());
        });

        public bool SelectBeatmap(BeatmapInfo beatmap, bool bypassFilters = true)
        {
            foreach (CarouselBeatmapSet set in beatmapSets)
            {
                if (!bypassFilters && set.Filtered.Value)
                    continue;

                var item = set.Beatmaps.FirstOrDefault(p => p.Beatmap.Equals(beatmap));

                if (item == null)
                    continue;

                if (!bypassFilters && item.Filtered.Value)
                    return false;

                select(item);

                if (set.Filtered.Value)
                {
                    Debug.Assert(bypassFilters);

                    applyActiveCriteria(false);
                }

                return true;
            }

            return false;
        }

        public void SelectNext(int direction = 1, bool skipDifficulties = true)
        {
            if (beatmapSets.All(s => s.Filtered.Value))
                return;

            if (skipDifficulties)
                selectNextSet(direction, true);
            else
                selectNextDifficulty(direction);
        }

        private void selectNextSet(int direction, bool skipDifficulties)
        {
            var unfilteredSets = beatmapSets.Where(s => !s.Filtered.Value).ToList();

            var nextSet = unfilteredSets[(unfilteredSets.IndexOf(selectedBeatmapSet) + direction + unfilteredSets.Count) % unfilteredSets.Count];

            if (skipDifficulties)
                select(nextSet);
            else
                select(direction > 0 ? nextSet.Beatmaps.First(b => !b.Filtered.Value) : nextSet.Beatmaps.Last(b => !b.Filtered.Value));
        }

        private void selectNextDifficulty(int direction)
        {
            var unfilteredDifficulties = selectedBeatmapSet.Children.Where(s => !s.Filtered.Value).ToList();

            int index = unfilteredDifficulties.IndexOf(selectedBeatmap);

            if (index + direction < 0 || index + direction >= unfilteredDifficulties.Count)
                selectNextSet(direction, false);
            else
                select(unfilteredDifficulties[index + direction]);
        }

        private void select(CarouselItem item)
        {
            if (!AllowSelection)
                return;

            if (item == null) return;

            item.State.Value = CarouselItemState.Selected;
        }

        protected ScheduledDelegate PendingFilter;

        public bool AllowSelection = true;

        private float visibleHalfHeight => (DrawHeight + bleed_bottom + bleed_top) / 2;

        /// <summary>
        /// The position of the lower visible bound with respect to the current scroll position.
        /// </summary>
        private float visibleBottomBound => scroll.Current + DrawHeight + bleed_bottom;

        /// <summary>
        /// The position of the upper visible bound with respect to the current scroll position.
        /// </summary>
        private float visibleUpperBound => scroll.Current - bleed_top;

        public void FlushPendingFilterOperations()
        {
            if (PendingFilter?.Completed == false)
            {
                applyActiveCriteria(false);
                Update();
            }
        }


        private void applyActiveCriteria(bool debounce, bool alwaysResetScrollPosition = true)
        {
            if (root.Children.Any() != true) return;

            void perform()
            {
                PendingFilter = null;

                itemsCache.Invalidate();

                if (alwaysResetScrollPosition || !scroll.UserScrolling)
                    ScrollToSelected();
            }

            PendingFilter?.Cancel();
            PendingFilter = null;

            if (debounce)
                PendingFilter = Scheduler.AddDelayed(perform, 250);
            else
                perform();
        }

        private float? scrollTarget;

        public void ScrollToSelected() => scrollPositionCache.Invalidate();

        protected override bool OnKeyDown(KeyDownEvent e)
        {
            switch (e.Key)
            {
                case Key.Left:
                    SelectNext(-1, true);
                    return true;

                case Key.Right:
                    SelectNext(1, true);
                    return true;
            }

            return false;
        }

        protected override void Update()
        {
            base.Update();

            if (!itemsCache.IsValid)
                updateItems();

            scrollableContent.RemoveAll(p => p.Y < visibleUpperBound - p.DrawHeight || p.Y > visibleBottomBound || !p.IsPresent);

            Trace.Assert(Items.Count == yPositions.Count);

            int firstIndex = yPositions.BinarySearch(visibleUpperBound - DrawableCarouselItem.MAX_HEIGHT);
            if (firstIndex < 0) firstIndex = ~firstIndex;
            int lastIndex = yPositions.BinarySearch(visibleBottomBound);
            if (lastIndex < 0) lastIndex = ~lastIndex;

            int notVisibleCount = 0;

            for (int i = firstIndex; i < lastIndex; ++i)
            {
                DrawableCarouselItem item = Items[i];

                if (!item.Item.Visible)
                {
                    if (!item.IsPresent)
                        notVisibleCount++;
                    continue;
                }

                float depth = i + (item is DrawableCarouselBeatmapSet ? -Items.Count : 0);

                if (!scrollableContent.Contains(item))
                {
                    item.Depth = depth;

                    switch (item.LoadState)
                    {
                        case LoadState.NotLoaded:
                            LoadComponentAsync(item);
                            break;

                        case LoadState.Loading:
                            break;

                        default:
                            scrollableContent.Add(item);
                            break;
                    }
                }
                else
                {
                    scrollableContent.ChangeChildDepth(item, depth);
                }
            }
            
            if (notVisibleCount > 50)
                itemsCache.Invalidate();

            foreach (DrawableCarouselItem p in scrollableContent.Children)
                updateItem(p);
        }

        protected override void UpdateAfterChildren()
        {
            base.UpdateAfterChildren();

            if (!scrollPositionCache.IsValid)
                updateScrollPosition();
        }

        protected override void Dispose(bool isDisposing)
        {
            base.Dispose(isDisposing);

            if (beatmaps != null)
            {
                beatmaps.ItemAdded -= beatmapAdded;
                beatmaps.ItemRemoved -= beatmapRemoved;
            }

            // aggressively dispose "off-screen" items to reduce GC pressure.
            foreach (var i in Items)
                i.Dispose();
        }

        private void beatmapRemoved(BeatmapSetInfo item) => RemoveBeatmapSet(item);

        private void beatmapAdded(BeatmapSetInfo item) => UpdateBeatmapSet(item);

        private CarouselBeatmapSet createCarouselSet(BeatmapSetInfo beatmapSet)
        {
            foreach (var b in beatmapSet.Beatmaps)
            {
                if (b.Metadata == null)
                    b.Metadata = beatmapSet.Metadata;
            }

            var set = new CarouselBeatmapSet(beatmapSet);

            foreach (var c in set.Beatmaps)
            {
                c.State.ValueChanged += state =>
                {
                    if (state.NewValue == CarouselItemState.Selected)
                    {
                        selectedBeatmapSet = set;
                        SelectionChanged?.Invoke(c.Beatmap);

                        itemsCache.Invalidate();
                        ScrollToSelected();
                    }
                };
            }

            return set;
        }
        
        private void updateItems()
        {
            Items = root.Drawables.ToList();

            yPositions.Clear();

            float currentY = visibleHalfHeight;
            DrawableCarouselBeatmapSet lastSet = null;

            scrollTarget = null;

            foreach (DrawableCarouselItem d in Items)
            {
                if (d.IsPresent)
                {
                    switch (d)
                    {
                        case DrawableCarouselBeatmapSet set:
                        {
                            lastSet = set;

                            set.MoveToX(set.Item.State.Value == CarouselItemState.Selected ? -100 : 0, 500, Easing.OutExpo);
                            set.MoveToY(currentY, 750, Easing.OutExpo);
                            break;
                        }

                        case DrawableCarouselBeatmap beatmap:
                        {
                            if (beatmap.Item.State.Value == CarouselItemState.Selected)
                                scrollTarget = currentY + beatmap.DrawHeight / 2 - DrawHeight / 2;

                            void performMove(float y, float? startY = null)
                            {
                                if (startY != null) beatmap.MoveTo(new Vector2(0, startY.Value));
                                beatmap.MoveToX(beatmap.Item.State.Value == CarouselItemState.Selected ? -50 : 0, 500, Easing.OutExpo);
                                beatmap.MoveToY(y, 750, Easing.OutExpo);
                            }

                            Debug.Assert(lastSet != null);

                            float? setY = null;
                            if (!d.IsLoaded || Math.Abs(beatmap.Alpha) < 0.05)
                                setY = lastSet.Y + lastSet.DrawHeight + 5;

                            if (d.IsLoaded)
                                performMove(currentY, setY);
                            else
                            {
                                float y = currentY;
                                d.OnLoadComplete += _ => performMove(y, setY);
                            }

                            break;
                        }
                    }
                }

                yPositions.Add(currentY);

                if (d.Item.Visible)
                    currentY += d.DrawHeight + 5;
            }

            currentY += visibleHalfHeight;
            scrollableContent.Height = currentY;

            if (BeatmapSetsLoaded && (selectedBeatmapSet == null || selectedBeatmap == null || selectedBeatmapSet.State.Value != CarouselItemState.Selected))
            {
                selectedBeatmapSet = null;
                SelectionChanged?.Invoke(null);
            }

            itemsCache.Validate();
        }

        private bool firstScroll = true;

        private void updateScrollPosition()
        {
            if (scrollTarget != null)
            {
                if (firstScroll)
                {
                    // reduce movement when first displaying the carousel.
                    scroll.ScrollTo(scrollTarget.Value - 200, false);
                    firstScroll = false;
                }

                scroll.ScrollTo(scrollTarget.Value);
                scrollPositionCache.Validate();
            }
        }

        private static float offsetX(float dist, float halfHeight)
        {
            const float circle_radius = 3;
            float discriminant = MathF.Max(0, circle_radius * circle_radius - dist * dist);
            float x = (circle_radius - MathF.Sqrt(discriminant)) * halfHeight;

            return 125 + x;
        }

        private void updateItem(DrawableCarouselItem p)
        {
            float itemDrawY = p.Position.Y - visibleUpperBound + p.DrawHeight / 2;
            float dist = Math.Abs(1f - itemDrawY / visibleHalfHeight);

            p.OriginPosition = new Vector2(-offsetX(dist, visibleHalfHeight), 0);

            p.SetMultiplicativeAlpha(Math.Clamp(1.75f - 1.5f * dist, 0, 1));
        }

        private class CarouselRoot : CarouselGroupEagerSelect
        {
            private readonly BeatmapCarousel carousel;

            public CarouselRoot(BeatmapCarousel carousel)
            {
                this.carousel = carousel;
                State.Value = CarouselItemState.Selected;
                State.ValueChanged += state => State.Value = CarouselItemState.Selected;
            }
        }

        private class CarouselScrollContainer : TachyonScrollContainer
        {
            private bool rightMouseScrollBlocked;

            public bool UserScrolling { get; private set; }

            protected override void OnUserScroll(float value, bool animated = true, double? distanceDecay = default)
            {
                UserScrolling = true;
                base.OnUserScroll(value, animated, distanceDecay);
            }

            public new void ScrollTo(float value, bool animated = true, double? distanceDecay = null)
            {
                UserScrolling = false;
                base.ScrollTo(value, animated, distanceDecay);
            }

            protected override bool OnMouseDown(MouseDownEvent e)
            {
                if (e.Button == MouseButton.Right)
                {
                    if (GetContainingInputManager().HoveredDrawables.OfType<DrawableCarouselItem>().Any())
                    {
                        rightMouseScrollBlocked = true;
                        return false;
                    }
                }

                rightMouseScrollBlocked = false;
                return base.OnMouseDown(e);
            }

            protected override bool OnDragStart(DragStartEvent e)
            {
                if (rightMouseScrollBlocked)
                    return false;

                return base.OnDragStart(e);
            }
        }
    }
}
