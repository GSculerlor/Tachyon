using System.Collections.Generic;
using System.Linq;

namespace Tachyon.Game.Screens.Select.Carousel
{
    public class CarouselGroup : CarouselItem
    {
        protected override DrawableCarouselItem CreateDrawableRepresentation() => null;

        public IReadOnlyList<CarouselItem> Children => InternalChildren;

        protected List<CarouselItem> InternalChildren = new List<CarouselItem>();

        private ulong currentChildID;

        public override List<DrawableCarouselItem> Drawables
        {
            get
            {
                var drawables = base.Drawables;

                if (DrawableRepresentation.Value?.IsPresent == false) return drawables;

                foreach (var c in InternalChildren)
                    drawables.AddRange(c.Drawables);
                return drawables;
            }
        }

        public virtual void RemoveChild(CarouselItem i)
        {
            InternalChildren.Remove(i);

            i.State.Value = CarouselItemState.Collapsed;
        }

        public virtual void AddChild(CarouselItem i)
        {
            i.State.ValueChanged += state => ChildItemStateChanged(i, state.NewValue);
            i.ChildID = ++currentChildID;
            InternalChildren.Add(i);
        }

        public CarouselGroup(List<CarouselItem> items = null)
        {
            if (items != null) InternalChildren = items;

            State.ValueChanged += state =>
            {
                switch (state.NewValue)
                {
                    case CarouselItemState.Collapsed:
                    case CarouselItemState.NotSelected:
                        InternalChildren.ForEach(c => c.State.Value = CarouselItemState.Collapsed);
                        break;

                    case CarouselItemState.Selected:
                        InternalChildren.ForEach(c =>
                        {
                            if (c.State.Value == CarouselItemState.Collapsed) c.State.Value = CarouselItemState.NotSelected;
                        });
                        break;
                }
            };
        }

        protected virtual void ChildItemStateChanged(CarouselItem item, CarouselItemState value)
        {
            if (value == CarouselItemState.Selected)
            {
                foreach (var b in InternalChildren)
                {
                    if (item == b) continue;

                    b.State.Value = CarouselItemState.NotSelected;
                }

                State.Value = CarouselItemState.Selected;
            }
        }
    }
}
