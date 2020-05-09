using osu.Framework.Allocation;
using osu.Framework.Audio;
using osu.Framework.Graphics;
using Tachyon.Game.Overlays.Settings.Items;

namespace Tachyon.Game.Overlays.Settings.Sections.Audio
{
    public class VolumeSettings : SettingsSubsection
    {
        protected override string Header => "Volume";

        [BackgroundDependencyLoader]
        private void load(AudioManager audio)
        {
            Children = new Drawable[]
            {
                new SettingsSlider<double>
                {
                    LabelText = "Master",
                    Bindable = audio.Volume,
                    KeyboardStep = 0.01f,
                    DisplayAsPercentage = true
                },
                new SettingsSlider<double>
                {
                    LabelText = "Effect",
                    Bindable = audio.VolumeSample,
                    KeyboardStep = 0.01f,
                    DisplayAsPercentage = true
                },
                new SettingsSlider<double>
                {
                    LabelText = "Music",
                    Bindable = audio.VolumeTrack,
                    KeyboardStep = 0.01f,
                    DisplayAsPercentage = true
                },
            };
        }
    }
}
