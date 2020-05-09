using System.Threading.Tasks;
using Foundation;
using osu.Framework.iOS;
using UIKit;

namespace Tachyon.iOS
{
    [Register("AppDelegate")]
    public class AppDelegate : GameAppDelegate
    {
        private TachyonGameIOS game;

        protected override osu.Framework.Game CreateGame() => game = new TachyonGameIOS();

        public override bool OpenUrl(UIApplication app, NSUrl url, NSDictionary options)
        {
            if (url.IsFileUrl)
                Task.Run(() => game.Import(url.Path));
            return true;
        }
    }
}