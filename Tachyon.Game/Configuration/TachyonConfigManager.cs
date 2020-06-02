using osu.Framework.Configuration;
using osu.Framework.Platform;
using Tachyon.Game.Generator;

namespace Tachyon.Game.Configuration
{
    public class TachyonConfigManager : IniConfigManager<TachyonSetting>
    {
        protected override void InitialiseDefaults()
        {
            Set(TachyonSetting.SuppressFail, false);
            
            Set(TachyonSetting.ShowFpsDisplay, false);
            
            Set(TachyonSetting.UIScale, 1f, 0.8f, 1.6f, 0.01f);

            Set(TachyonSetting.GenerationType, GenerationType.Random);

            Set(TachyonSetting.DivisorValue, DivisorValue.Two);
        }

        public TachyonConfigManager(Storage storage) : base(storage)
        {
        }
    }

    public enum TachyonSetting
    {
        SuppressFail,
        ShowFpsDisplay,
        UIScale,
        GenerationType,
        DivisorValue
    }
}