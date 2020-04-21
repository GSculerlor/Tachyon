 using System;
 using System.Collections.Generic;
 using System.Diagnostics;
 using System.Linq;
 using osu.Framework.Allocation;
 using osu.Framework.Caching;
 using osu.Framework.Extensions.IEnumerableExtensions;
 using osu.Framework.Graphics;
 using osu.Framework.Graphics.Containers;
 using osu.Framework.Input.Events;
 using osu.Framework.Threading;
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

        private bool beatmapSetsLoaded;

        private readonly CarouselScrollContainer scroll;

        private IEnumerable<CarouselBeatmapSet> beatmapSets => root.Children.OfType<CarouselBeatmapSet>();

        public IEnumerable<BeatmapSetInfo> BeatmapSets
        {
            get => beatmapSets.Select(g => g.BeatmapSet);
            set => loadBeatmapSets(value);
        }

        private void loadBeatmapSets(IEnumerable<BeatmapSetInfo> beatmapSets)
        {
            CarouselRoot newRoot = new CarouselRoot();

            var beatmapSetInfos = beatmapSets.ToList();
            beatmapSetInfos.Select(createCarouselSet).Where(g => g != null).ForEach(newRoot.AddChild);

            _ = newRoot.Drawables;

            root = newRoot;
            if (selectedBeatmapSet != null && !beatmapSetInfos.Contains(selectedBeatmapSet.BeatmapSet))
                selectedBeatmapSet = null;

            scrollableContent.Clear(false);
            itemsCache.Invalidate();
            scrollPositionCache.Invalidate();

            SchedulerAfterChildren.Add(() =>
            {
                BeatmapSetsChanged?.Invoke();
                beatmapSetsLoaded = true;
            });
        }

        private readonly List<float> yPositions = new List<float>();
        private readonly Cached itemsCache = new Cached();
        private readonly Cached scrollPositionCache = new Cached();

        private readonly Container<DrawableCarouselItem> scrollableContent;

        private List<DrawableCarouselItem> items = new List<DrawableCarouselItem>();
        private CarouselRoot root;

        public BeatmapCarousel()
        {
            root = new CarouselRoot();
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

        [BackgroundDependencyLoader]
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

            applyActiveCriteria(false, false);

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

        private float visibleBottomBound => scroll.Current + DrawHeight + bleed_bottom;

        private float visibleUpperBound => scroll.Current - bleed_top;

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
                    SelectNext(-1);
                    return true;

                case Key.Right:
                    SelectNext();
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

            Trace.Assert(items.Count == yPositions.Count);

            int firstIndex = yPositions.BinarySearch(visibleUpperBound - DrawableCarouselItem.MAX_HEIGHT);
            if (firstIndex < 0) firstIndex = ~firstIndex;
            int lastIndex = yPositions.BinarySearch(visibleBottomBound);
            if (lastIndex < 0) lastIndex = ~lastIndex;

            int notVisibleCount = 0;

            for (int i = firstIndex; i < lastIndex; ++i)
            {
                DrawableCarouselItem item = items[i];

                if (!item.Item.Visible)
                {
                    if (!item.IsPresent)
                        notVisibleCount++;
                    continue;
                }

                float depth = i + (item is DrawableCarouselBeatmapSet ? -items.Count : 0);

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
                p.OriginPosition = new Vector2(-200, 0);
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

            foreach (var i in items)
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
            items = root.Drawables.ToList();

            yPositions.Clear();

            float currentY = visibleHalfHeight;
            DrawableCarouselBeatmapSet lastSet = null;

            scrollTarget = null;

            foreach (DrawableCarouselItem d in items)
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

            if (beatmapSetsLoaded && (selectedBeatmapSet == null || selectedBeatmap == null || selectedBeatmapSet.State.Value != CarouselItemState.Selected))
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
                    scroll.ScrollTo(scrollTarget.Value - 200, false);
                    firstScroll = false;
                }

                scroll.ScrollTo(scrollTarget.Value);
                scrollPositionCache.Validate();
            }
        }

        private class CarouselRoot : CarouselGroupEagerSelect
        {
            public CarouselRoot()
            {
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
