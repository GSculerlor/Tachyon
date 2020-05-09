using System;
using osu.Framework.Graphics;
using Tachyon.Game.Graphics.UserInterface;

namespace Tachyon.Game.Overlays.Settings.Items
{
    public class SettingsEnumDropdown<T> : SettingsDropdown<T>
        where T : struct, Enum
    {
        protected override TachyonDropdown<T> CreateDropdown() => new DropdownControl();

        protected new class DropdownControl : TachyonEnumDropdown<T>
        {
            public DropdownControl()
            {
                Margin = new MarginPadding { Top = 5 };
                RelativeSizeAxes = Axes.X;
            }
        }
    }
}
