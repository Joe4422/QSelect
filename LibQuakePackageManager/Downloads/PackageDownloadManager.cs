using LibQuakePackageManager.Databases;
using LibQuakePackageManager.Providers;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Threading.Tasks;

namespace LibQuakePackageManager.Downloads
{
    public class PackageDownloadManager : BaseDownloadManager<Package>
    {
        public PackageDownloadManager(string downloadDir, string installDir) : base(downloadDir, installDir)
        {
        }

        protected override async Task InstallItemAsync(Package item, string downloadedFilePath)
        {
            string extractDir = null;
            if (item.UnzipDirectory != null)
            {
                if (item.UnzipDirectory.Contains("maps"))
                {
                    extractDir = $"{installDir}/{item.Id}/maps/";
                }
                else
                {
                    extractDir = $"{installDir}/{item.Id}/";
                }
            }

            Directory.CreateDirectory(extractDir);

            await Task.Run(() => ZipFile.ExtractToDirectory(downloadedFilePath, extractDir));
        }
    }
}
