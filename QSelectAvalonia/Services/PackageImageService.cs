using LibQuakePackageManager.Providers;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace QSelectAvalonia.Services
{
    public static class PackageImageService
    {
        #region Variables
        private static Dictionary<string, byte[]> bitmapCache = new Dictionary<string, byte[]>();
        #endregion

        #region Methods
        public static async Task<byte[]> GetBitmapAsync(Package package)
        {
            if (bitmapCache.ContainsKey(package.Id))
            {
                return bitmapCache[package.Id];
            }
            else
            {
                if (package.HasAttribute("Screenshot"))
                {
                    byte[] data;
                    try
                    {
                        using (WebClient client = new WebClient())
                        {
                            data = await client.DownloadDataTaskAsync(package.Attributes["Screenshot"]);
                        }
                    } 
                    catch (Exception)
                    {
                        return null;
                    }
                    bitmapCache[package.Id] = data;
                    return data;
                }
                else
                {
                    return null;
                }
            }
        }
        #endregion
    }
}
