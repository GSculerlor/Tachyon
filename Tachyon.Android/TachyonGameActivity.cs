using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Views;
using osu.Framework.Android;

namespace Tachyon.Android
{
    [Activity(Theme = "@android:style/Theme.NoTitleBar", MainLauncher = true, ScreenOrientation = ScreenOrientation.FullSensor, 
        ConfigurationChanges = ConfigChanges.Orientation | ConfigChanges.ScreenSize,
        HardwareAccelerated = true)]
    public class TachyonGameActivity : AndroidGameActivity
    {
        protected override osu.Framework.Game CreateGame() => new TachyonGameAndroid();

        protected override void OnCreate(Bundle savedInstanceState)
        {
            System.Environment.CurrentDirectory = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);

            base.OnCreate(savedInstanceState);

            Window.AddFlags(WindowManagerFlags.Fullscreen);
            Window.AddFlags(WindowManagerFlags.KeepScreenOn);
        }
    }
}