using osu.Framework.Allocation;
using osu.Framework.Configuration;
using osu.Framework.Graphics;
using osu.Framework.Platform;
using Tachyon.Game.Configuration;
using Tachyon.Game.Overlays.Settings.Items;

namespace Tachyon.Game.Overlays.Settings.Sections.Graphics
{
    public class FrameLimiterSettings : SettingsSubsection
    {
        protected override string Header => "Frame Limiter";

        [BackgroundDependencyLoader]
        private void load(FrameworkConfigManager config, TachyonConfigManager tachyonConfig)
        {
            Children = new Drawable[]
            {
                new SettingsCheckbox
                {
                    LabelText = "Show FPS Display",
                    Bindable = tachyonConfig.GetBindable<bool>(TachyonSetting.ShowFpsDisplay)
                },
                new SettingsEnumDropdown<FrameSync>
                {
                    LabelText = "Frame limiter",
                    Bindable = config.GetBindable<FrameSync>(FrameworkSetting.FrameSync)
                },
                new SettingsEnumDropdown<ExecutionMode>
                {
                    LabelText = "Threading mode",
                    Bindable = config.GetBindable<ExecutionMode>(FrameworkSetting.ExecutionMode)
                }
            };
        }
    }
}
