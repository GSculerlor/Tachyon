using System;

namespace Tachyon.Game.Beatmaps.Objects
{
    [Flags]
    internal enum EffectFlags
    {
        None = 0,
        Kiai = 1,
        OmitFirstBarLine = 8
    }
}