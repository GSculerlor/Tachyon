﻿using System;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Internal;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Layout;

namespace Tachyon.Game.Graphics.Containers
{
    public class SectionsContainer<T> : Container<T>
        where T : Drawable
    {
        public Bindable<T> SelectedSection { get; } = new Bindable<T>();

        public Drawable ExpandableHeader
        {
            get => expandableHeader;
            set
            {
                if (value == expandableHeader) return;

                expandableHeader?.Expire();
                expandableHeader = value;
                if (value == null) return;

                AddInternal(expandableHeader);
                lastKnownScroll = float.NaN;
            }
        }

        public Drawable FixedHeader
        {
            get => fixedHeader;
            set
            {
                if (value == fixedHeader) return;

                fixedHeader?.Expire();
                fixedHeader = value;
                if (value == null) return;

                AddInternal(fixedHeader);
                lastKnownScroll = float.NaN;
            }
        }

        public Drawable Footer
        {
            get => footer;
            set
            {
                if (value == footer) return;

                if (footer != null)
                    scrollContainer.Remove(footer);
                footer = value;
                if (value == null) return;

                footer.Anchor |= Anchor.y2;
                footer.Origin |= Anchor.y2;
                scrollContainer.Add(footer);
                lastKnownScroll = float.NaN;
            }
        }

        public Drawable HeaderBackground
        {
            get => headerBackground;
            set
            {
                if (value == headerBackground) return;

                headerBackgroundContainer.Clear();
                headerBackground = value;

                if (value == null) return;

                headerBackgroundContainer.Add(headerBackground);

                lastKnownScroll = float.NaN;
            }
        }

        protected override Container<T> Content => scrollContentContainer;

        private readonly TachyonScrollContainer scrollContainer;
        private readonly Container headerBackgroundContainer;
        private readonly MarginPadding originalSectionsMargin;
        private Drawable expandableHeader, fixedHeader, footer, headerBackground;
        private FlowContainer<T> scrollContentContainer;

        private float headerHeight, footerHeight;

        private float lastKnownScroll;

        public SectionsContainer()
        {
            AddRangeInternal(new Drawable[]
            {
                scrollContainer = CreateScrollContainer().With(s =>
                {
                    s.RelativeSizeAxes = Axes.Both;
                    s.Masking = true;
                    s.ScrollbarVisible = false;
                    s.Child = scrollContentContainer = CreateScrollContentContainer();
                }),
                headerBackgroundContainer = new Container
                {
                    RelativeSizeAxes = Axes.X
                }
            });

            originalSectionsMargin = scrollContentContainer.Margin;
        }

        public override void Add(T drawable)
        {
            base.Add(drawable);
            lastKnownScroll = float.NaN;
            headerHeight = float.NaN;
            footerHeight = float.NaN;
        }

        public void ScrollTo(Drawable section) =>
            scrollContainer.ScrollTo(scrollContainer.GetChildPosInContent(section) - (FixedHeader?.BoundingBox.Height ?? 0));

        public void ScrollToTop() => scrollContainer.ScrollTo(0);

        [NotNull]
        protected virtual TachyonScrollContainer CreateScrollContainer() => new TachyonScrollContainer();

        [NotNull]
        protected virtual FlowContainer<T> CreateScrollContentContainer() =>
            new FillFlowContainer<T>
            {
                Direction = FillDirection.Vertical,
                AutoSizeAxes = Axes.Y,
                RelativeSizeAxes = Axes.X,
            };

        protected override bool OnInvalidate(Invalidation invalidation, InvalidationSource source)
        {
            var result = base.OnInvalidate(invalidation, source);

            if (source == InvalidationSource.Child && (invalidation & Invalidation.DrawSize) != 0)
            {
                lastKnownScroll = -1;
                result = true;
            }

            return result;
        }

        protected override void UpdateAfterChildren()
        {
            base.UpdateAfterChildren();

            float headerH = (ExpandableHeader?.LayoutSize.Y ?? 0) + (FixedHeader?.LayoutSize.Y ?? 0);
            float footerH = Footer?.LayoutSize.Y ?? 0;

            if (headerH != headerHeight || footerH != footerHeight)
            {
                headerHeight = headerH;
                footerHeight = footerH;
                updateSectionsMargin();
            }

            float currentScroll = scrollContainer.Current;

            if (currentScroll != lastKnownScroll)
            {
                lastKnownScroll = currentScroll;

                if (ExpandableHeader != null && FixedHeader != null)
                {
                    float offset = Math.Min(ExpandableHeader.LayoutSize.Y, currentScroll);

                    ExpandableHeader.Y = -offset;
                    FixedHeader.Y = -offset + ExpandableHeader.LayoutSize.Y;
                }

                headerBackgroundContainer.Height = (ExpandableHeader?.LayoutSize.Y ?? 0) + (FixedHeader?.LayoutSize.Y ?? 0);
                headerBackgroundContainer.Y = ExpandableHeader?.Y ?? 0;

                T bestMatch = null;
                float minDiff = float.MaxValue;
                float scrollOffset = FixedHeader?.LayoutSize.Y ?? 0;

                foreach (var section in Children)
                {
                    float diff = Math.Abs(scrollContainer.GetChildPosInContent(section) - currentScroll - scrollOffset);

                    if (diff < minDiff)
                    {
                        minDiff = diff;
                        bestMatch = section;
                    }
                }

                if (bestMatch != null)
                    SelectedSection.Value = bestMatch;
            }
        }

        private void updateSectionsMargin()
        {
            if (!Children.Any()) return;

            var newMargin = originalSectionsMargin;
            newMargin.Top += headerHeight;
            newMargin.Bottom += footerHeight;

            scrollContentContainer.Margin = newMargin;
        }
    }
}
