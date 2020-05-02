using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using osu.Framework.Logging;
using osu.Framework.Platform;
using osuTK.Input;
using Tachyon.Game;

namespace Tachyon.Desktop
{
    public class TachyonGameDesktop : TachyonGame
    {
        public override void SetHost(GameHost host)
        {
            base.SetHost(host);
            
            if (host.Window is DesktopGameWindow desktopWindow)
            {
                desktopWindow.CursorState |= CursorState.Hidden;
                
                //desktopWindow.SetIconFromStream(Assembly.GetExecutingAssembly().GetManifestResourceStream(GetType(), "lazer.ico"));
                desktopWindow.Title = Name;

                desktopWindow.FileDrop += fileDrop;
            }
        }

        protected override void LoadComplete()
        {
            base.LoadComplete();
            
            LoadComponentAsync(new DiscordRichPresence(), Add);
        }

        private void fileDrop(object sender, FileDropEventArgs e)
        {
            var filePaths = e.FileNames;

            var firstExtension = Path.GetExtension(filePaths.First());

            if (filePaths.Any(f => Path.GetExtension(f) != firstExtension)) return;

            Task.Factory.StartNew(() => Import(filePaths), TaskCreationOptions.LongRunning);
        }
    }
}