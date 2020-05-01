using System;
using System.Collections.Generic;
using System.Linq;
using osu.Framework.Allocation;
using osu.Framework.Extensions.Color4Extensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Effects;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Input.Bindings;
using osu.Framework.Input.Events;
using osuTK;
using osuTK.Graphics;
using Tachyon.Game.Graphics;
using Tachyon.Game.Graphics.UserInterface;
using Tachyon.Game.Input;

namespace Tachyon.Game.Screens.Play
{
    public class GameplayMenuOverlay : OverlayContainer, IKeyBindingHandler<GlobalAction>
    {
        private const int transition_duration = 200;
        private const int button_height = 90;
        private const float background_alpha = 0.75f;
        
        protected override bool BlockNonPositionalInput => true;

        public override bool ReceivePositionalInputAt(Vector2 screenSpacePos) => true;

        public Action OnRetry;
        public Action OnQuit;
        
        protected virtual Action BackAction => () => InternalButtons.Children.LastOrDefault()?.Click();

        protected virtual Action SelectAction => () => InternalButtons.Children.FirstOrDefault(f => f.Selected.Value)?.Click();

        protected internal FillFlowContainer<OverlayButton> InternalButtons;
        public IReadOnlyList<OverlayButton> Buttons => InternalButtons;
        
        protected GameplayMenuOverlay()
        {
            RelativeSizeAxes = Axes.Both;

            State.ValueChanged += s => selectionIndex = -1;
        }
        
        
        [BackgroundDependencyLoader]
        private void load(TachyonColor colors)
        {
            Children = new Drawable[]
            {
                new Box
                {
                    RelativeSizeAxes = Axes.Both,
                    Colour = Color4.Black,
                    Alpha = background_alpha,
                },
                new FillFlowContainer
                {
                    RelativeSizeAxes = Axes.X,
                    AutoSizeAxes = Axes.Y,
                    Direction = FillDirection.Vertical,
                    Spacing = new Vector2(0, 50),
                    Origin = Anchor.Centre,
                    Anchor = Anchor.Centre,
                    Children = new Drawable[]
                    {
                        InternalButtons = new FillFlowContainer<OverlayButton>
                        {
                            Origin = Anchor.TopCentre,
                            Anchor = Anchor.TopCentre,
                            RelativeSizeAxes = Axes.X,
                            AutoSizeAxes = Axes.Y,
                            Direction = FillDirection.Vertical,
                            Masking = true,
                            EdgeEffect = new EdgeEffectParameters
                            {
                                Type = EdgeEffectType.Shadow,
                                Colour = Color4.Black.Opacity(0.6f),
                                Radius = 50
                            },
                        }
                    }
                },
            };
        }
        
        protected override void PopIn() => this.FadeIn(transition_duration, Easing.In);
        protected override void PopOut() => this.FadeOut(transition_duration, Easing.In);

        // Don't let mouse down events through the overlay or people can click circles while paused.
        protected override bool OnMouseDown(MouseDownEvent e) => true;

        protected override bool OnMouseMove(MouseMoveEvent e) => true;
        
        private int selectionIndex = -1;
        
        protected void AddButton(string text, IconUsage icon, Action action, bool isDestructive = false)
        {
            var button = new OverlayButton
            {
                Text = text,
                Icon = icon,
                IsDestructive = isDestructive,
                Origin = Anchor.TopCentre,
                Anchor = Anchor.TopCentre,
                Height = button_height,
                Action = delegate
                {
                    action?.Invoke();
                    Hide();
                }
            };

            button.Selected.ValueChanged += selected => buttonSelectionChanged(button, selected.NewValue);

            InternalButtons.Add(button);
        }


        private void setSelected(int value)
        {
            if (selectionIndex == value)
                return;

            // Deselect the previously-selected button
            if (selectionIndex != -1)
                InternalButtons[selectionIndex].Selected.Value = false;

            selectionIndex = value;

            // Select the newly-selected button
            if (selectionIndex != -1)
                InternalButtons[selectionIndex].Selected.Value = true;
        }

        private void buttonSelectionChanged(OverlayButton button, bool isSelected)
        {
            if (!isSelected)
                setSelected(-1);
            else
                setSelected(InternalButtons.IndexOf(button));
        }
        
        public bool OnPressed(GlobalAction action)
        {
            switch (action)
            {
                case GlobalAction.Back:
                    BackAction.Invoke();
                    return true;
            }

            return false;
        }

        public void OnReleased(GlobalAction action)
        {
        }
    }
}
