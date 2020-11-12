using LibPackageManager.Repositories;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibQSelect.PackageManager.Thumbnails
{
    public class InstalledThumbnailRepository : IRepository<Thumbnail>
    {
        protected string thumbnailDirPath;

        public List<Thumbnail> Items { get; } = new();

        public RepositoryDataSource DataSource { get; }

        public InstalledThumbnailRepository(string thumbnailDirPath)
        {
            this.thumbnailDirPath = thumbnailDirPath ?? throw new ArgumentNullException(nameof(thumbnailDirPath));
        }

        public async Task RefreshAsync()
        {
            // Check directory exists
            if (!Directory.Exists(thumbnailDirPath)) throw new DirectoryNotFoundException("Specified thumbnail directory was not found.");

            // Clear existing package list
            Items.Clear();

            // Load packages by directory name
            await Task.Run(() =>
            {
                foreach (string id in Directory.GetDirectories(thumbnailDirPath).Select(x => Path.GetFileName(x)))
                {
                    Thumbnail thumbnail = new(id)
                    {
                        InstallPath = $"{thumbnailDirPath}/{id}.jpg"
                    };

                    Items.Add(thumbnail);
                }
            });

        }
    }
}
