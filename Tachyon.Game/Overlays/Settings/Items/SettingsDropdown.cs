using System.Collections.Generic;
using System.Linq;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using Tachyon.Game.Graphics.UserInterface;

namespace Tachyon.Game.Overlays.Settings.Items
{
    public class SettingsDropdown<T> : SettingsItem<T>
    {
        protected new TachyonDropdown<T> Control => (TachyonDropdown<T>)base.Control;

        public IEnumerable<T> Items
        {
            get => Control.Items;
            set => Control.Items = value;
        }

        public IBindableList<T> ItemSource
        {
            get => Control.ItemSource;
            set => Control.ItemSource = value;
        }

        public override IEnumerable<string> FilterTerms => base.FilterTerms.Concat(Control.Items.Select(i => i.ToString()));

        protected sealed override Drawable CreateControl() => CreateDropdown();

        protected virtual TachyonDropdown<T> CreateDropdown() => new DropdownControl();

        protected class DropdownControl : TachyonDropdown<T>
        {
            public DropdownControl()
            {
                Margin = new MarginPadding { Top = 5 };
                RelativeSizeAxes = Axes.X;
            }
        }
    }
}
