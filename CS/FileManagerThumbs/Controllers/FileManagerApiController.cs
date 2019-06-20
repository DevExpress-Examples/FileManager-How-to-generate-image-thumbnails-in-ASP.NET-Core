using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DevExtreme.AspNet.Mvc.FileManagement;
using FileManagerThumbs.Helpers;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;

namespace FileManagerThumbs.Controllers {
    public class FileManagerApiController : ControllerBase {
        IHostingEnvironment _hostingEnvironment;
        public FileManagerApiController(IHostingEnvironment hostingEnvironment) {
            _hostingEnvironment = hostingEnvironment;
        }
        public IActionResult FileSystem(FileSystemCommand command, string arguments) {
            var config = new FileSystemConfiguration {
                Request = Request,
                FileSystemProvider = new FileSystemThumbnailsProvider(_hostingEnvironment.WebRootPath + "\\ContentFolder", 
                _hostingEnvironment.WebRootPath + "\\Thumbs", ControllerContext),
                AllowCopy = true,
                AllowCreate = true,
                AllowMove = true,
                AllowRemove = true,
                AllowRename = true,
                AllowUpload = true
            };
            var processor = new FileSystemCommandProcessor(config);
            var result = processor.Execute(command, arguments);
            return Ok(result.GetClientCommandResult());
        }
    }
}
