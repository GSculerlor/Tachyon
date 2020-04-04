using System;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;

namespace Tachyon.Game.Beatmaps.Drawables
{
    public class UpdateableBeatmapBackgroundSprite : ModelBackedDrawable<BeatmapInfo>
    {
        public readonly Bindable<BeatmapInfo> Beatmap = new Bindable<BeatmapInfo>();
        
        protected override double LoadDelay => 500;
        private double unloadDelay => 10000;
        
        [Resolved]
        private BeatmapManager beatmaps { get; set; }

        public UpdateableBeatmapBackgroundSprite()
        {
            Beatmap.BindValueChanged(beatmap => Model = beatmap.NewValue);
        }

        protected override DelayedLoadWrapper CreateDelayedLoadWrapper(Func<Drawable> createContentFunc, double timeBeforeLoad)
        {
            return new DelayedLoadUnloadWrapper(createContentFunc, timeBeforeLoad, unloadDelay);
        }

        protected override Drawable CreateDrawable(BeatmapInfo model)
        {
            var drawable = loadBackgroundFromBeatmap(model);
            drawable.RelativeSizeAxes = Axes.Both;
            drawable.Anchor = Anchor.Centre;
            drawable.Origin = Anchor.Centre;
            drawable.FillMode = FillMode.Fill;

            return drawable;
        }

        private Drawable loadBackgroundFromBeatmap(BeatmapInfo model)
        {
            return model?.ID > 0
                ? new BeatmapBackgroundSprite(beatmaps.GetWorkingBeatmap(model))
                : new BeatmapBackgroundSprite(beatmaps.DefaultBeatmap);
        }
    }
}