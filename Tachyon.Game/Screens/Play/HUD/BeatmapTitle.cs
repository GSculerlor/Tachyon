using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using Tachyon.Game.Beatmaps;
using Tachyon.Game.Graphics;
using Tachyon.Game.Graphics.Sprites;

namespace Tachyon.Game.Screens.Play.HUD
{
    public class BeatmapTitle : Container
    {
        [BackgroundDependencyLoader]
        private void load(IBindable<WorkingBeatmap> beatmap)
        {
            Children = new Drawable[]
            {
                new TachyonSpriteText
                {
                    Font = TachyonFont.Default.With(size: 24, weight: FontWeight.SemiBold),
                    Text = beatmap.Value.ToString()
                }
            };
            AutoSizeAxes = Axes.Both;
        }
    }
}
