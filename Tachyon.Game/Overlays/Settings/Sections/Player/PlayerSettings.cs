using osu.Framework.Allocation;
using osu.Framework.Configuration;
using osu.Framework.Graphics;
using osu.Framework.Platform;
using Tachyon.Game.Configuration;
using Tachyon.Game.Overlays.Settings.Items;

namespace Tachyon.Game.Overlays.Settings.Sections.Player
{
    public class PlayerSettings : SettingsSubsection
    {
        protected override string Header => "Player";

        [BackgroundDependencyLoader]
        private void load(TachyonConfigManager tachyonConfig)
        {
            Children = new Drawable[]
            {
                new SettingsCheckbox
                {
                    LabelText = "Suppress Fail",
                    Bindable = tachyonConfig.GetBindable<bool>(TachyonSetting.SuppressFail)
                },
            };
        }
    }
}
