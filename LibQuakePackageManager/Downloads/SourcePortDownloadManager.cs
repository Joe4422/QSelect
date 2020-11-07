using LibQuakePackageManager.Providers;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Threading.Tasks;

namespace LibQuakePackageManager.Downloads
{
    public class SourcePortDownloadManager : BaseDownloadManager<SourcePort>
    {
        public SourcePortDownloadManager(string downloadDir, string installDir) : base(downloadDir, installDir)
        {
        }

        protected override async Task InstallItemAsync(SourcePort item, string downloadedFilePath)
        {
            string extractDir = $"{installDir}/{item.Id}";

            Directory.CreateDirectory(extractDir);

            await Task.Run(() => ZipFile.ExtractToDirectory(downloadedFilePath, extractDir));
        }
    }
}
