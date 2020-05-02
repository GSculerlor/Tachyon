using System;
using System.Collections.Generic;
using System.Linq;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Input.Events;
using osu.Framework.Localisation;
using osuTK.Graphics;
using Tachyon.Game.Beatmaps;
using Tachyon.Game.Graphics;
using Tachyon.Game.Graphics.Containers;

namespace Tachyon.Game.Overlays.Music
{
    public class PlaylistItem : TachyonRearrangeableListItem<BeatmapSetInfo>
    {
        public readonly Bindable<BeatmapSetInfo> SelectedSet = new Bindable<BeatmapSetInfo>();
        
        private TextFlowContainer text;
        private IEnumerable<Drawable> titleSprites;
        private ILocalisedBindableString title;
        private ILocalisedBindableString artist;
        private Color4 selectedColor;
        private Color4 artistColor;
        public Action<BeatmapSetInfo> RequestSelection;
        
        public PlaylistItem(BeatmapSetInfo item) : base(item)
        {
            Padding = new MarginPadding { Left = 5 };
        }
        
        [BackgroundDependencyLoader]
        private void load(TachyonColor colors, LocalisationManager localisation)
        {
            selectedColor = colors.Secondary;
            artistColor = colors.Gray9;

            title = localisation.GetLocalisedString(new LocalisedString((Model.Metadata.TitleUnicode, Model.Metadata.Title)));
            artist = localisation.GetLocalisedString(new LocalisedString((Model.Metadata.ArtistUnicode, Model.Metadata.Artist)));
        }
        protected override void LoadComplete()
        {
            base.LoadComplete();

            artist.BindValueChanged(_ => recreateText(), true);

            SelectedSet.BindValueChanged(set =>
            {
                if (set.OldValue?.Equals(Model) != true && set.NewValue?.Equals(Model) != true)
                    return;

                foreach (Drawable s in titleSprites)
                    s.FadeColour(set.NewValue.Equals(Model) ? selectedColor : Color4.White, 100f);
            }, true);
        }
        
        protected override Drawable CreateContent() => text = new TachyonTextFlowContainer
        {
            RelativeSizeAxes = Axes.X,
            AutoSizeAxes = Axes.Y,
        };
        
        protected override bool OnClick(ClickEvent e)
        {
            RequestSelection?.Invoke(Model);
            return true;
        }
        
        private void recreateText()
        {
            text.Clear();

            titleSprites = text.AddText(title.Value, sprite => sprite.Font = TachyonFont.GetFont(size: 18, weight: FontWeight.Bold)).OfType<SpriteText>();

            text.AddText(@" by " + artist.Value + @" ", sprite =>
            {
                sprite.Font = TachyonFont.GetFont(size: 18, weight: FontWeight.Bold);
                sprite.Colour = artistColor;
            });
        }
    }
}