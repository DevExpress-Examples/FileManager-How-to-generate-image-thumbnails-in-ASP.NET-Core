/**
* DevExtreme (file_management/object_provider.d.ts)
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
export interface ObjectFileSystemProviderOptions extends FileSystemProviderBaseOptions<ObjectFileSystemProvider> {
    /**
     * @docid ObjectFileSystemProviderOptions.contentExpr
     * @type string|function(fileSystemItem)
     * @prevFileNamespace DevExpress.fileManagement
     * @public
     */
    contentExpr?: string | Function;
    /**
     * @docid ObjectFileSystemProviderOptions.data
     * @type Array<any>
     * @prevFileNamespace DevExpress.fileManagement
     * @public
     */
    data?: Array<any>;
    /**
     * @docid ObjectFileSystemProviderOptions.itemsExpr
     * @type string|function(fileSystemItem)
     * @prevFileNamespace DevExpress.fileManagement
     * @public
     */
    itemsExpr?: string | Function;
}
/** The Object file system provider works with a virtual file system represented by an in-memory array of JSON objects. */
export default class ObjectFileSystemProvider extends FileSystemProviderBase {
    constructor(options?: ObjectFileSystemProviderOptions)
}
