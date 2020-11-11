using Avalonia.Media.Imaging;
using LibQSelect.PackageManager;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace QSelectAvalonia.Services
{
    public static class PackageImageService
    {
        #region Variables
        private static readonly Dictionary<string, byte[]> bitmapCache = new();

        private static readonly Dictionary<string, Bitmap> thumbnailCache = new();
        #endregion

        #region Methods
        public static async Task<Bitmap> GetThumbnailAsync(Package package, int size)
        {
            if (thumbnailCache.TryGetValue(package.Id, out Bitmap value)) return value;
            else
            {
                if (package.HasAttribute("Screenshot"))
                {
                    byte[] data;
                    try
                    {
                        using (WebClient client = new())
                        {
                            data = await client.DownloadDataTaskAsync(package.Attributes["Screenshot"]);
                        }
                    }
                    catch (Exception)
                    {
                        return null;
                    }

                    using MemoryStream ms = new(data);
                    using Bitmap bitmap = new(ms);
                    ms.Seek(0, SeekOrigin.Begin);

                    Bitmap thumb;

                    if (bitmap.Size.Width > bitmap.Size.Height)
                    {
                        thumb = Bitmap.DecodeToWidth(ms, size);
                    }
                    else
                    {
                        thumb = Bitmap.DecodeToHeight(ms, size);
                    }

                    thumbnailCache[package.Id] = thumb;

                    return thumb;
                }
                else return null;
            }
        }
        
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
                        using (WebClient client = new())
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
