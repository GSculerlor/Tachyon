using System;
using osu.Framework.Graphics.UserInterface;

namespace Tachyon.Game.Graphics.UserInterface
{
    public class TachyonMenuItem : MenuItem
    {
        public readonly MenuItemType Type;

        public TachyonMenuItem(string text, MenuItemType type = MenuItemType.Standard)
            : this(text, type, null)
        {
        }

        public TachyonMenuItem(string text, MenuItemType type, Action action)
            : base(text, action)
        {
            Type = type;
        }
    }
    
    public enum MenuItemType
    {
        Standard,
        Highlighted,
        Destructive,
    }
}
