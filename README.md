<!-- default badges list -->
![](https://img.shields.io/endpoint?url=https://codecentral.devexpress.com/api/v1/VersionRange/193664916/20.1.3%2B)
[![](https://img.shields.io/badge/Open_in_DevExpress_Support_Center-FF7200?style=flat-square&logo=DevExpress&logoColor=white)](https://supportcenter.devexpress.com/ticket/details/T828667)
[![](https://img.shields.io/badge/📖_How_to_use_DevExpress_Examples-e9f6fc?style=flat-square)](https://docs.devexpress.com/GeneralInformation/403183)
[![](https://img.shields.io/badge/💬_Leave_Feedback-feecdd?style=flat-square)](#does-this-example-address-your-development-requirementsobjectives)
<!-- default badges end -->
# File Manager - How to generate thumbnails for images on server

This examples shows how to generate and show thumbnails for image files. The thumbnail generation is implemented in a custom service that returns the thumbnail URLs in ClientFileSystemItem's custom fields. The thumbnails are rendered on the client side in the customizeThumbnail method, similar to the [FileManager - Custom Thumbnails](https://js.devexpress.com/Demos/WidgetsGallery/Demo/FileManager/CustomThumbnails/jQuery/Light/) demo.

### Follow these steps:
1. Add the FileManager to your page and setup it on the client side.
2. Connect the File Manager to your API Controller via the Remote file system provider:
```cs
@(Html.DevExtreme().FileManager()
    .ID("file-manager")
    .FileSystemProvider(p=>
        p.Remote().Url(Url.Action("FileSystem", "FileManagerApi"))
    )
```
3. Copy the service implementation from the [ThumbnailGeneratorService.cs](CS/FileManagerThumbs/Services/ThumbnailGeneratorService.cs) file. It uses the [System.Drawing.Common](https://www.nuget.org/packages/System.Drawing.Common/) library that supports .NET Core: [System.Drawing.Common - Release Notes](https://github.com/dotnet/core/tree/master/release-notes). 

4. Register the service in Startup.cs:
```cs
         services
                .AddSingleton<IActionContextAccessor, ActionContextAccessor>()
                .AddSingleton<IThumbnailGeneratorService, ThumbnailGeneratorService>();
```
5. To use the service, create a method in your [API Controller](CS/FileManagerThumbs/Controllers/FileManagerApiController.cs) that will handle the File Manager operations and inject the service via Dependency Injection in the following way: 
```cs
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
                ...
            };
            ...
        }
```
6. On the client side, use the [CustomizeThumbnail](https://js.devexpress.com/jQuery/Documentation/ApiReference/UI_Components/dxFileManager/Configuration/#customizeThumbnail) method and get the passed thumbnailUrl from **fileManagerItem.dataItem**:
```cs
...
.CustomizeThumbnail("OnCustomizeThumbnail")
```
```js
  function OnCustomizeThumbnail(fileManagerItem) {
        console.log(fileManagerItem);
        return fileManagerItem.dataItem ? fileManagerItem.dataItem.thumbnailUrl : null;
    }
```
> **NOTE**
> On *Unix-based* systems, you may get the *"System.TypeInitializationException: The type initializer for 'Gdip' threw an exception. ---> System.DllNotFoundException: Unable to load DLL 'libgdiplus': The specified module could not be found"* exception. To solve the problem, install gdi+ using the following command:
```
brew install mono-libgdiplus
```
<!-- feedback -->
## Does this example address your development requirements/objectives?

[<img src="https://www.devexpress.com/support/examples/i/yes-button.svg"/>](https://www.devexpress.com/support/examples/survey.xml?utm_source=github&utm_campaign=FileManager-How-to-generate-image-thumbnails-in-ASP.NET-Core&~~~was_helpful=yes) [<img src="https://www.devexpress.com/support/examples/i/no-button.svg"/>](https://www.devexpress.com/support/examples/survey.xml?utm_source=github&utm_campaign=FileManager-How-to-generate-image-thumbnails-in-ASP.NET-Core&~~~was_helpful=no)

(you will be redirected to DevExpress.com to submit your response)
<!-- feedback end -->
