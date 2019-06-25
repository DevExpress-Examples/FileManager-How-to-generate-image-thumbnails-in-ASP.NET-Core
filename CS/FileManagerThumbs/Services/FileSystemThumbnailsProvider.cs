using DevExtreme.AspNet.Mvc.FileManagement;
using FileManagerThumbs.Models;
using Microsoft.AspNetCore.Hosting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace FileManagerThumbs.Services {
    public class FileSystemThumbnailsProvider : IFileProvider {
        const string ParentDirectorySymbol = "..";
        static readonly char[] PossibleDirectorySeparators = { '\\', '/' };
        FileManagerThumbnailService ThumbnailService;
        IHostingEnvironment Environment;
        IFileProvider FileProvider { get; set; }
        public string ThumbnailPath { get; set; }
        public string RootDirectoryPath { get; }
        public FileSystemThumbnailsProvider(IHostingEnvironment env, FileManagerThumbnailService thumbnailService) {
            ThumbnailService = thumbnailService;
            Environment = env;
            RootDirectoryPath = Environment.WebRootPath + "\\ContentFolder";
            FileProvider = new DefaultFileProvider(RootDirectoryPath);
        }

        public IList<IClientFileSystemItem> GetDirectoryContents(string dirKey) {
            string dirPath = GetFullDirPathWithCheckOnExistence(dirKey);
            return FileProvider.GetDirectoryContents(dirKey)
                .Select(item => CreateClientFileItem(item, dirPath))
                .ToList();
        }

        IClientFileSystemItem CreateClientFileItem(IClientFileSystemItem item, string dirPath) {
            string thumbnailUrl = string.Empty;
            if (!item.IsDirectory) {
                string filePath = Path.Combine(dirPath, item.Name);
                var fileInfo = new FileInfo(filePath);
                if (ThumbnailService.CanGenerateThumbnail(fileInfo.Extension))
                    thumbnailUrl = ThumbnailService.GetThumbnailUrl(fileInfo);
            }
            var result = new ClientFileSystemItem {
                Name = item.Name,
                DateModified = item.DateModified,
                IsDirectory = item.IsDirectory,
                Size = item.Size,
                HasSubDirectories = item.HasSubDirectories
            };
            result.CustomFields.Add("thumbnailUrl", thumbnailUrl);
            return result;
        }

        public void CreateDirectory(string rootKey, string name) {
            FileProvider.CreateDirectory(rootKey, name);
        }

        public void Rename(string key, string newName) {
            FileProvider.Rename(key, newName);
        }

        public void Move(string sourceKey, string destinationKey) {
            FileProvider.Move(sourceKey, destinationKey);
        }

        public void MoveUploadedFile(FileInfo file, string destinationKey) {
            FileProvider.MoveUploadedFile(file, destinationKey);
        }
        public void RemoveUploadedFile(FileInfo file) {
            FileProvider.RemoveUploadedFile(file);
        }
        public void Copy(string sourceKey, string destinationKey) {
            FileProvider.Copy(sourceKey, destinationKey);
        }

        public void Remove(string key) {
            FileProvider.Remove(key);
        }
        string GetFullDirPathWithCheckOnExistence(string rootKey) {
            var parentDirPath = Path.Combine(RootDirectoryPath, PreparePath(rootKey));
            if (!Directory.Exists(parentDirPath))
                throw new DirectoryNotFoundException(parentDirPath);
            return parentDirPath;
        }
        static string PreparePath(string path) {
            if (string.IsNullOrEmpty(path))
                return string.Empty;

            var pathParts = path
                .Split(PossibleDirectorySeparators, StringSplitOptions.RemoveEmptyEntries)
                .ToList();

            var index = 0;
            while (index < pathParts.Count) {
                if (pathParts[index] == ParentDirectorySymbol && index > 0) {
                    pathParts.RemoveAt(index);
                    pathParts.RemoveAt(index - 1);
                    index--;
                } else
                    index++;
            }

            if (pathParts.Any() && pathParts[0] == ParentDirectorySymbol)
                throw new Exception("No access");

            return Path.Combine(pathParts.ToArray());
        }
    }
}

