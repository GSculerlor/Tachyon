﻿using osu.Framework.Bindables;
using osu.Framework.Extensions.Color4Extensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using Tachyon.Game.Graphics;

namespace Tachyon.Game.Overlays.Toolbar
{
    public class ToolbarOverlayToggleButton: ToolbarButton
    {
        private readonly Box stateBackground;

        private OverlayContainer stateContainer;

        private readonly Bindable<Visibility> overlayState = new Bindable<Visibility>();

        public OverlayContainer StateContainer
        {
            get => stateContainer;
            set
            {
                stateContainer = value;

                overlayState.UnbindBindings();

                if (stateContainer != null)
                {
                    Action = stateContainer.ToggleVisibility;
                    overlayState.BindTo(stateContainer.State);
                }
            }
        }

        public ToolbarOverlayToggleButton()
        {
            Add(stateBackground = new Box
            {
                RelativeSizeAxes = Axes.Both,
                Colour = TachyonColor.Gray(150).Opacity(180),
                Blending = BlendingParameters.Additive,
                Depth = 2,
                Alpha = 0,
            });

            overlayState.ValueChanged += stateChanged;
        }

        private void stateChanged(ValueChangedEvent<Visibility> state)
        {
            switch (state.NewValue)
            {
                case Visibility.Hidden:
                    stateBackground.FadeOut(200);
                    break;

                case Visibility.Visible:
                    stateBackground.FadeIn(200);
                    break;
            }
        }
    }
}