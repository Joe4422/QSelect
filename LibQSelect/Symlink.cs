using CreateMaps;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace LibQSelect
{
    public class Symlink
    {
        public static bool CreateDirectory(string target, string source)
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                JunctionPoint.Create(target, source, true);
            }
            else
            {
                LostTech.IO.Links.Symlink.CreateForDirectory(source, target);
            }
            return Directory.Exists(target);
        }
    }
}
