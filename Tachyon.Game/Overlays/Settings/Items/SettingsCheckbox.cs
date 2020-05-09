using osu.Framework.Graphics;
using Tachyon.Game.Graphics.UserInterface;

namespace Tachyon.Game.Overlays.Settings.Items
{
    public class SettingsCheckbox : SettingsItem<bool>
    {
        private string labelText;

        protected override Drawable CreateControl() => new TachyonCheckbox();

        public override string LabelText
        {
            get => labelText;
            set => ((TachyonCheckbox)Control).LabelText = labelText = value;
        }
    }
}
