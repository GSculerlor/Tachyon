using System.ComponentModel;

namespace Tachyon.Game.GameModes.Scoring
{
    public enum HitResult
    {
        [Description(@"")]
        None,

        [Description(@"Miss")]
        Miss,

        [Description(@"Late")]
        Late,

        [Description(@"Perfect")]
        Perfect,
    }
}