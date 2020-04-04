using System;

namespace Tachyon.Game.Beatmaps.Objects
{
    /// <summary>
    /// Hit object types are stored in an 8-bit integer where each bit is a flag with special meaning.
    /// The base hit object type is given by bits 0, 1, 3, and 7 (from least to most significant):
    /// - 0: Hit circle
    /// - 1: Slider
    /// - 3: Spinner
    /// </summary>
    [Flags]
    internal enum HitObjectType
    {
        Circle = 1,
        Slider = 1 << 1,
        NewCombo = 1 << 2,
        Spinner = 1 << 3,
        ComboOffset = (1 << 4) | (1 << 5) | (1 << 6)
    }
}