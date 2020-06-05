/**
* DevExtreme (file_management/remote_provider.d.ts)
* Version: 20.1.3
* Build date: Fri Apr 24 2020
*
* Copyright (c) 2012 - 2020 Developer Express Inc. ALL RIGHTS RESERVED
* Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
*/
import FileSystemProviderBase, {
    FileSystemProviderBaseOptions
} from './provider_base';

/** Warning! This type is used for internal purposes. Do not import it directly. */
export interface RemoteFileSystemProviderOptions extends FileSystemProviderBaseOptions<RemoteFileSystemProvider> {
    /**
     * @docid RemoteFileSystemProviderOptions.endpointUrl
     * @type string
     * @prevFileNamespace DevExpress.fileManagement
     * @public
     */
    endpointUrl?: string;
    /**
     * @docid RemoteFileSystemProviderOptions.hasSubDirectoriesExpr
     * @type string|function(fileSystemItem)
     * @prevFileNamespace DevExpress.fileManagement
     * @public
     */
    hasSubDirectoriesExpr?: string | Function;
}
/** The Remote file system provider works with a file system located on the server. */
export default class RemoteFileSystemProvider extends FileSystemProviderBase {
    constructor(options?: RemoteFileSystemProviderOptions)
}
