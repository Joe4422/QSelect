using LibPackageManager.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibQSelect.PackageManager.Thumbnails
{
    public class Thumbnail : IRepositoryItem
    {
        public string Id { get; }

        public string InstallPath { get; set; }

        public string DownloadUrl { get; }

        public Thumbnail(string id, string downloadUrl = null)
        {
            Id = id ?? throw new ArgumentNullException(nameof(id));
            DownloadUrl = downloadUrl;
        }
    }
}
