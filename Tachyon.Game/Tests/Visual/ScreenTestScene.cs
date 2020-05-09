using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Testing;
using Tachyon.Game.Screens;

namespace Tachyon.Game.Tests.Visual
{
    public abstract class ScreenTestScene : TachyonManualInputManagerTestScene
    {
        protected readonly TachyonScreenStack Stack;

        private readonly Container content;

        protected override Container<Drawable> Content => content;

        protected ScreenTestScene()
        {
            base.Content.AddRange(new Drawable[]
            {
                Stack = new TachyonScreenStack { RelativeSizeAxes = Axes.Both },
                content = new Container { RelativeSizeAxes = Axes.Both }
            });
        }

        protected void LoadScreen(TachyonScreen screen) => Stack.Push(screen);

        [SetUpSteps]
        public virtual void SetUpSteps() => addExitAllScreensStep();

        [TearDownSteps]
        public virtual void TearDownSteps() => addExitAllScreensStep();

        private void addExitAllScreensStep()
        {
            AddUntilStep("exit all screens", () =>
            {
                if (Stack.CurrentScreen == null) return true;

                Stack.Exit();
                return false;
            });
        }
    }
}
