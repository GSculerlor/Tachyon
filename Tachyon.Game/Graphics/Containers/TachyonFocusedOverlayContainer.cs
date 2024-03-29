﻿using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics.Containers;
using osu.Framework.Input.Bindings;
using osu.Framework.Input.Events;
using osuTK;
using Tachyon.Game.Input;

namespace Tachyon.Game.Graphics.Containers
{
    [Cached]
    public abstract class TachyonFocusedOverlayContainer : FocusedOverlayContainer, IKeyBindingHandler<GlobalAction>
    {
        protected override bool BlockNonPositionalInput => true;

        /// <summary>
        /// Temporary to allow for overlays in the main screen content to not dim theirselves.
        /// Should be eventually replaced by dimming which is aware of the target dim container (traverse parent for certain interface type?).
        /// </summary>
        protected virtual bool DimMainContent => true;

        [Resolved(CanBeNull = true)]
        private TachyonGame game { get; set; }

        /// <summary>
        /// Whether mouse input should be blocked screen-wide while this overlay is visible.
        /// Performing mouse actions outside of the valid extents will hide the overlay.
        /// </summary>
        public virtual bool BlockScreenWideMouse => BlockPositionalInput;

        // receive input outside our bounds so we can trigger a close event on ourselves.
        public override bool ReceivePositionalInputAt(Vector2 screenSpacePos) => BlockScreenWideMouse || base.ReceivePositionalInputAt(screenSpacePos);

        private bool closeOnMouseUp;

        protected override bool OnMouseDown(MouseDownEvent e)
        {
            closeOnMouseUp = !base.ReceivePositionalInputAt(e.ScreenSpaceMousePosition);

            return base.OnMouseDown(e);
        }

        protected override void OnMouseUp(MouseUpEvent e)
        {
            if (closeOnMouseUp && !base.ReceivePositionalInputAt(e.ScreenSpaceMousePosition))
                Hide();

            base.OnMouseUp(e);
        }

        public virtual bool OnPressed(GlobalAction action)
        {
            switch (action)
            {
                case GlobalAction.Back:
                    Hide();
                    return true;

                case GlobalAction.Select:
                    return true;
            }

            return false;
        }

        public void OnReleased(GlobalAction action)
        {
        }

        protected override void UpdateState(ValueChangedEvent<Visibility> state)
        {
            switch (state.NewValue)
            {
                case Visibility.Visible:
                    if (BlockScreenWideMouse && DimMainContent) game?.AddBlockingOverlay(this);
                    break;

                case Visibility.Hidden:
                    if (BlockScreenWideMouse) game?.RemoveBlockingOverlay(this);
                    break;
            }

            base.UpdateState(state);
        }

        /*protected override void Dispose(bool isDisposing)
        {
            base.Dispose(isDisposing);
            game?.RemoveBlockingOverlay(this);
        }*/
    }
}
