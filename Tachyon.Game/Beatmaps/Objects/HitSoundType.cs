using System;

namespace Tachyon.Game.Beatmaps.Objects
{
    [Flags]
    internal enum HitSoundType
    {
        None = 0,
        Normal = 1,
        Whistle = 2,
        Finish = 4,
        Clap = 8
    }
}