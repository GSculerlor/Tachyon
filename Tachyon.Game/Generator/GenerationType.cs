using System.ComponentModel;

namespace Tachyon.Game.Generator
{
    public enum GenerationType
    {
        [Description("Random Note Placement")]
        Random,
        
        [Description("Ganen's Big Brain Note Placement")]
        AudioBased
    }
}
