using osu.Framework.Platform;

namespace Tachyon.Game.Tests
{
    public class CleanHeadlessGameHost : HeadlessGameHost
    {
        public CleanHeadlessGameHost(string gameName = @"", bool bindIPC = false, bool realtime = true)
            : base(gameName, bindIPC, realtime)
        {
        }
        
        protected override void SetupForRun()
        {
            base.SetupForRun();
            Storage.DeleteDirectory(string.Empty);
        }
    }
}