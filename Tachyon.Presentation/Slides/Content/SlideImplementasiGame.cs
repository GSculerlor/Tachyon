using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Input;
using osu.Framework.Platform;
using osuTK;
using Tachyon.Game;

namespace Tachyon.Presentation.Slides.Content
{
    public class SlideImplementasiGame : SlideWithTitle
    {
        private FillFlowContainer fill;
        private GameHost host;

        public SlideImplementasiGame()
            : base("Implementasi Game")
        {
        }

        [BackgroundDependencyLoader]
        private void load(GameHost host)
        {
            this.host = host;
            Content.Add(fill = new FillFlowContainer
            {
                RelativeSizeAxes = Axes.Both,
                Direction = FillDirection.Full
            });

            addNewGame();
        }

        private void addNewGame()
        {
            var game = new TachyonGame();
            game.SetHost(host);

            var container = new InputRedirection
            {
                Masking = true,
                RelativeSizeAxes = Axes.Both,
                Child = game
            };

            LoadComponentAsync(container, loaded =>
            {
                fill.Add(loaded);

                var targetScale = findTargetScale();
                foreach (var child in fill.Children)
                    child.ResizeTo(targetScale, 1000, Easing.OutQuint);
            });
        }

        private Vector2 findTargetScale()
        {
            var ratio = Vector2.One;

            float count = fill.Children.Count;
            var vertical = false;

            while ((count /= 2) > 0.5f)
            {
                if (vertical)
                    ratio.Y *= 0.5f;
                else
                    ratio.X *= 0.5f;

                vertical = !vertical;
            }

            return ratio - new Vector2(0.01f);
        }
    }

    internal class InputRedirection : PassThroughInputManager
    {
    }
}
