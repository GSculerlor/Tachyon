using DiscordRPC;
using DiscordRPC.Message;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Logging;

namespace Tachyon.Desktop
{
    public class DiscordRichPresence : Component
    {
        private const string client_id = "489508874622074891";
        
        private DiscordRpcClient client;
        
        private readonly RichPresence presence = new RichPresence
        {
            Assets = new Assets { LargeImageKey = "tachyon_grani", }
        };
        
        [BackgroundDependencyLoader]
        private void load()
        {
            client = new DiscordRpcClient(client_id)
            {
                SkipIdenticalPresence = false
            };

            client.OnReady += onReady;
            client.OnConnectionFailed += (_, __) => client.Deinitialize();
            client.OnError += (_, e) => Logger.Log($"An error occurred with Discord RPC Client: {e.Code} {e.Message}", LoggingTarget.Network);
            
            client.Initialize();
        }
        
        protected override void Dispose(bool isDisposing)
        {
            client.Dispose();
            base.Dispose(isDisposing);
        }
        
        private void updateStatus()
        {
            if (!client.IsInitialized)
                return;

            presence.State = "Developing...";
            presence.Details = "Working on import and storing beatmaps";

            client.SetPresence(presence);
        }
        
        private void onReady(object _, ReadyMessage __)
        {
            Logger.Log("Discord RPC Client ready.", LoggingTarget.Network, LogLevel.Debug);
            updateStatus();
        }
    }
}
