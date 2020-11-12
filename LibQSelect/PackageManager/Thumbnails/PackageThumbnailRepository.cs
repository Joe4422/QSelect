using LibPackageManager.Repositories;
using LibQSelect.PackageManager.Packages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibQSelect.PackageManager.Thumbnails
{
    public class PackageThumbnailRepository : IRepository<Thumbnail>
    {
        public List<Thumbnail> Items { get; } = new();

        public RepositoryDataSource DataSource { get; }

        public PackageThumbnailRepository(List<Package> packages)
        {
            foreach (Package package in packages)
            {
                if (package.HasAttribute("ThumbnailURL"))
                {
                    Items.Add(new Thumbnail(package.Id, package.GetAttribute("ThumbnailURL")));
                }
            }
        }

        public Task RefreshAsync()
        {
            return Task.CompletedTask;
        }
    }
}
