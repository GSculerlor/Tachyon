using System;
using System.Linq;
using osu.Framework;
using osu.Framework.Platform;
using Tachyon.Game.Tests;

namespace Tachyon.Presentation
{
    public static class Program
    {
        [STAThread]
        public static void Main(string[] args)
        {
            using (DesktopGameHost host = Host.GetSuitableHost(@"Presentasi Tugas Akhir", true))
            {
                switch (args.FirstOrDefault() ?? string.Empty)
                {
                    default:
                        host.Run(new Presentation());
                        break;

                    case "--tests":
                        host.Run(new TachyonTestBrowser());
                        break;
                }
            }
        }
    }
}
