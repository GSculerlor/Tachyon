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
        private ScheduledDelegate nextTask;
        private Background background;
        
        [Resolved]
        private IBindable<WorkingBeatmap> beatmap { get; set; }
        
        [BackgroundDependencyLoader]
        private void load()
        {
            beatmap.ValueChanged += _ => Next();

            display(createBackground());
        }
        
        public void Next()
        {
            nextTask?.Cancel();
            nextTask = Scheduler.AddDelayed(() => { LoadComponentAsync(createBackground(), display); }, 100);
        }
        
        private void display(Background newBackground)
        {
            background?.FadeOut(800, Easing.InOutSine);
            background?.Expire();

            AddInternal(background = newBackground);
        }
        
        private Background createBackground() => new BeatmapBackground(beatmap.Value);
    }
}
