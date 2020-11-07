using System;
using System.Collections.Generic;
using LibQuakePackageManager.Databases;
using LibQuakePackageManager.Providers;
using LibQuakePackageManager.Downloads;

namespace QSelectConsole
{
    class Program
    {
        
        static void Main(string[] args)
        {
            List<IProvider<Package>> packageProviders = new List<IProvider<Package>>()
            {
                new LocalPackageProvider("Packages"),
                new BuiltInPackageProvider(),
                new QuaddictedPackageProvider()
            };
            PackageDatabaseManager pdm = new PackageDatabaseManager("packages.json", packageProviders);
            pdm.LoadDatabaseAsync().Wait();

            List<IProvider<SourcePort>> sourcePortProviders = new List<IProvider<SourcePort>>()
            {
                new LocalSourcePortProvider("SourcePorts"),
                new BuiltInSourcePortProvider()
            };
            SourcePortDatabaseManager spdm = new SourcePortDatabaseManager("sourceports.json", sourcePortProviders);
            spdm.LoadDatabaseAsync().Wait();

            PackageDownloadManager pdlm = new PackageDownloadManager("Downloads", "Packages");
            SourcePortDownloadManager spdlm = new SourcePortDownloadManager("Downloads", "SourcePorts");

            if (args.Length < 2)
            {
                Console.WriteLine("Not enough arguments specified.");
                return;
            }

            if (args[0] == "package" || args[0] == "source-port")
            {
                string[] items = args[2..];

                foreach (string item in items)
                {
                    IProviderItem providerItem = null;
                    if (args[0] == "package") providerItem = pdm[item];
                    else if (args[0] == "source-port") providerItem = spdm[item];

                    if (providerItem == null || providerItem.DownloadUrl == null)
                    {
                        Console.WriteLine($"Unknown item {item}.");
                        continue;
                    }

                    Console.WriteLine();

                    if (args[1] == "get")
                    {
                        if (providerItem.IsDownloaded)
                        {
                            Console.WriteLine($"Item {item} is already downloaded.");
                            continue;
                        }

                        Console.WriteLine($"Downloading item {item}...");

                        if (args[0] == "package") pdlm.DownloadItemAsync((Package)providerItem).Wait();
                        else if (args[0] == "source-port") spdlm.DownloadItemAsync((SourcePort)providerItem).Wait();

                        Console.WriteLine($"Finished downloading {item}.");
                    }
                    else if (args[1] == "remove")
                    {
                        if (!providerItem.IsDownloaded)
                        {
                            Console.WriteLine($"Item {item} is not downloaded.");
                            continue;
                        }

                        Console.WriteLine($"Removing item {item}...");

                        if (args[0] == "package") pdlm.RemoveItemAsync((Package)providerItem).Wait();
                        else if (args[0] == "source-port") spdlm.RemoveItemAsync((SourcePort)providerItem).Wait();

                        Console.WriteLine($"Finished removing {item}.");
                    }
                    else
                    {
                        Console.WriteLine("Invalid argument.");
                        return;
                    }
                }
                
                if (args[0] == "package")
                {
                    pdm.SaveDatabaseAsync().Wait();
                }
                else
                {
                    spdm.SaveDatabaseAsync().Wait();
                }
            }
            else
            {
                Console.WriteLine("Invalid argument.");
                return;
            }

        }
    }
}