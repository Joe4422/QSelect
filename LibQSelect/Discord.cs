using DiscordRPC;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Timers;

namespace LibQSelect
{
    public class Discord
    {
        protected QSelect select;
        protected DiscordRpcClient rpc;
        protected RichPresence presence;
        protected Timer watcher = null;
        protected FileStream logfs = null;
        protected StreamReader logsr = null;

        public Discord(QSelect select)
        {
            this.select = select ?? throw new ArgumentNullException(nameof(select));

            rpc = new DiscordRpcClient("771089873947590666");
            rpc.Initialize();
        }

        public void Update(Binary binary = null, Mod mod = null)
        {
            Assets quake = new Assets
            {
                LargeImageKey = "quake"
            };
            if (binary != null & mod != null)
            {
                presence = new RichPresence().WithDetails($"{mod.Alias}").WithAssets(quake);
                rpc.SetPresence(presence);
                StartWatcher();
            }
            else
            {
                presence = new RichPresence().WithDetails("Choosing game...").WithAssets(quake);
                rpc.SetPresence(presence);
                if (watcher != null) watcher.Dispose();
                if (logfs != null) logfs.Dispose();
                if (logsr != null) logsr.Dispose();
                watcher = null;
                logfs = null;
                logsr = null;
            }
        }

        protected void StartWatcher()
        {
            if (watcher != null) watcher.Dispose();

            watcher = new Timer(15_000.0)
            {
                AutoReset = true
            };
            watcher.Elapsed += Watcher_Elapsed;

            watcher.Start();
        }

        private void Watcher_Elapsed(object sender, ElapsedEventArgs e)
        {
            watcher.Stop();
            if (File.Exists("qconsole.log"))
            {
                if (logfs == null || logsr == null)
                {
                    logfs = File.Open("qconsole.log", FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                    logsr = new StreamReader(logfs);
                }
                string[] data = logsr.ReadToEnd().Split("\r\n", StringSplitOptions.RemoveEmptyEntries);

                for (int i = data.Length - 1; i >= 0; i--)
                {
                    if (data[i].StartsWith("Using protocol "))
                    {
                        string mapname = data[i - 1][1..].ToUpper();

                        presence = presence.WithState($"in \"{mapname}\"");
                        rpc.SetPresence(presence);
                    }
                }
            }
            watcher.Start();
        }
    }
}
