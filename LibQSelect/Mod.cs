using CreateMaps;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace LibQSelect
{
    public class Mod
    {
        public string Directory { get; }

        public string Alias { get; set; } = null;

        public List<Mod> Dependencies { get; } = new List<Mod>();

        public bool IsLoaded { get; protected set; } = false;

        public Mod(string directory, string alias = null)
        {
            Directory = directory;
            Alias = alias;

            if (alias == null) Alias = Directory;
        }

        public static List<Mod> LoadMods(QSelect select, JObject root)
        {
            List<Mod> mods = new List<Mod>();
            foreach (string directory in System.IO.Directory.GetDirectories(select.Settings.ModsDirectory).Select((x) => Path.GetFileName(x)))
            {
                Mod mod = new Mod(directory);

                if (root != null && root["ModMetadata"] != null && root["ModMetadata"][directory] != null)
                {
                    mod.Alias = (string)root["ModMetadata"][directory]["Alias"];
                }

                mods.Add(mod);
            }

            // Add mod dependency on id1
            Mod id1Mod = mods.Where((x) => x.Directory == "id1").First();
            foreach (Mod mod in mods.Where((x) => x.Directory != "id1"))
            {
                mod.Dependencies.Add(id1Mod);
            }

            // Move id1 to index 0
            mods.Remove(id1Mod);
            mods.Insert(0, id1Mod);

            return mods;
        }

        public void LoadMod(QSelect select, Binary binary)
        {
            if (IsLoaded) return;

            JunctionPoint.Create($"{select.Settings.BinariesDirectory}/{binary.Directory}/{Directory}", $"{select.Settings.ModsDirectory}/{Directory}", true);

            if (select.Settings.SyncQuakeConfig)
            {
                File.Copy($"{select.Settings.BinariesDirectory}/{binary.Directory}/config.cfg", $"{select.Settings.BinariesDirectory}/{binary.Directory}/{Directory}/config.cfg", true);
            }

            Dependencies.ForEach((x) => x.LoadMod(select, binary));

            IsLoaded = true;
        }

        public void UnloadMod(IAppSettings settings, Binary binary)
        {
            if (!IsLoaded) return;

            if (settings.SyncQuakeConfig)
            {
                File.Copy($"{settings.BinariesDirectory}/{binary.Directory}/{Directory}/config.cfg", $"{settings.BinariesDirectory}/{binary.Directory}/config.cfg", true);
                File.Delete($"{settings.BinariesDirectory}/{binary.Directory}/{Directory}/config.cfg");
            }
            JunctionPoint.Delete($"{settings.BinariesDirectory}/{binary.Directory}/{Directory}");
            if (System.IO.Directory.Exists($"{settings.BinariesDirectory}/{binary.Directory}/{Directory}")) System.IO.Directory.Delete($"{settings.BinariesDirectory}/{binary.Directory}/{Directory}", true);

            bool syncConfig = settings.SyncQuakeConfig;
            settings.SyncQuakeConfig = false;
            Dependencies.ForEach((x) => x.UnloadMod(settings, binary));
            settings.SyncQuakeConfig = syncConfig;

            IsLoaded = false;
        }

        public override string ToString()
        {
            if (Alias == Directory) return Directory;
            else return $"{Alias} ({Directory})";
        }
    }
}
