using Microsoft.VisualBasic.FileIO;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using LibQuaddicted;

namespace QSelectConsole
{
    class Program
    {

        static void Main(string[] args)
        {
            string url = "https://www.quaddicted.com/reviews/quaddicted_database.xml";

            ModDatabase moddb = new ModDatabase(url);
            DownloadDatabase downdb = new DownloadDatabase(moddb, "db.json", "Mods");
            _ = downdb.DownloadModAsync(moddb.ModFiles["100b2"], "Downloads").Result;

            Console.Write("nice");
        }
    }
}