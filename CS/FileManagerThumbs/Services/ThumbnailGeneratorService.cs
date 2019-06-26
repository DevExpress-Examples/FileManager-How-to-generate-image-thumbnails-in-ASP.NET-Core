using System;
using System.IO;
using System.Drawing;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Routing;
using System.Security.Cryptography;
using System.Text;
using DevExtreme.AspNet.Mvc.FileManagement;
using Microsoft.AspNetCore.Mvc.Infrastructure;

namespace FileManagerThumbs.Services {
    public interface IThumbnailGeneratorService {
        void AssignThumbnailUrl(FileSystemInfo fileSystemInfo, IClientFileSystemItem clientItem);
    }

    public class ThumbnailGeneratorService : IThumbnailGeneratorService {
        const int
            ThumbnailWidth = 50,
            ThumbnailHeight = 50;

        static readonly string[] AllowedFileExtensions = {
            ".png",
            ".gif",
            ".jpg",
            ".jpeg",
            ".ico",
            ".bmp"
        };

        public ThumbnailGeneratorService(string thumbnailsDirectoryPath, IUrlHelperFactory urlHelperFactory, IActionContextAccessor actionContextAccessor) {
            UrlHelperFactory = urlHelperFactory ?? throw new ArgumentNullException(nameof(urlHelperFactory));
            ActionContextAccessor = actionContextAccessor ?? throw new ArgumentNullException(nameof(actionContextAccessor));
            
            ThumbnailsDirectory = new DirectoryInfo(thumbnailsDirectoryPath);
        }

        IUrlHelperFactory UrlHelperFactory { get; }
        IActionContextAccessor ActionContextAccessor { get; }
        DirectoryInfo ThumbnailsDirectory { get; }

        public void AssignThumbnailUrl(FileSystemInfo fileSystemInfo, IClientFileSystemItem clientItem) {
            if(clientItem.IsDirectory || !CanGenerateThumbnail(fileSystemInfo))
                return;
            
            if(!(fileSystemInfo is FileInfo fileInfo))
                return;
            
            var helper = UrlHelperFactory.GetUrlHelper(ActionContextAccessor.ActionContext);
            var thumbnail = GetThumbnail(fileInfo);
            var relativeThumbnailPath = Path.Combine(ThumbnailsDirectory.Name, thumbnail.Directory?.Name, thumbnail.Name);
            clientItem.CustomFields["thumbnailUrl"] = helper.Content(relativeThumbnailPath);
        }
        
        FileInfo GetThumbnail(FileInfo file) {
            var thumbnailFile = new FileInfo(GetThumbnailFilePath(file));

            if(!HasActualThumbnail(file, thumbnailFile)) {
                using (var thumbnailStream = file.OpenRead()) {
                    if (!GenerateThumbnail(thumbnailStream, thumbnailFile))
                        return null;
                }
            }

            return thumbnailFile;
        }
        
        static bool GenerateThumbnail(Stream file, FileInfo thumbnailFile) {
            try {
                if (!Directory.Exists(thumbnailFile.DirectoryName))
                    Directory.CreateDirectory(thumbnailFile.DirectoryName);

                GenerateThumbnailCore(file, thumbnailFile, ThumbnailWidth, ThumbnailHeight);
                return true;
            } catch {
                return false;
            }
        }
        static void GenerateThumbnailCore(Stream file, FileInfo thumbnailFile, int width, int height) {
            var original = Image.FromStream(file);
            var thumbnail = ChangeImageSize(original, width, height);
            try {
                thumbnail.Save(thumbnailFile.FullName);
            } catch {
                throw new Exception("Thumbnail error");
            } finally {
                thumbnail.Dispose();
                original.Dispose();
            }
        }
        static Bitmap ChangeImageSize(Image original, int width, int height) {
            var thumbnail = new Bitmap(width, height);

            var newHeight = original.Height;
            var newWidth = original.Width;
            if (original.Height > height || original.Width > width) {
                newHeight = (original.Height > original.Width) ? height : (height * original.Height / original.Width);
                newWidth = (original.Width > original.Height) ? width : (width * original.Width / original.Height);
            }

            var g = Graphics.FromImage(thumbnail);
            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;

            var top = (height - newHeight) / 2;
            var left = (width - newWidth) / 2;
            g.DrawImage(original, left, top, newWidth, newHeight);

            return thumbnail;
        }
        
        static bool HasActualThumbnail(FileSystemInfo file, FileSystemInfo thumbnail) {
            if (!thumbnail.Exists)
                return false;

            if(file.LastWriteTime <= thumbnail.LastWriteTime)
                return true;
            
            try {
                thumbnail.Delete();
            } catch {
                throw new Exception("Thumbnail error");
            }
            return false;
        }
        
        static bool CanGenerateThumbnail(FileSystemInfo fileSystemInfo) {
            return AllowedFileExtensions.Any(s => s.Equals(fileSystemInfo.Extension, StringComparison.OrdinalIgnoreCase));
        }
        string GetThumbnailFilePath(FileSystemInfo file) {
            var thumbDir = GetThumbnailFolderName(file);
            return Path.Combine(ThumbnailsDirectory.FullName, thumbDir, GetUriSafeFileName(GetThumbnailFileName(file)));
        }
        static string GetThumbnailFileName(FileSystemInfo file) {
            return "thumb_" + file.Name;
        }
        static string GetUriSafeFileName(string name) {
            return name
                .Replace("&", "[amp];")
                .Replace("#", "[sharp]")
                .Replace("+", "[plus]");
        }
        static string GetThumbnailFolderName(FileSystemInfo file) {
            var token = Path.GetDirectoryName(file.FullName);
            return GetSHA1Hash(Encoding.ASCII.GetBytes(token));
        }
        static string GetSHA1Hash(byte[] data) {
            var algorithm = new SHA1CryptoServiceProvider();
            var hashBytes = algorithm.ComputeHash(data);
            var sb = new StringBuilder();
            for (var i = 0; i < hashBytes.Length; i++) {
                var str = hashBytes[i].ToString("x2");
                sb.Append(str);
            }
            return sb.ToString();
        }
    }
}
