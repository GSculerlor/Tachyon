using System;

namespace Tachyon.Game.Graphics.UserInterface
{
    public class TachyonEnumDropdown<T> : TachyonDropdown<T>
        where T : struct, Enum
    {
        public TachyonEnumDropdown()
        {
            Items = (T[])Enum.GetValues(typeof(T));
        }
    }
}
