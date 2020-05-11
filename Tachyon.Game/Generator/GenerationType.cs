using System.ComponentModel;

namespace Tachyon.Game.Generator
{
    public enum GenerationType
    {
        [Description("Random Patterning")]
        Random,
        
        [Description("Ganen's Big Brain Algorithm Patterning")]
        AudioBased
    }
}
