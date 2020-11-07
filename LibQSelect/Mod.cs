using CreateMaps;
using LibQuaddicted;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace LibQSelect
{
    public class Mod
    {
        #region Properties
        /// <summary>
        /// Directory name and (if downloaded) Quaddicted ID of this mod.
        /// </summary>
        public string Id { get; }

        /// <summary>
        /// Quaddicted database entry this mod corresponds to. Can be null.
        /// </summary>
        public ModDatabaseItem DatabaseItem { get; }
        #endregion

        #region Constructors
        /// <summary>
        /// Creates a new Mod with the given ID and Quaddicted database entry.
        /// </summary>
        /// <param name="id">The mod's ID.</param>
        /// <param name="databaseItem">The Quaddicted database entry reference. Can be null.</param>
        /// <exception cref="ArgumentNullException"/>
        public Mod(string id, ModDatabaseItem databaseItem = null)
        {
            Id = id ?? throw new ArgumentNullException(nameof(id));
            DatabaseItem = databaseItem;
        }
        #endregion

        #region Methods
        /// <summary>
        /// Finds all the mods in the given directory, creates Mod objects representing them, matches them with their Quaddicted database entry (if possible) and returns them in a List.
        /// </summary>
        /// <param name="modDir">The directory to look for mods.</param>
        /// <param name="db">The Quaddicted database to use for matching.</param>
        /// <returns>A list of all the mods found in the given directory.</returns>
        /// <exception cref="ArgumentNullException"/>
        /// <exception cref="DirectoryNotFoundException"/>
        public static List<Mod> FindMods(string modDir, ModDatabase db)
        {
            // Check arguments are not null
            if (modDir is null) throw new ArgumentNullException(nameof(modDir));
            if (db is null) throw new ArgumentNullException(nameof(db));

            // Check directory exists
            if (!Directory.Exists(modDir)) throw new DirectoryNotFoundException("Specified mod directory was not found.");

            // Create mod list
            List<Mod> mods = new List<Mod>();

            // Load mods by directory name
            foreach (string id in Directory.GetDirectories(modDir))
            {
                Mod mod = new Mod(id, db[id]);

                mods.Add(mod);
            }

            return mods;
        }

        /// <summary>
        /// Indicates if the mod is readied for a given source port.
        /// </summary>
        /// <param name="sourcePort">The source port to check this mod has been readied for.</param>
        /// <returns>Whether the mod has been readied for the given source port.</returns>
        /// <exception cref="ArgumentNullException"/>
        public bool IsReadied(SourcePortOld sourcePort)
        {
            // Check sourcePort is not null
            if (sourcePort is null) throw new ArgumentNullException(nameof(sourcePort));

            return sourcePort.ReadiedMods.Contains(this);
        }

        /// <summary>
        /// Readies this mod by creating a symlink to its content directory in the source port's main directory.
        /// </summary>
        /// <param name="settings">Settings to determine where source ports and mods are stored.</param>
        /// <param name="sourcePort">The source port to ready the mod for.</param>
        /// <exception cref="ArgumentNullException"/>
        /// <exception cref="PlatformNotSupportedException"/>
        /// <exception cref="IOException"/>
        public void ReadyMod(IAppSettings settings, SourcePortOld sourcePort)
        {
            // Check settings and sourcePort are not null
            if (settings is null) throw new ArgumentNullException(nameof(settings));
            if (sourcePort is null) throw new ArgumentNullException(nameof(settings));

            // Check if we're already ready - if so, no need to repeat
            if (IsReadied(sourcePort)) return;

            // Determine which symlink implementation to use
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                // Create junction from mod directory in source port directory
                JunctionPoint.Create($"{settings.SourcePortsDirectory}/{sourcePort.Directory}/{Id}", $"{settings.ModsDirectory}/{Id}", true);
            }
            else
            {
                throw new PlatformNotSupportedException("Symlinks on platforms other than Windows are currently not supported.");
            }

            // Try to copy Quake configuration from source port directory to mod directory
            if (settings.SyncQuakeConfig)
            {
                try
                {
                    File.Copy($"{settings.SourcePortsDirectory}/{sourcePort.Directory}/config.cfg", $"{settings.SourcePortsDirectory}/{sourcePort.Directory}/{Id}/config.cfg", true);
                }
                catch (Exception)
                {
                    throw new IOException("Error synchronizing Quake configuration.");
                }
            }

            sourcePort.ReadiedMods.Add(this);
        }

        /// <summary>
        /// Unreadies the current mod by removing the symlink between its content directory and the source port's main directory.
        /// </summary>
        /// <param name="settings">Settings to determine where source ports and mods are stored.</param>
        /// <param name="sourcePort">The source port to unready the mod for.</param>
        /// <exception cref="ArgumentNullException"/>
        /// <exception cref="IOException"/>
        /// <exception cref="PlatformNotSupportedException"/>
        public void UnreadyMod(IAppSettings settings, SourcePortOld sourcePort)
        {
            // Check settings and sourcePort are not null
            if (settings is null) throw new ArgumentNullException(nameof(settings));
            if (sourcePort is null) throw new ArgumentNullException(nameof(settings));

            // Check if we're already unreadied - if so, no need to repeat
            if (!IsReadied(sourcePort)) return;

            // Try to copy Quake configuration back to source port directory from mod directory, to preserve any changes made by the player
            if (settings.SyncQuakeConfig)
            {
                try
                {
                    File.Copy($"{settings.SourcePortsDirectory}/{sourcePort.Directory}/{Id}/config.cfg", $"{settings.SourcePortsDirectory}/{sourcePort.Directory}/config.cfg", true);
                    File.Delete($"{settings.SourcePortsDirectory}/{sourcePort.Directory}/{Id}/config.cfg");
                }
                catch (Exception)
                {
                    throw new IOException("Error synchronizing Quake configuration.");
                }
            }

            // Determine which symlink implementation to use
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                // Delete junction of mod directory in source port directory
                JunctionPoint.Delete($"{settings.SourcePortsDirectory}/{sourcePort.Directory}/{Id}");
            }
            else
            {
                throw new PlatformNotSupportedException("Symlinks on platforms other than Windows are currently not supported.");
            }

            // Double check directory is properly disposed of
            if (Directory.Exists($"{settings.SourcePortsDirectory}/{sourcePort.Directory}/{Id}")) Directory.Delete($"{settings.SourcePortsDirectory}/{sourcePort.Directory}/{Id}", true);

            sourcePort.ReadiedMods.Remove(this);
        }

        public override string ToString()
        {
            if (DatabaseItem == null) return Id;
            else return DatabaseItem.Title;
        }
        #endregion
    }
}
