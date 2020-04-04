using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Testing;

namespace Tachyon.Game.Tests.Visual
{
    public abstract class TachyonTestScene : TestScene
    {
        protected override Container<Drawable> Content => content ?? base.Content;

        private readonly Container content;
        
        protected TachyonTestScene()
        {
            base.Content.Add(content = new DrawSizePreservingFillContainer());
        }
        
        protected override ITestSceneTestRunner CreateRunner() => new TachyonTestSceneTestRunner();

        private class TachyonTestSceneTestRunner : TachyonGameBase, ITestSceneTestRunner
        {
            private TestSceneTestRunner.TestRunner runner;

            protected override void LoadAsyncComplete()
            {
                Add(runner = new TestSceneTestRunner.TestRunner());
            }

            public void RunTestBlocking(TestScene test) => runner.RunTestBlocking(test);
        }
    }
}