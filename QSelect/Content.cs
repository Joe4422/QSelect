using CreateMaps;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace QSelect
{
    public class Content
    {
        public string Directory { get; }

        public string Alias { get; set; } = null;

        public List<Content> Dependencies { get; } = new List<Content>();

        public Content(string directory, string alias = null)
        {
            Directory = directory;
            Alias = alias;
        }

        public void LoadContent(IAppSettings settings)
        {
            JunctionPoint.Create($"{settings.GameDirectory}/{Directory}", $"{settings.ContentDirectory}/{Directory}", true);

            if (settings.SyncQuakeConfig)
            {
                File.Copy(settings.QuakeConfigPath, $"{settings.GameDirectory}/{Directory}/config.cfg", true);
            }

            Dependencies.ForEach((x) => x.LoadContent(settings));
        }

        public void UnloadContent(IAppSettings settings)
        {
            if (settings.SyncQuakeConfig)
            {
                File.Copy($"{settings.GameDirectory}/{Directory}/config.cfg", settings.QuakeConfigPath, true);
                File.Delete($"{settings.GameDirectory}/{Directory}/config.cfg");
            }
            JunctionPoint.Delete($"{settings.GameDirectory}/{Directory}");

            bool syncConfig = settings.SyncQuakeConfig;
            settings.SyncQuakeConfig = false;
            Dependencies.ForEach((x) => x.UnloadContent(settings));
            settings.SyncQuakeConfig = syncConfig;
        }

        public override string ToString()
        {
            if (Alias == null) return Directory;
            else return $"{Alias} ({Directory})";
        }
    }
}
