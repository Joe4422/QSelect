using Microsoft.VisualBasic.FileIO;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using Newtonsoft.Json.Linq;
using LibQSelect;

namespace QSelectConsole
{
    class Program
    {
        private static readonly string configFile = "config.json";

        static void Main(string[] _)
        {

            // Load data
            QSelect select = new QSelect(configFile);

            // Print options
            Console.WriteLine("=== QSELECT ===");
            Console.WriteLine();
            Console.WriteLine($"Please select a mod.");
            Console.WriteLine();
            for (int i = 0; i < select.Mods.Count; i++)
            {
                Console.WriteLine($"({i})\t{select.Mods[i]}");
            }
            Console.WriteLine();

            // Read selection
            Mod chosenMod = null;
            while (chosenMod == null)
            {
                Console.Write("Selection: ");
                string s = Console.ReadLine();
                if (int.TryParse(s, out int result))
                {
                    if (result >= 0 && result < select.Mods.Count)
                    {
                        chosenMod = select.Mods[result];
                    }
                }
            }

            Binary chosenBinary = select.Binaries.Where((x) => x.Directory == "quakespasm-spiked").First();

            // Run game
            select.RunBinary(chosenBinary, new List<Mod> { chosenMod });
        }
    }
}