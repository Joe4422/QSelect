using LibPackageManager.Managers;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Threading.Tasks;
using LibPackageManager.Repositories;

namespace LibQSelect.PackageManager.SourcePorts
{
    public class SourcePortDownloadManager : BaseDownloadManager<SourcePort>
    {
        public SourcePortDownloadManager(string downloadDir, string installDir) : base(downloadDir, installDir)
        {
        }

        protected override async Task<bool> InstallItemAsync(SourcePort sourcePort)
        {
            string extractDir = $"{installDir}/{sourcePort.Id}/";

            // Try to install source port
            try
            {
                Directory.CreateDirectory(extractDir);
                await Task.Run(() => ZipFile.ExtractToDirectory($"{downloadDir}/{Path.GetFileName(sourcePort.DownloadUrl)}", extractDir));
                return true;
            }
            catch (Exception)
            {
                sourcePort.Token.State = ProgressToken.ProgressState.Failed;
                Directory.Delete(extractDir, true);
                return false;
            }
        }
    }
}
