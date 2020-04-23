using System;
using System.Collections.Generic;
using System.Linq;
using osu.Framework.Extensions.IEnumerableExtensions;
using Tachyon.Game.Beatmaps;

namespace Tachyon.Game.Screens.Select.Carousel
{
    public class CarouselBeatmapSet : CarouselGroupEagerSelect
    {
        public IEnumerable<CarouselBeatmap> Beatmaps => InternalChildren.OfType<CarouselBeatmap>();

        public BeatmapSetInfo BeatmapSet;

        public CarouselBeatmapSet(BeatmapSetInfo beatmapSet)
        {
            BeatmapSet = beatmapSet ?? throw new ArgumentNullException(nameof(beatmapSet));

            beatmapSet.Beatmaps
                      .Select(b => new CarouselBeatmap(b))
                      .ForEach(AddChild);
        }

        protected override DrawableCarouselItem CreateDrawableRepresentation() => new DrawableCarouselBeatmapSet(this);

        protected IEnumerable<BeatmapInfo> ValidBeatmaps => Beatmaps.Where(b => !b.Filtered.Value || b.State.Value == CarouselItemState.Selected).Select(b => b.Beatmap);
        
        public override string ToString() => BeatmapSet.ToString();
    }
}
