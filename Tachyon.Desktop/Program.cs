using osu.Framework;
using osu.Framework.Platform;
using Tachyon.Game;

namespace Tachyon.Desktop
{
    public static class Program
    {
        public static void Main()
        {
            using (GameHost host = Host.GetSuitableHost(@"Tachyon"))
            using (osu.Framework.Game game = new TachyonGameDesktop())
                host.Run(game);
        }
    }
}
