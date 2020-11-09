using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace LibQSelect
{
    public class Settings
    {
        public string DownloadsPath { get; set; } = "Downloads";
        public string PackagesPath { get; set; } = "Packages";
        public string SourcePortsPath { get; set; } = "SourcePorts";
    }
}
