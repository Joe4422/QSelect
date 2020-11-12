﻿using LibPackageManager.Managers;
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
        public SourcePortDatabaseManager(string dbFilePath, List<IRepository<SourcePort>> repositories) : base(dbFilePath, repositories)
        {
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
            SourcePort.OperatingSystem supportedOS = superior.SupportedOS == SourcePort.OperatingSystem.Unknown ? inferior.SupportedOS : superior.SupportedOS;

            // Determine install path
            string installPath = superior.InstallPath is null ? inferior.InstallPath : superior.InstallPath;

            SourcePort sourcePort = new(superior.Id, name, author, executable, downloadUrl, supportedOS)
            {
                InstallPath = installPath
            };

            return sourcePort;
        }
    }
}