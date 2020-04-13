using System.Linq;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Extensions.Color4Extensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Effects;
using osu.Framework.Graphics.Shapes;
using osuTK.Graphics;
using Tachyon.Game.Beatmaps;
using Tachyon.Game.Graphics;

namespace Tachyon.Game.Overlays.Music
{
    public class PlaylistOverlay : Container
    {
        private const float playlist_height = 200;
        
        public IBindableList<BeatmapSetInfo> BeatmapSets => beatmapSets;

        private readonly BindableList<BeatmapSetInfo> beatmapSets = new BindableList<BeatmapSetInfo>();

        private readonly Bindable<WorkingBeatmap> beatmap = new Bindable<WorkingBeatmap>();
        
        private Playlist beatmapList;
        
        [Resolved]
        private BeatmapManager beatmaps { get; set; }

        public PlaylistOverlay()
        {
            Height = playlist_height;
        }
        
        [BackgroundDependencyLoader]
        private void load(TachyonColor colors, Bindable<WorkingBeatmap> beatmap)
        {
            this.beatmap.BindTo(beatmap);

            Children = new Drawable[]
            {
                new Container
                {
                    RelativeSizeAxes = Axes.Both,
                    CornerRadius = 5,
                    Masking = true,
                    EdgeEffect = new EdgeEffectParameters
                    {
                        Type = EdgeEffectType.Shadow,
                        Colour = Color4.Black.Opacity(40),
                        Radius = 5,
                    },
                    Children = new Drawable[]
                    {
                        new Box
                        {
                            Colour = colors.Gray3,
                            RelativeSizeAxes = Axes.Both,
                        },
                        beatmapList = new Playlist
                        {
                            RelativeSizeAxes = Axes.Both,
                            Padding = new MarginPadding { Horizontal = 16, Vertical = 10 },
                            RequestSelection = itemSelected
                        }
                    },
                },
            };
        }
        
        protected override void LoadComplete()
        {
            base.LoadComplete();

            beatmapList.Items.BindTo(beatmapSets);
            beatmap.BindValueChanged(working => beatmapList.SelectedSet.Value = working.NewValue.BeatmapSetInfo, true);
        }
        
        private void itemSelected(BeatmapSetInfo set)
        {
            if (set.ID == (beatmap.Value?.BeatmapSetInfo?.ID ?? -1))
            {
                beatmap.Value?.Track?.Seek(0);
                return;
            }

            beatmap.Value = beatmaps.GetWorkingBeatmap(set.Beatmaps.First());
            beatmap.Value.Track.Restart();
        }
    }
}
