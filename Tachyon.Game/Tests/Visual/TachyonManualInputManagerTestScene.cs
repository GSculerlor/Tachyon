using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Cursor;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Testing.Input;
using osuTK;
using osuTK.Graphics;
using Tachyon.Game.Graphics.Sprites;
using Tachyon.Game.Graphics.UserInterface;
using Tachyon.Game.Input;

namespace Tachyon.Game.Tests.Visual
{
    public class TachyonManualInputManagerTestScene : TachyonTestScene
    {
         protected override Container<Drawable> Content => content;
        private readonly Container content;

        protected readonly ManualInputManager InputManager;

        private readonly TachyonButton buttonTest;
        private readonly TachyonButton buttonLocal;

        protected TachyonManualInputManagerTestScene()
        {
            CursorContainer cursorContainer;

            base.Content.AddRange(new Drawable[]
            {
                InputManager = new ManualInputManager
                {
                    UseParentInput = true,
                    Child = new GlobalActionContainer(null)
                        .WithChild((cursorContainer = new CursorContainer { RelativeSizeAxes = Axes.Both })
                            .WithChild(content = new TooltipContainer { RelativeSizeAxes = Axes.Both }))
                },
                new Container
                {
                    AutoSizeAxes = Axes.Both,
                    Anchor = Anchor.TopRight,
                    Origin = Anchor.TopRight,
                    Margin = new MarginPadding(5),
                    CornerRadius = 5,
                    Masking = true,
                    Children = new Drawable[]
                    {
                        new Box
                        {
                            Colour = Color4.Black,
                            RelativeSizeAxes = Axes.Both,
                            Alpha = 0.5f,
                        },
                        new FillFlowContainer
                        {
                            AutoSizeAxes = Axes.Both,
                            Direction = FillDirection.Vertical,
                            Margin = new MarginPadding(5),
                            Spacing = new Vector2(5),
                            Children = new Drawable[]
                            {
                                new TachyonSpriteText
                                {
                                    Anchor = Anchor.TopCentre,
                                    Origin = Anchor.TopCentre,
                                    Text = "Input Priority"
                                },
                                new FillFlowContainer
                                {
                                    AutoSizeAxes = Axes.Both,
                                    Anchor = Anchor.TopCentre,
                                    Origin = Anchor.TopCentre,
                                    Margin = new MarginPadding(5),
                                    Spacing = new Vector2(5),
                                    Direction = FillDirection.Horizontal,

                                    Children = new Drawable[]
                                    {
                                        buttonLocal = new TachyonButton
                                        {
                                            Text = "local",
                                            Size = new Vector2(50, 30),
                                            Action = returnUserInput
                                        },
                                        buttonTest = new TachyonButton
                                        {
                                            Text = "test",
                                            Size = new Vector2(50, 30),
                                            Action = returnTestInput
                                        },
                                    }
                                },
                            }
                        },
                    }
                },
            });
        }

        protected override void Update()
        {
            base.Update();

            buttonTest.Enabled.Value = InputManager.UseParentInput;
            buttonLocal.Enabled.Value = !InputManager.UseParentInput;
        }

        private void returnUserInput() =>
            InputManager.UseParentInput = true;

        private void returnTestInput() =>
            InputManager.UseParentInput = false;
    }
}
