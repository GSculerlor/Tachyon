using Tachyon.Game.Rulesets.Judgements;

namespace Tachyon.Game.Rulesets.Objects
{
    public class Note : TachyonHitObject
    {
        /// <summary>
        /// The <see cref="NoteType"/> that actuates this <see cref="Note"/>.
        /// </summary>
        public NoteType Type { get; set; }
    }
}
