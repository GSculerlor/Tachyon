using osu.Framework.Allocation;
using osu.Framework.Configuration;
using osu.Framework.Graphics;
using Tachyon.Game.Configuration;
using Tachyon.Game.Generator;
using Tachyon.Game.Overlays.Settings.Items;

namespace Tachyon.Game.Overlays.Settings.Sections.BeatmapGenerator
{
    public class PatternGeneratorSettings : SettingsSubsection
    {
        protected override string Header => "Pattern Generator";

        [BackgroundDependencyLoader]
        private void load(FrameworkConfigManager config, TachyonConfigManager tachyonConfig)
        {
            Children = new Drawable[]
            {
                new SettingsEnumDropdown<GenerationType>
                {
                    LabelText = "Pattern Generation Algorithm",
                    Bindable = tachyonConfig.GetBindable<GenerationType>(TachyonSetting.GenerationType)
                },
                
                new SettingsEnumDropdown<DivisorValue>
                {
                    LabelText = "Divisor Value",
                    Bindable = tachyonConfig.GetBindable<DivisorValue>(TachyonSetting.DivisorValue)
                }, 
            };
        }
    }
}
