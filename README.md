# File Manager - How to generate thumbnails for images on server

This examples shows how to generate and show thumbnails for image files. This scenario is implemented with a custom provider based on the **[IFileProvider](https://docs.devexpress.com/AspNetCore/DevExtreme.AspNet.Mvc.FileManagement.IFileProvider)** interface. The custom provider returns the thumbnail URLs in ClientFileSystemItem's custom fields. The thumbnails are rendered on the client side in the customThumbnail method, similar to the [FileManager - Custom Thumbnails](https://js.devexpress.com/Demos/WidgetsGallery/Demo/FileManager/CustomThumbnails/jQuery/Light/) demo.

### Follow these steps:
1. Add the FileManager to your page and setup it on the client side.
2. Set the [fileProvider.endpointUrl](https://js.devexpress.com/DevExtreme/ApiReference/UI_Widgets/dxFileManager/Configuration/#fileProvider) option so that it points to your API controller.
3. Implement the [IFileProvider](https://docs.devexpress.com/AspNetCore/DevExtreme.AspNet.Mvc.FileManagement.IFileProvider) interface. You can use the DefaultFileProvider operations in most methods. The only method that should be implemented explicitly is **GetDirectoryContents** !!!**TODO - ADD LINK TO DOC**.

4. The **GetDirectoryContents** method should return a list of **IClientFileSystemItem** !!!**TODO - ADD LINK TO DOC** objects with the generated thumbnails:   
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
5. The thumbnail generation is implemented in the [FileManagerThumbnailHelper]() !!!**TODO - ADD LINK TO FILE** class. It uses the [System.Drawing.Common](https://www.nuget.org/packages/System.Drawing.Common/) library that supports with .NET Core: [System.Drawing.Common - Release Notes](https://github.com/dotnet/core/tree/master/release-notes). 
6. To use the custom provider, create a method in your [API Controller]() !!!**TODO - ADD LINK TO FILE** that will handle the File Manager operations and use your custom provider there:
```cs
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
```
7. On the client side, use the customThumbnail method and get the passed thumbnailUrl from **fileManagerItem.dataItem**:
```js
 customizeThumbnail: function (fileManagerItem) {
                if (fileManagerItem.dataItem)
                    return fileManagerItem.dataItem.thumbnailUrl;
            }
```

