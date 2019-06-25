using Microsoft.AspNetCore.Hosting;
using System;
using System.IO;
using System.Drawing;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Routing;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Mvc.Infrastructure;

namespace FileManagerThumbs.Services {
    public class FileManagerThumbnailService{
        const int ThumbnailWidth = 50;
        const int ThumbnailHeight = 50;

        string ThumbnailsFolderPath { get; set; }
        IHostingEnvironment Environment;
        readonly IActionContextAccessor ContextAccessor;
        IUrlHelperFactory UrlHelperFactory; 
        readonly string[] CanGenerateThumbnailList = new string[] {
            ".png",
            ".gif",
            ".jpg",
            ".jpeg",
            ".ico",
            ".bmp"
        };

        public FileManagerThumbnailService(IHostingEnvironment env, IUrlHelperFactory urlHelperFactory, IActionContextAccessor actionContextAccessor) {
            Environment = env;
            ThumbnailsFolderPath = Environment.WebRootPath + "\\Thumbs";
            ContextAccessor = actionContextAccessor;
            UrlHelperFactory = urlHelperFactory;

        }
        public bool CanGenerateThumbnail(string extension) {
            return !string.IsNullOrEmpty(ThumbnailsFolderPath) && CanGenerateThumbnailList.Any(s => s.Equals(extension, StringComparison.OrdinalIgnoreCase));
        }
        public string GetThumbnailUrl(FileInfo file) {
            var helper = UrlHelperFactory.GetUrlHelper(ContextAccessor.ActionContext);
            return helper.Content(GetGeneratedThumbnailUrl(file)) ?? string.Empty;
        }
        string GetGeneratedThumbnailUrl(FileInfo file) {
            if (CanGenerateThumbnail(file.Extension)) {
                FileInfo thumbnailFile = new FileInfo(GetThumbnailFilePath(file));
                if (!HasActualThumbnail(file, thumbnailFile)) {
                    using (var thumbnailStream = file.OpenRead()) {
                        if (!GenerateThumbnail(thumbnailStream, thumbnailFile))
                            return null;
                    }
                }
                return Path.Combine("\\", new DirectoryInfo(ThumbnailsFolderPath).Name, GetThumbnailFolderName(file), thumbnailFile.Name);
            }
            return null;
        }
        bool HasActualThumbnail(FileInfo file, FileInfo thumbnailFile) {
            if (!thumbnailFile.Exists)
                return false;

            if (file.LastWriteTime > thumbnailFile.LastWriteTime) {
                try {
                    thumbnailFile.Delete();
                } catch {
                    throw new Exception("Thumbnail error");
                }
                return false;
            }

            return true;
        }
        bool GenerateThumbnail(Stream file, FileInfo thumbnailFile) {
            try {
                if (!Directory.Exists(thumbnailFile.DirectoryName)) {
                    Directory.CreateDirectory(thumbnailFile.DirectoryName);
                }
            } catch {
                throw new Exception("Thumbnail error");
            }
            try {
                GenerateThumbnail(file, thumbnailFile, ThumbnailWidth, ThumbnailHeight);
                return true;
            } catch {
                return false;
            }
        }

        string GetThumbnailFilePath(FileInfo file) {
            string thumbDir = GetThumbnailFolderName(file);
            return Path.Combine(ThumbnailsFolderPath, thumbDir, GetUriSafeFileName(GetThumbnailFileName(file)));
        }
        string GetThumbnailFileName(FileInfo file) {
            return "thumb_" + file.Name;
        }
        static string GetUriSafeFileName(string name) {
            return name.Replace("&", "[amp];").Replace("#", "[sharp]").Replace("+", "[plus]");
        }

        string GetThumbnailFolderName(FileInfo file) {
            string token = file.Directory.FullName;
            return GetSHA1Hash(Encoding.ASCII.GetBytes(token));
        }
        static string GetSHA1Hash(byte[] data) {
            HashAlgorithm algorithm = new SHA1CryptoServiceProvider();
            byte[] hashBytes = algorithm.ComputeHash(data);
            var sb = new StringBuilder();
            for (int i = 0; i < hashBytes.Length; i++) {
                string str = hashBytes[i].ToString("x2");
                sb.Append(str);
            }
            return sb.ToString();
        }

        void GenerateThumbnail(Stream file, FileInfo thumbnailFile, int width, int height) {
            System.Drawing.Image original = System.Drawing.Image.FromStream(file);
            Bitmap thumbnail = ChangeImageSize(original, width, height);
            try {
                thumbnail.Save(thumbnailFile.FullName);
            } catch {
                throw new Exception("Thumbnail error");
            } finally {
                thumbnail.Dispose();
                original.Dispose();
            }
        }
        Bitmap ChangeImageSize(System.Drawing.Image original, int width, int height) {
            Bitmap thumbnail = new Bitmap(width, height);

            int newHeight = original.Height;
            int newWidth = original.Width;
            if (original.Height > height || original.Width > width) {
                newHeight = (original.Height > original.Width) ? height : (int)(height * original.Height / original.Width);
                newWidth = (original.Width > original.Height) ? width : (int)(width * original.Width / original.Height);
            }

            Graphics g = Graphics.FromImage(thumbnail);
            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;

            int top = (int)(height - newHeight) / 2;
            int left = (int)(width - newWidth) / 2;
            g.DrawImage(original, left, top, newWidth, newHeight);

            return thumbnail;
        }
    }
}
