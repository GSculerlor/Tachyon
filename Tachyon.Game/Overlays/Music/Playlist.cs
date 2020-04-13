using System;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osuTK;
using Tachyon.Game.Beatmaps;
using Tachyon.Game.Graphics.Containers;

namespace Tachyon.Game.Overlays.Music
{
    public class Playlist : TachyonRearrangeableListContainer<BeatmapSetInfo>
    {
        public readonly Bindable<BeatmapSetInfo> SelectedSet = new Bindable<BeatmapSetInfo>();
        
        public Action<BeatmapSetInfo> RequestSelection;
        
        public new MarginPadding Padding
        {
            get => base.Padding;
            set => base.Padding = value;
        }

        protected override TachyonRearrangeableListItem<BeatmapSetInfo> CreateListItemDrawable(BeatmapSetInfo item) =>
            new PlaylistItem(item)
            {
                SelectedSet = { BindTarget = SelectedSet },
                RequestSelection = set => RequestSelection?.Invoke(set)
            };
        
        protected override FillFlowContainer<RearrangeableListItem<BeatmapSetInfo>> CreateListFillFlowContainer() => new SearchContainer<RearrangeableListItem<BeatmapSetInfo>>
        {
            Spacing = new Vector2(0, 3),
            LayoutDuration = 200,
            LayoutEasing = Easing.OutQuint,
        };
    }
}