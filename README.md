# File Manager - How to generate thumbnails for images on server

This examples shows how to generate and show thumbnails for image files. This scenario is implemented with a custom provider based on the **[IFileProvider](https://docs.devexpress.com/AspNetCore/DevExtreme.AspNet.Mvc.FileManagement.IFileProvider)** interface. The custom provider returns the thumbnail URLs in ClientFileSystemItem's custom fields. The thumbnails are rendered on the client side in the customizeThumbnail method, similar to the [FileManager - Custom Thumbnails](https://js.devexpress.com/Demos/WidgetsGallery/Demo/FileManager/CustomThumbnails/jQuery/Light/) demo.

### Follow these steps:
1. Add the FileManager to your page and setup it on the client side.
2. Set the [fileProvider.endpointUrl](https://js.devexpress.com/DevExtreme/ApiReference/UI_Widgets/dxFileManager/Configuration/#fileProvider) option so that it points to your API controller.
3. Implement the [IFileProvider](https://docs.devexpress.com/AspNetCore/DevExtreme.AspNet.Mvc.FileManagement.IFileProvider) interface. You can use the DefaultFileProvider operations in most methods. The only method that should be implemented explicitly is [GetDirectoryContents](https://docs.devexpress.com/AspNetCore/DevExtreme.AspNet.Mvc.FileManagement.IFileProvider.GetDirectoryContents(System.String)).

4. The **GetDirectoryContents** method should return a list of [IClientFileSystemItem](https://docs.devexpress.com/AspNetCore/DevExtreme.AspNet.Mvc.FileManagement.IClientFileSystemItem) objects with the generated thumbnails:   
```cs
 IClientFileSystemItem CreateClientFileItem(IClientFileSystemItem item, string dirPath) {
            string thumbnailUrl = string.Empty;
            if(!item.IsDirectory) {
                string filePath = Path.Combine(dirPath, item.Name);
                var fileInfo = new FileInfo(filePath);
                if(ThumbnailHelper.CanGenerateThumbnail(fileInfo.Extension))
                    thumbnailUrl = ThumbnailHelper.GetThumbnailUrl(fileInfo);
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
```
5. The thumbnail generation is implemented in the [FileManagerThumbnailService](.\CS\FileManagerThumbs\Services\FileManagerThumbnailService.cs) class. It uses the [System.Drawing.Common](https://www.nuget.org/packages/System.Drawing.Common/) library that supports .NET Core: [System.Drawing.Common - Release Notes](https://github.com/dotnet/core/tree/master/release-notes). 
6. To use the custom provider, create a method in your [API Controller](.\CS\FileManagerThumbs\Controllers\FileManagerApiController.cs) that will handle the File Manager operations and use your custom provider there. Here, the custom provider is added via Dependency Injection: 
```cs
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
```
7. On the client side, use the [customizeThumbnail](https://js.devexpress.com/DevExtreme/ApiReference/UI_Widgets/dxFileManager/Configuration/#customizeThumbnail) method and get the passed thumbnailUrl from **fileManagerItem.dataItem**:
```js
 customizeThumbnail: function (fileManagerItem) {
                if (fileManagerItem.dataItem)
                    return fileManagerItem.dataItem.thumbnailUrl;
            }
```

