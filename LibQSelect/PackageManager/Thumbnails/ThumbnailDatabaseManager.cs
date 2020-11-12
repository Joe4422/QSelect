using LibPackageManager.Managers;
using LibPackageManager.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace LibQSelect.PackageManager.Thumbnails
{
    public class ThumbnailDatabaseManager : BaseDatabaseManager<Thumbnail>
    {
        public ThumbnailDatabaseManager(string dbFilePath, List<IRepository<Thumbnail>> repositories) : base(dbFilePath, repositories)
        {

        }

        protected override Thumbnail MergeItems(Thumbnail superior, Thumbnail inferior)
        {
            string downloadUrl = superior.DownloadUrl is null ? inferior.DownloadUrl : superior.DownloadUrl;
            string installPath = superior.InstallPath is null ? inferior.InstallPath : superior.InstallPath;

            return new Thumbnail(superior.Id, downloadUrl)
            {
                InstallPath = installPath
            };
        }
    }
}
