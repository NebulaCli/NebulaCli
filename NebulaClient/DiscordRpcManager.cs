using DiscordRPC;
using DiscordRPC.Logging;
using System;
using System.Diagnostics;

namespace NebulaClient
{
    public class DiscordRpcManager : IDisposable
    {
        private DiscordRpcClient? client;
        private static DiscordRpcManager? _instance;
        public static DiscordRpcManager Instance => _instance ??= new DiscordRpcManager();
        private const string ClientId = "1467495679009099802";
        private System.Timers.Timer? callbackTimer;

        public void Initialize()
        {
            try
            {
                if (client != null && !client.IsDisposed) return;

                Debug.WriteLine("[RPC] Initializing Discord RPC...");
                client = new DiscordRpcClient(ClientId);
                
                // Set the logger
                client.Logger = new ConsoleLogger() { Level = LogLevel.Warning };

                // Subscribe to events
                client.OnReady += (sender, msg) => 
                {
                    Debug.WriteLine($"[RPC] Connected to discord as {msg.User.Username}");
                };

                client.OnError += (sender, msg) => 
                {
                    Debug.WriteLine($"[RPC] Discord Error: {msg.Message}");
                };

                // Initialize the connection
                client.Initialize();
                
                // Start a timer to call Invoke periodically
                callbackTimer = new System.Timers.Timer(2000);
                callbackTimer.Elapsed += (s, e) => client?.Invoke();
                callbackTimer.AutoReset = true;
                callbackTimer.Enabled = true;

                Debug.WriteLine("[RPC] Connection request sent and timer started.");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[RPC] Critical Init Error: {ex.Message}");
            }
        }

        public void UpdatePresence(string details, string state)
        {
            try 
            {
                if (client == null || client.IsDisposed) Initialize();

                var presence = new RichPresence()
                {
                    Details = details,
                    State = state,
                    Assets = new Assets()
                    {
                        LargeImageKey = "logo", // Ensure this key exists in your Discord App Assets
                        LargeImageText = "Nebula Client v1.0",
                        SmallImageKey = "logo",
                        SmallImageText = "Online"
                    },
                    Timestamps = Timestamps.Now
                };

                Debug.WriteLine($"[RPC] Updating presence: {details} - {state}");
                client?.SetPresence(presence);
                client?.Invoke(); // Force an update dispatch
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[RPC] Update Presence Error: {ex.Message}");
            }
        }

        public void Stop()
        {
            if (client != null)
            {
                client.ClearPresence();
                client.Dispose();
                client = null;
                Debug.WriteLine("[RPC] Stopped.");
            }
        }

        public void Dispose()
        {
            Stop();
        }
    }
}
