﻿using System.IO;
using DevExtreme.AspNet.Mvc.FileManagement;
using FileManagerThumbs.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;

namespace FileManagerThumbs.Controllers {
    public class FileManagerApiController : Controller {

        public FileManagerApiController(IWebHostEnvironment environment, IThumbnailGeneratorService thumbnailGenerator) {
            Environment = environment;
            ThumbnailGenerator = thumbnailGenerator;
        }

        IWebHostEnvironment Environment { get; }
        IThumbnailGeneratorService ThumbnailGenerator { get; }

        public IActionResult FileSystem(FileSystemCommand command, string arguments) {
            var rootPath = Path.Combine(Environment.WebRootPath, "ContentFolder");
            var config = new FileSystemConfiguration {
                Request = Request,
                FileSystemProvider = new PhysicalFileSystemProvider(rootPath, ThumbnailGenerator.AssignThumbnailUrl),
                AllowCopy = true,
                AllowCreate = true,
                AllowMove = true,
                AllowDelete = true,
                AllowRename = true,
                AllowUpload = true
            };
            config.AllowedFileExtensions = new[] { ".png", ".gif", ".jpg", ".jpeg", ".ico", ".bmp" };
            var processor = new FileSystemCommandProcessor(config);
            var result = processor.Execute(command, arguments);
            return Ok(result.GetClientCommandResult());
        }
    }
}
