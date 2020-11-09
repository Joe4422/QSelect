using LibQuakePackageManager.Databases;
using System;
using System.Collections.Generic;
using System.Text;

namespace QSelectAvalonia.Services
{
    public static class DatabaseService
    {
        #region Properties
        public static PackageDatabaseManager PackageDatabase { get; private set; } = null;
        public static SourcePortDatabaseManager SourcePortDatabase { get; private set; } = null;
        #endregion

        #region Methods
        public static void Initialise()
        {
            //PackageDatabase = new PackageDatabaseManager()
        }
        #endregion
    }
}
