using DiscordRPC;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Timers;

namespace LibQSelect
{
    public class DiscordRichPresenceHandler
    {
        #region Variables
        /// <summary>
        /// Discord Rich Presence client.
        /// </summary>
        protected DiscordRpcClient rpc;
        /// <summary>
        /// Current Rich Presence state.
        /// </summary>
        protected RichPresence presence;
        /// <summary>
        /// Timer used to update the map read from the log at regular intervals.
        /// </summary>
        protected Timer watcher = null;
        /// <summary>
        /// Quake console log filestream.
        /// </summary>
        protected FileStream logfs = null;
        /// <summary>
        /// Quake console log stream reader.
        /// </summary>
        protected StreamReader logsr = null;
        #endregion

        #region Constructors
        /// <summary>
        /// Creates a new Discord Rich Presence Handler.
        /// </summary>
        public DiscordRichPresenceHandler()
        {
            rpc = new DiscordRpcClient("771089873947590666");
            rpc.Initialize();
        }
        #endregion

        #region Methods
        /// <summary>
        /// Updates the Rich Presence status to show the current mod being played. If the mod is null, "Choosing game..." will be displayed instead.
        /// </summary>
        /// <param name="mod">The mod currently being played. Can be null.</param>
        /// <param name="startWatcher">If true, start watching the log file for map names.</param>
        public void SetActiveMod(Mod mod = null, bool startWatcher = true)
        {
            // Fetch the Quake logo asset for display.
            Assets quake = new Assets
            {
                LargeImageKey = "quake"
            };

            if (mod != null)
            {
                // Display mod details.
                presence = new RichPresence().WithDetails($"{mod}").WithAssets(quake);
                rpc.SetPresence(presence);

                if (startWatcher)
                {
                    StartWatcher();
                }
            }
            else
            {
                // Display default "Choosing game..." text.
                presence = new RichPresence().WithDetails("Choosing game...").WithAssets(quake);
                rpc.SetPresence(presence);

                // Dispose of log file watcher and streams.
                if (watcher != null) watcher.Dispose();
                if (logfs != null) logfs.Dispose();
                if (logsr != null) logsr.Dispose();
                watcher = null;
                logfs = null;
                logsr = null;
            }
        }

        /// <summary>
        /// Starts the console log map name watcher.
        /// </summary>
        protected void StartWatcher()
        {
            if (watcher != null) return;

            // Create a new timer with a 15 second interval.
            watcher = new Timer(15_000.0)
            {
                AutoReset = true
            };
            watcher.Elapsed += Watcher_Elapsed;

            watcher.Start();
        }

        private void Watcher_Elapsed(object sender, ElapsedEventArgs e)
        {
            // Stop the watcher while we're doing this to prevent elapsed events colliding
            watcher.Stop();
            
            // Check logfile exists
            if (File.Exists("qconsole.log"))
            {
                // If the file stream or stream reader are null, establish them
                if (logfs == null || logsr == null)
                {
                    try
                    {
                        logfs = File.Open("qconsole.log", FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                        logsr = new StreamReader(logfs);
                    }
                    catch (Exception)
                    {
                        return;
                    }
                }

                // Read all the new data from the stream reader and split it into lines
                string[] data = logsr.ReadToEnd().Split("\r\n", StringSplitOptions.RemoveEmptyEntries);

                // Find the last instance of the line starting "Using protocol ", and read the map name by getting the line above it
                for (int i = data.Length - 1; i >= 0; i--)
                {
                    if (data[i].StartsWith("Using protocol "))
                    {
                        // Ensure that i - 1 is not out of range
                        if (i - 1 >= 0)
                        {
                            string mapname = data[i - 1][1..].ToUpper();

                            presence = presence.WithState($"in \"{mapname}\"");
                            rpc.SetPresence(presence);
                        }
                    }
                }
            }
            watcher.Start();
        }
        #endregion
    }
}
