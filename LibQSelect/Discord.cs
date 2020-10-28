using DiscordRPC;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace LibQSelect
{
    public class Discord
    {
        protected QSelect select;
        protected DiscordRpcClient rpc;

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
                RichPresence presence = new RichPresence().WithDetails(binary.Alias).WithState($"playing {mod.Alias}").WithAssets(quake);
                rpc.SetPresence(presence);
            }
            else
            {
                rpc.SetPresence(new RichPresence().WithDetails("Choosing game...").WithAssets(quake));
            }
        }
    }
}
