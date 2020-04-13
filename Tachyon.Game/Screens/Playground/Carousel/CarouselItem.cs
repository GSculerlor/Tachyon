using System;
using System.Collections.Generic;
using osu.Framework.Bindables;

namespace Tachyon.Game.Screens.Playground.Carousel
{
    public abstract class CarouselItem
    {
        public readonly BindableBool Filtered = new BindableBool();

        public readonly Bindable<CarouselItemState> State = new Bindable<CarouselItemState>(CarouselItemState.NotSelected);

        public bool Visible => State.Value != CarouselItemState.Collapsed && !Filtered.Value;

        public virtual List<DrawableCarouselItem> Drawables
        {
            get
            {
                var items = new List<DrawableCarouselItem>();

                var self = DrawableRepresentation.Value;
                if (self?.IsPresent == true) items.Add(self);

                return items;
            }
        }

        protected CarouselItem()
        {
            DrawableRepresentation = new Lazy<DrawableCarouselItem>(CreateDrawableRepresentation);

            Filtered.ValueChanged += filtered =>
            {
                if (filtered.NewValue && State.Value == CarouselItemState.Selected)
                    State.Value = CarouselItemState.NotSelected;
            };
        }

        protected readonly Lazy<DrawableCarouselItem> DrawableRepresentation;

        internal ulong ChildID;

        protected abstract DrawableCarouselItem CreateDrawableRepresentation();

        public virtual int CompareTo(CarouselItem other) => ChildID.CompareTo(other.ChildID);
    }

    public enum CarouselItemState
    {
        Collapsed,
        NotSelected,
        Selected,
    }
}
