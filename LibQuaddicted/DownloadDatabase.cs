using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace LibQuaddicted
{
    public class DownloadDatabase
    {
        public string DatabaseFile { get; }

        protected List<ModFile> downloadedMods;
        protected string modsDir;
        protected JObject root;
        protected string dbPath;

        public DownloadDatabase(ModDatabase modDb, string dbPath, string modsDir)
        {
            this.modsDir = modsDir;
            this.dbPath = dbPath;

            if (File.Exists(dbPath))
            {
                root = JObject.Parse(File.ReadAllText(dbPath));

                if (root.ContainsKey("Downloads"))
                {
                    foreach (ModFile mod in modDb.ModFiles.Values)
                    {
                        if (root["Downloads"][mod.Id] != null)
                        {
                            mod.Directory = (string)root["Downloads"][mod.Id]["Directory"];
                        }
                    }
                }
            }
            else
            {
                File.Create(dbPath);
            }
        }

        public async Task<bool> DownloadModAsync(ModFile modFile, string downloadDir)
        {
            string url = $"https://www.quaddicted.com/filebase/{modFile.Id}.zip";

            if (modFile.Downloaded) return true;
            Directory.CreateDirectory(downloadDir);

            using (WebClient client = new WebClient())
            {
                try
                {
                    await client.DownloadFileTaskAsync(new Uri(url), $"{downloadDir}/{modFile.Id}.zip");
                }
                catch (Exception e)
                {
                    return false;
                }
            }

            // Determine output path
            string outputPath;
            string outputTopLevel;
            if (modFile.TechInfo.ZipBaseDir.Contains("id1/maps"))
            {
                outputPath = $"{modsDir}/{modFile.Id}/maps/";
                outputTopLevel = $"{modsDir}/{modFile.Id}/";
            }
            else if (modFile.TechInfo.ZipBaseDir.Contains("id1"))
            {
                outputPath = $"{modsDir}/{modFile.Id}/";
                outputTopLevel = $"{modsDir}/{modFile.Id}/";
            }
            else
            {
                outputPath = $"{modsDir}/{modFile.TechInfo.ZipBaseDir}/";
                outputTopLevel = $"{modsDir}/{modFile.TechInfo.ZipBaseDir}/";
            }

            if (Directory.Exists(outputTopLevel)) Directory.Delete(outputTopLevel, true);

            Directory.CreateDirectory(outputPath);
            ZipFile.ExtractToDirectory($"{downloadDir}/{modFile.Id}.zip", outputPath);

            if (root["Downloads"][modFile.Id] != null)
            {
                root["Downloads"][modFile.Id]["Directory"] = outputTopLevel;
            }
            else
            {
                JObject modPath = new JObject
                {
                    { "Directory", outputTopLevel }
                };
                (root["Downloads"] as JObject).Add(modFile.Id, modPath);
            }

            await WriteDatabaseAsync();

            return true;
        }

        public async Task<bool> DeleteModAsync(ModFile modFile)
        {
            if (modFile.Downloaded) return true;

            try
            {
                await Task.Run(() => Directory.Delete(modFile.Directory));
            } 
            catch (Exception)
            {
                return false;
            }

            (root["Downloads"] as JObject).Remove(modFile.Id);
            await WriteDatabaseAsync();

            return true;
        }

        protected async Task WriteDatabaseAsync()
        {
            await File.WriteAllTextAsync(dbPath, root.ToString());
        }
    }
}
