using LibPackageManager.Managers;
using LibPackageManager.Repositories;
using System;
using System.Collections.Generic;
using System.Text;

namespace LibQSelect.PackageManager
{
    public class PackageDatabaseManager : BaseDatabaseManager<Package>
    {
        public PackageDatabaseManager(string dbFilePath, List<IRepository<Package>> repositories) : base(dbFilePath, repositories)
        {
        }

        protected override Package MergeItems(Package superior, Package inferior)
        {
            // Check if arguments are null
            if (superior is null) throw new ArgumentNullException(nameof(superior));
            if (inferior is null) throw new ArgumentNullException(nameof(inferior));

            // Check package IDs match
            if (superior.Id != inferior.Id) throw new ArgumentException("Package IDs did not match!");

            // Determine MD5 checksum
            string md5Checksum = superior.MD5Checksum is null ? inferior.MD5Checksum : superior.MD5Checksum;

            // Determine unzip directory
            string unzipDirectory = superior.UnzipDirectory is null ? inferior.UnzipDirectory : superior.UnzipDirectory;

            // Determine download URL
            string downloadUrl = superior.DownloadUrl is null ? inferior.DownloadUrl : superior.DownloadUrl;

            // Merge attributes
            Dictionary<string, string> attributes = null;
            if (inferior.Attributes is null && !(superior.Attributes is null))
            {
                attributes = new Dictionary<string, string>(superior.Attributes);
            }
            else if (!(inferior.Attributes is null) && superior.Attributes is null)
            {
                attributes = new Dictionary<string, string>(inferior.Attributes);
            }
            else if (!(inferior.Attributes is null) && !(superior.Attributes is null))
            {
                attributes = new Dictionary<string, string>(inferior.Attributes);
                foreach (KeyValuePair<string, string> kvp in superior.Attributes)
                {
                    attributes[kvp.Key] = kvp.Value;
                }
            }

            // Merge dependencies
            Dictionary<string, IDependentRepositoryItem> dependencies = null;
            if (inferior.Dependencies is null && !(superior.Dependencies is null))
            {
                dependencies = new Dictionary<string, IDependentRepositoryItem>(superior.Dependencies);
            }
            else if (!(inferior.Dependencies is null) && superior.Dependencies is null)
            {
                dependencies = new Dictionary<string, IDependentRepositoryItem>(inferior.Dependencies);
            }
            else if (!(inferior.Dependencies is null) && !(superior.Dependencies is null))
            {
                dependencies = new Dictionary<string, IDependentRepositoryItem>(inferior.Dependencies);
                foreach (string s in superior.Dependencies.Keys)
                {
                    dependencies[s] = superior.Dependencies[s];
                }
            }

            // Determine install directory
            string installPath = superior.InstallPath is null ? inferior.InstallPath : superior.InstallPath;

            Package package = new Package(superior.Id, attributes, md5Checksum, unzipDirectory, downloadUrl, dependencies)
            {
                InstallPath = installPath
            };

            return package;
        }
    }
}
