using Microsoft.VisualBasic.FileIO;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using CreateMaps;
using System.Runtime.InteropServices;
using Config.Net;
using Newtonsoft.Json.Linq;

namespace QSelect
{
    class Program
    {
        private static readonly string configFile = "config.json";

        static void Main(string[] _)
        {
            // Load settings
            ConfigurationBuilder<IAppSettings> builder = new ConfigurationBuilder<IAppSettings>().UseJsonFile(configFile);
            IAppSettings settings = builder.Build();

            // Ensure settings are written to file
            settings.LastSelected = settings.LastSelected;
            settings.Executable = settings.Executable;
            settings.GameDirectory = settings.GameDirectory;
            settings.ContentDirectory = settings.ContentDirectory;
            settings.SyncQuakeConfig = settings.SyncQuakeConfig;
            settings.QuakeConfigPath = settings.QuakeConfigPath;

            // Load aliases
            JObject root = JObject.Parse(File.ReadAllText(configFile));
            List<ContentAlias> aliases = new List<ContentAlias>();
            if (root["Content"] != null && root["Content"]["Aliases"] != null)
            {
                foreach (JObject obj in (JArray)root["Content"]["Aliases"])
                {
                    aliases.Add(new ContentAlias((string)obj["Folder"], (string)obj["Alias"]));
                }
            }

            // Load content
            List<Content> contents = new List<Content>();
            foreach (string folder in Directory.GetDirectories(settings.ContentDirectory).Select((x) => Path.GetFileName(x)))
            {
                Content content = new Content(folder);

                if (aliases.Select((x) => x.Folder).Contains(folder))
                {
                    content.Alias = aliases.Where((x) => x.Folder == folder).First().Alias;
                }

                contents.Add(content);
            }

            // Add content dependency on id1
            Content quakeContent = contents.Where((x) => x.Directory == "id1").First();
            foreach(Content content in contents.Where((x) => x.Directory != "id1"))
            {
                content.Dependencies.Add(quakeContent);
            }

            // Move id1 to index 0
            contents.Remove(quakeContent);
            contents.Insert(0, quakeContent);

            // Determine last selected content
            Content lastSelectedContent;
            if (contents.Select((x) => x.Directory).Contains(settings.LastSelected))
            {
                lastSelectedContent = contents.Where((x) => x.Directory == settings.LastSelected).First();
            }
            else 
            {
                lastSelectedContent = contents[0];
            }

            // Print options
            Console.WriteLine("=== QSELECT ===");
            Console.WriteLine();
            Console.WriteLine($"Please select a content directory, or press Enter to resume {lastSelectedContent}.");
            Console.WriteLine();
            for (int i = 0; i < contents.Count; i++)
            {
                Console.WriteLine($"({i})\t{contents[i]}");
            }
            Console.WriteLine();

            // Read selection
            Content selectedContent = null;
            while (selectedContent == null)
            {
                Console.Write("Selection: ");
                string s = Console.ReadLine();
                if (s == "")
                {
                    selectedContent = lastSelectedContent;
                }
                else
                {
                    if (int.TryParse(s, out int result))
                    {
                        if (result >= 0 && result < contents.Count)
                        {
                            selectedContent = contents[result];
                        }
                    }
                }
            }

            // Update last selected content
            settings.LastSelected = selectedContent.Directory;

            // Run game
            selectedContent.LoadContent(settings);
            Directory.SetCurrentDirectory(settings.GameDirectory);
            Process p = Process.Start(settings.Executable, $"-game {selectedContent.Directory}");
            while (p.HasExited == false) ;
            Directory.SetCurrentDirectory("..");
            selectedContent.UnloadContent(settings);
        }
    }
}