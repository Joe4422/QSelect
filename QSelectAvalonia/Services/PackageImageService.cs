﻿using Avalonia.Media.Imaging;
using LibQSelect.PackageManager.Packages;
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
        private static readonly Dictionary<string, Bitmap> thumbnails = new();
        #endregion

        #region Methods
        public static async Task<Bitmap> GetThumbnailAsync(Package package)
        {
            if (thumbnails.TryGetValue(package.Id, out Bitmap value) == true) return value;
            else
            {
                if (package.HasAttribute("ThumbnailURL"))
                {
                    byte[] data;
                    try
                    {
                        using (WebClient client = new())
                        {
                            data = await client.DownloadDataTaskAsync(package.Attributes["ThumbnailURL"]);
                        }
                    }
                    catch (Exception)
                    {
                        return null;
                    }

                    using MemoryStream ms = new(data);
                    Bitmap bitmap = new(ms);

                    thumbnails.Add(package.Id, bitmap);

                    return bitmap;
                }
                else return null;
            }
        }
        
        public static async Task<Bitmap> GetScreenshotAsync(Package package)
        {
            if (package.HasAttribute("ScreenshotURL"))
            {
                byte[] data;
                try
                {
                    using (WebClient client = new())
                    {
                        data = await client.DownloadDataTaskAsync(package.Attributes["ScreenshotURL"]);
                    }
                } 
                catch (Exception)
                {
                    return null;
                }

                using MemoryStream ms = new(data);
                Bitmap bitmap = new(ms);

                return bitmap;
            }
            else
            {
                return null;
            }
        }
        #endregion
    }
}