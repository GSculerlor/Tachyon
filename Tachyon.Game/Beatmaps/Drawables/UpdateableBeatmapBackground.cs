using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using Tachyon.Game.Graphics;

namespace Tachyon.Game.Beatmaps.Drawables
{
    public class UpdateableBeatmapBackground : Container
    {
        private Drawable displayedCover;

        private BeatmapSetInfo beatmapSet;
        
        public BeatmapSetInfo BeatmapSet
        {
            get => beatmapSet;
            set
            {
                if (value == beatmapSet) return;

                beatmapSet = value;

                if (IsLoaded)
                    updateCover();
            }
        }

        public UpdateableBeatmapBackground()
        {
            Child = new Box
            {
                RelativeSizeAxes = Axes.Both,
                Colour = TachyonColor.Gray(0.2f),
            };
        }

        protected override void LoadComplete()
        {
            base.LoadComplete();
            updateCover();
        }

        private void updateCover()
        {
            displayedCover?.FadeOut(400);
            displayedCover?.Expire();
            displayedCover = null;

            if (beatmapSet != null)
            {
                BeatmapSetCover cover;

                Add(displayedCover = new DelayedLoadWrapper(
                    cover = new BeatmapSetCover(beatmapSet)
                    {
                        Anchor = Anchor.Centre,
                        Origin = Anchor.Centre,
                        RelativeSizeAxes = Axes.Both,
                        FillMode = FillMode.Fill,
                    })
                );

                cover.OnLoadComplete += d => d.FadeInFromZero(400, Easing.Out);
            }
        }
    }
}
