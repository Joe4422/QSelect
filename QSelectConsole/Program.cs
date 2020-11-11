using System;
using System.Collections.Generic;
using LibQSelect.PackageManager;
using LibQSelect;
using LibPackageManager.Repositories;

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

            Settings settings = new Settings();

            dlm = new DownloadManager(settings, pdm, spdm);
            gm = new GameManager(settings, pdm);

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
            List<IRepository<Package>> packageProviders = new List<IRepository<Package>>()
            {
                new InstalledPackageRepository("Packages"),
                new BuiltInPackageRepository(),
                new QuaddictedPackageRepository()
            };
            pdm = new PackageDatabaseManager("packages.json", packageProviders);
            pdm.LoadDatabaseAsync().Wait();

            List<IRepository<SourcePort>> sourcePortProviders = new List<IRepository<SourcePort>>()
            {
                new InstalledSourcePortRepository("SourcePorts"),
                new BuiltInSourcePortRepository()
            };
            spdm = new SourcePortDatabaseManager("sourceports.json", sourcePortProviders);
            spdm.LoadDatabaseAsync().Wait();
        }

        static void CmdGetHandler(string[] args)
        {
            foreach (string id in args[1..])
            {
                IRepositoryItem pkg = pdm[id];
                IRepositoryItem sp = spdm[id];

                IRepositoryItem item;
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