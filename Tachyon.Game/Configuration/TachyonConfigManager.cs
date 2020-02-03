using osu.Framework.Configuration;
using osu.Framework.Platform;

namespace Tachyon.Game.Configuration
{
    public class TachyonConfigManager : IniConfigManager<TachyonSetting>
    {
        protected override void InitialiseDefaults()
        {
            Set(TachyonSetting.ShowFpsDisplay, false);
            
            Set(TachyonSetting.UIScale, 1f, 0.8f, 1.6f, 0.01f);
        }

        public TachyonConfigManager(Storage storage) : base(storage)
        {
        }
    }

    public enum TachyonSetting
    {
        ShowFpsDisplay,
        UIScale,
    }
}