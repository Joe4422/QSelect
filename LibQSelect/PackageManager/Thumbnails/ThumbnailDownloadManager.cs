using LibPackageManager.Managers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibQSelect.PackageManager.Thumbnails
{
    public class ThumbnailDownloadManager : BaseDownloadManager<Thumbnail>
    {
        public ThumbnailDownloadManager(string downloadDir, string installDir) : base(downloadDir, installDir)
        {
        }

        protected override async Task InstallItemAsync(Thumbnail item, string downloadedFilePath)
        {
            await Task.Run(() =>
            {
                File.Copy(downloadedFilePath, $"{installDir}/{Path.GetFileName(downloadedFilePath)}");
            });
        }
    }
}
