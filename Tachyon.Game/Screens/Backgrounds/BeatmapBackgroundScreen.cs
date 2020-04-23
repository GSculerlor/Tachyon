using System;
using System.Threading;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using Tachyon.Game.Beatmaps;
using Tachyon.Game.Graphics.Backgrounds;
using Tachyon.Game.Graphics.Containers;

namespace Tachyon.Game.Screens.Backgrounds
{
    public class BeatmapBackgroundScreen : BackgroundScreen
    {
        protected Background Background;

        private WorkingBeatmap beatmap;
        
        private readonly DimmableBackground dimmable;

        public BeatmapBackgroundScreen(WorkingBeatmap beatmap = null)
        {
            Beatmap = beatmap;
            
            InternalChild = dimmable = new DimmableBackground { RelativeSizeAxes = Axes.Both };
        }
        
        [BackgroundDependencyLoader]
        private void load()
        {
            var background = new BeatmapBackground(beatmap);
            LoadComponent(background);
            switchBackground(background);
        }
        
        private CancellationTokenSource cancellationSource;

        public WorkingBeatmap Beatmap
        {
            get => beatmap;
            set
            {
                if (beatmap == value && beatmap != null)
                    return;

                beatmap = value;

                Schedule(() =>
                {
                    if ((Background as BeatmapBackground)?.Beatmap.BeatmapInfo.BackgroundEquals(beatmap?.BeatmapInfo) ?? false)
                        return;

                    cancellationSource?.Cancel();
                    LoadComponentAsync(new BeatmapBackground(beatmap), switchBackground, (cancellationSource = new CancellationTokenSource()).Token);
                });
            }
        }
        
        private void switchBackground(BeatmapBackground b)
        {
            float newDepth = 0;

            if (Background != null)
            {
                newDepth = Background.Depth + 1;
                Background.FinishTransforms();
                Background.FadeOut(250);
                Background.Expire();
            }

            b.Depth = newDepth;
            dimmable.Background = Background = b;
        }

        public override bool Equals(BackgroundScreen other)
        {
            if (!(other is BeatmapBackgroundScreen otherBeatmapBackground)) return false;

            return base.Equals(other) && beatmap == otherBeatmapBackground.Beatmap;
        }

        public class DimmableBackground : DimmableContainer
        {
            private Background background;
            
            public Background Background
            {
                get => background;
                set
                {
                    background?.Expire();

                    base.Add(background = value);
                }
            }
            
            public override void Add(Drawable drawable)
            {
                if (drawable is Background)
                    throw new InvalidOperationException($"Use {nameof(Background)} to set a background.");

                base.Add(drawable);
            }
        }
    }
}
