using DevExtreme.AspNet.Mvc.FileManagement;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;

namespace FileManagerThumbs.Controllers {
    public class FileManagerApiController : ControllerBase {
        readonly IHostingEnvironment _hostingEnvironment;
        IFileProvider _provider;
        public FileManagerApiController(IHostingEnvironment hostingEnvironment, IFileProvider provider) {
            _hostingEnvironment = hostingEnvironment;
            _provider = provider;
        }
        public IActionResult FileSystem(FileSystemCommand command, string arguments) {
            var config = new FileSystemConfiguration {
                Request = Request,
                FileSystemProvider = _provider,
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
