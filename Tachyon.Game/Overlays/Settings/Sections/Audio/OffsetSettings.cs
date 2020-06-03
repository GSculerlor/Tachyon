using osu.Framework.Allocation;
using osu.Framework.Audio;
using osu.Framework.Graphics;
using Tachyon.Game.Configuration;
using Tachyon.Game.Graphics.UserInterface;
using Tachyon.Game.Overlays.Settings.Items;

namespace Tachyon.Game.Overlays.Settings.Sections.Audio
{
    public class OffsetSettings : SettingsSubsection
    {
        protected override string Header => "Offset";

        [BackgroundDependencyLoader]
        private void load(TachyonConfigManager config)
        {
            Children = new Drawable[]
            {
                new SettingsSlider<double, OffsetSlider>
                {
                    LabelText = "Audio offset",
                    Bindable = config.GetBindable<double>(TachyonSetting.AudioOffset),
                    KeyboardStep = 1f
                },
            };
        }
        
        private class OffsetSlider : TachyonSliderBar<double>
        {
            public override string TooltipText => Current.Value.ToString(@"0ms");
        }
    }
}
