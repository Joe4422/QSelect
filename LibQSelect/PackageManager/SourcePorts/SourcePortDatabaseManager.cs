using LibPackageManager.Managers;
using LibPackageManager.Repositories;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace LibQSelect.PackageManager.SourcePorts
{
    public class SourcePortDatabaseManager : BaseDatabaseManager<SourcePort>
    {
        public SourcePortDatabaseManager(List<IRepository<SourcePort>> repositories) : base(repositories)
        {
        }

        protected override SourcePort CreateUnknownDependency(string id)
        {
            return new(id, null);
        }

        protected override SourcePort MergeItems(SourcePort superior, SourcePort inferior)
        {
            // Check if arguments are null
            if (superior is null) throw new ArgumentNullException(nameof(superior));
            if (inferior is null) throw new ArgumentNullException(nameof(inferior));

            // Check package IDs match
            if (superior.Id != inferior.Id) throw new ArgumentException("Package IDs did not match!");

            // Determine name
            string name = superior.Name is null ? inferior.Name : superior.Name;

            // Determine author
            string author = superior.Author is null ? inferior.Author : superior.Author;

            // Determine executable
            string executable = superior.Executable is null ? inferior.Executable : superior.Executable;

            // Determine download URL
            string downloadUrl = superior.DownloadUrl is null ? inferior.DownloadUrl : superior.DownloadUrl;

            // Determine operating system
            SourcePort.OperatingSystem os = superior.SupportedOS == SourcePort.OperatingSystem.Unknown ? inferior.SupportedOS : superior.SupportedOS;

            SourcePort sourcePort = new
            (
                id: superior.Id,
                downloadUrl: downloadUrl,
                name: name,
                author: author,
                executable: executable,
                os: os
            );

            if (superior.Token.State == ProgressToken.ProgressState.Installed || inferior.Token.State == ProgressToken.ProgressState.Installed) sourcePort.Token.State = ProgressToken.ProgressState.Installed;

            return sourcePort;
        }
    }
}
