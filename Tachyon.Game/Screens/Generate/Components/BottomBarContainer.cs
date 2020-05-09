using osu.Framework.Allocation;
using osu.Framework.Audio.Track;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using Tachyon.Game.Beatmaps;
using Tachyon.Game.Graphics;

namespace Tachyon.Game.Screens.Generate.Components
{
    public class BottomBarContainer : Container
    {
        private const float corner_radius = 5;
        private const float contents_padding = 15;

        protected readonly IBindable<WorkingBeatmap> Beatmap = new Bindable<WorkingBeatmap>();
        protected Track Track => Beatmap.Value.Track;

        private readonly Drawable background;
        private readonly Container content;

        protected override Container<Drawable> Content => content;

        public BottomBarContainer()
        {
            Masking = true;
            CornerRadius = corner_radius;

            InternalChildren = new[]
            {
                background = new Box { RelativeSizeAxes = Axes.Both },
                content = new Container
                {
                    RelativeSizeAxes = Axes.Both,
                    Padding = new MarginPadding { Horizontal = contents_padding },
                }
            };
        }

        [BackgroundDependencyLoader]
        private void load(IBindable<WorkingBeatmap> beatmap, TachyonColor colors)
        {
            Beatmap.BindTo(beatmap);
            background.Colour = colors.Gray1;
        }
    }
}
