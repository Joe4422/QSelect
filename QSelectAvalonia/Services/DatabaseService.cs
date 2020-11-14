using LibQSelect.PackageManager.Packages;
using LibQSelect.PackageManager.SourcePorts;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace QSelectAvalonia.Services
{
    public static class DatabaseService
    {
        #region Properties
        public static PackageDatabaseManager Packages { get; private set; } = null;
        public static SourcePortDatabaseManager SourcePorts { get; private set; } = null;
        #endregion

        #region Events
        public delegate void InitialisedEventHandler();
        public static event InitialisedEventHandler Initialised;
        #endregion

        #region Methods
        public static async Task InitialiseAsync(string packagesPath, string sourcePortsPath)
        {
            if (packagesPath is null) throw new ArgumentNullException(nameof(packagesPath));
            if (sourcePortsPath is null) throw new ArgumentNullException(nameof(sourcePortsPath));

            if (Packages != null || SourcePorts != null) throw new Exception("Attempted to re-initialise DatabaseService.");

            Packages = new PackageDatabaseManager($"{packagesPath}/packages.json", new()
            {
                new InstalledPackageRepository(packagesPath),
                new BuiltInPackageRepository(),
                new QuaddictedPackageRepository()
            });
            await Packages.RefreshDatabaseAsync();
            SourcePorts = new SourcePortDatabaseManager($"{sourcePortsPath}/sourceports.json", new()
            {
                new InstalledSourcePortRepository(sourcePortsPath),
                new BuiltInSourcePortRepository()
            });
            await SourcePorts.RefreshDatabaseAsync();

            Initialised?.Invoke();
        }
        #endregion
    }
}
