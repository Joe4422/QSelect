using LibQSelect.PackageManager.Packages;
using LibQSelect.PackageManager.SourcePorts;
using LibQSelect.PackageManager.Thumbnails;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace QSelectAvalonia.Services
{
    public static class DatabaseService
    {
        #region Properties
        public static PackageDatabaseManager Packages { get; private set; } = null;
        public static SourcePortDatabaseManager SourcePorts { get; private set; } = null;
        public static ThumbnailDatabaseManager Thumbnails { get; private set; } = null;
        #endregion

        #region Methods
        public static async Task InitialiseAsync(string packagesPath, string sourcePortsPath, string thumbnailsPath)
        {
            if (packagesPath is null) throw new ArgumentNullException(nameof(packagesPath));
            if (sourcePortsPath is null) throw new ArgumentNullException(nameof(sourcePortsPath));
            if (thumbnailsPath is null) throw new ArgumentNullException(nameof(thumbnailsPath));

            if (Packages != null || SourcePorts != null || Thumbnails != null) throw new Exception("Attempted to re-initialise DatabaseService.");

            Packages = new PackageDatabaseManager($"{packagesPath}/packages.json", new()
            {
                new InstalledPackageRepository(packagesPath),
                new BuiltInPackageRepository(),
                new QuaddictedPackageRepository()
            });
            await Packages.LoadDatabaseAsync();
            SourcePorts = new SourcePortDatabaseManager($"{sourcePortsPath}/sourceports.json", new()
            {
                new InstalledSourcePortRepository(sourcePortsPath),
                new BuiltInSourcePortRepository()
            });
            await SourcePorts.LoadDatabaseAsync();
            Thumbnails = new ThumbnailDatabaseManager($"{thumbnailsPath}/thumbnails.json", new()
            {
                new InstalledThumbnailRepository(thumbnailsPath),
                new PackageThumbnailRepository(Packages.Items)
            });
        }
        #endregion
    }
}
