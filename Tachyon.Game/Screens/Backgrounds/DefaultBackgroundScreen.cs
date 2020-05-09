using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Threading;
using Tachyon.Game.Beatmaps;
using Tachyon.Game.Graphics.Backgrounds;

namespace Tachyon.Game.Screens.Backgrounds
{
    public class DefaultBackgroundScreen : BackgroundScreen
    {
        private Background background;

        private int currentDisplay;

        private string backgroundName => @"Backgrounds/background_gatau_cantik";

        [Resolved]
        private IBindable<WorkingBeatmap> beatmap { get; set; }

        [BackgroundDependencyLoader]
        private void load()
        {
            display(createBackground());
        }

        private void display(Background newBackground)
        {
            background?.FadeOut(800, Easing.InOutSine);
            background?.Expire();

            AddInternal(background = newBackground);
            currentDisplay++;
        }

        private ScheduledDelegate nextTask;

        public void Next()
        {
            nextTask?.Cancel();
            nextTask = Scheduler.AddDelayed(() => { LoadComponentAsync(createBackground(), display); }, 100);
        }

        private Background createBackground()
        {
            var newBackground = new Background(backgroundName) { Depth = currentDisplay };

            return newBackground;
        }
    }
}
