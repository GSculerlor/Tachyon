using System;
using System.Linq;

namespace Tachyon.Game.Screens.Select.Carousel
{
    public class CarouselGroupEagerSelect : CarouselGroup
    {
        public CarouselGroupEagerSelect()
        {
            State.ValueChanged += state =>
            {
                if (state.NewValue == CarouselItemState.Selected)
                    attemptSelection();
            };
        }

        protected CarouselItem LastSelected { get; private set; }

        private int lastSelectedIndex;
        public override void RemoveChild(CarouselItem i)
        {
            base.RemoveChild(i);

            if (i != LastSelected)
                updateSelectedIndex();
        }

        public override void AddChild(CarouselItem i)
        {
            base.AddChild(i);
            attemptSelection();
        }

        protected override void ChildItemStateChanged(CarouselItem item, CarouselItemState value)
        {
            base.ChildItemStateChanged(item, value);

            switch (value)
            {
                case CarouselItemState.Selected:
                    updateSelected(item);
                    break;

                case CarouselItemState.NotSelected:
                case CarouselItemState.Collapsed:
                    attemptSelection();
                    break;
            }
        }

        private void attemptSelection()
        {
            if (State.Value != CarouselItemState.Selected) return;

            if (Children.Any(i => i.State.Value == CarouselItemState.Selected)) return;

            PerformSelection();
        }

        protected virtual void PerformSelection()
        {
            CarouselItem nextToSelect =
                Children.Skip(lastSelectedIndex).FirstOrDefault(i => !i.Filtered.Value) ??
                Children.Reverse().Skip(InternalChildren.Count - lastSelectedIndex).FirstOrDefault(i => !i.Filtered.Value);

            if (nextToSelect != null)
                nextToSelect.State.Value = CarouselItemState.Selected;
            else
                updateSelected(null);
        }

        private void updateSelected(CarouselItem newSelection)
        {
            if (newSelection != null)
                LastSelected = newSelection;
            updateSelectedIndex();
        }

        private void updateSelectedIndex() => lastSelectedIndex = LastSelected == null ? 0 : Math.Max(0, InternalChildren.IndexOf(LastSelected));
    }
}
