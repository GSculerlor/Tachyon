using System.ComponentModel.DataAnnotations.Schema;
using osu.Framework.Input.Bindings;
using Tachyon.Game.Database;

namespace Tachyon.Game.Input.Bindings
{
    [Table("KeyBinding")]
    public class DatabasedKeyBinding : KeyBinding, IHasPrimaryKey
    {
        public int ID { get; set; }

        public int? RulesetID { get; set; }

        public int? Variant { get; set; }

        [Column("Keys")]
        public string KeysString
        {
            get => KeyCombination.ToString();
            private set => KeyCombination = value;
        }

        [Column("Action")]
        public int IntAction
        {
            get => (int)Action;
            set => Action = value;
        }
    }
}
