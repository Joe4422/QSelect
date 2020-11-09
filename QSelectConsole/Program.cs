using System;
using System.Collections.Generic;
using LibQuakePackageManager.Databases;
using LibQuakePackageManager.Providers;
using LibQSelect;

namespace QSelectConsole
{
    class Program
    {
        static PackageDatabaseManager pdm;
        static SourcePortDatabaseManager spdm;
        static DownloadManager dlm;
        static GameManager gm;

        static void Main(string[] args)
        {
            InitDatabases();

            dlm = new DownloadManager(pdm, spdm);
            gm = new GameManager(pdm);

            if (args.Length < 2)
            {
                Console.WriteLine("Not enough arguments specified.");
                return;
            }

            if (args[0] == "get")
            {
                CmdGetHandler(args);
            }
            else if (args[0] == "run")
            {
                CmdRunHandler(args);
            }
            else
            {
                Console.WriteLine("Invalid argument.");
            }

        }

        public static void InitDatabases()
        {
            List<IProvider<Package>> packageProviders = new List<IProvider<Package>>()
            {
                new LocalPackageProvider("Packages"),
                new BuiltInPackageProvider(),
                new QuaddictedPackageProvider()
            };
            pdm = new PackageDatabaseManager("packages.json", packageProviders);
            pdm.LoadDatabaseAsync().Wait();

            List<IProvider<SourcePort>> sourcePortProviders = new List<IProvider<SourcePort>>()
            {
                new LocalSourcePortProvider("SourcePorts"),
                new BuiltInSourcePortProvider()
            };
            spdm = new SourcePortDatabaseManager("sourceports.json", sourcePortProviders);
            spdm.LoadDatabaseAsync().Wait();
        }

        static void CmdGetHandler(string[] args)
        {
            foreach (string id in args[1..])
            {
                IProviderItem pkg = pdm[id];
                IProviderItem sp = spdm[id];

                IProviderItem item;
                if (pkg != null) item = pkg;
                else if (sp != null) item = sp;
                else
                {
                    Console.WriteLine($"Unknown item {id}.");
                    continue;
                }

                Console.WriteLine($"Downloading item {id}...");
                dlm.DownloadItemAsync(item).Wait();
                Console.WriteLine($"Finished downloading and installing item {id}.");
            }

            Console.WriteLine("Download finished.");
        }

        static void CmdRunHandler(string[] args)
        {
            string sourcePortId = args[1];
            string[] packageIds = args[2..];

            SourcePort sourcePort = spdm[sourcePortId];
            if (sourcePort is null)
            {
                Console.WriteLine($"Unknown source port {sourcePortId}.");
                return;
            }

            Console.WriteLine($"Loading source port {sourcePortId}...");

            gm.LoadSourcePort(sourcePort);

            Console.WriteLine("Loading packages...");

            foreach (string id in packageIds)
            {
                Package pkg = pdm[id];
                if (pkg is null)
                {
                    Console.WriteLine($"Unknown package {id}.");
                    gm.UnloadAllPackages();
                    return;
                }
                gm.LoadPackageAsync(pkg).Wait();
            }

            Console.WriteLine($"Executing {sourcePortId}...");

            gm.ExecuteLoadedSourcePortAsync().Wait();

            Console.WriteLine("Unloading packages...");

            gm.UnloadAllPackages();
        }
    }
}