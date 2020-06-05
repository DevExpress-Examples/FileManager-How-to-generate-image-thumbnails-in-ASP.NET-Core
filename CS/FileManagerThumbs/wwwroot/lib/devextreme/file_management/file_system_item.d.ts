/**
* DevExtreme (file_management/file_system_item.d.ts)
* Version: 20.1.3
* Build date: Fri Apr 24 2020
*
* Copyright (c) 2012 - 2020 Developer Express Inc. ALL RIGHTS RESERVED
* Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
*/
/** An object that provides information about a file system item (file or folder) in the FileManager widget. */
export default class FileSystemItem {
    constructor(path: string, isDirectory: boolean, pathKeys?: Array<string>);

    /**
     * @docid FileSystemItemFields.path
     * @type string
     * @prevFileNamespace DevExpress.fileManagement
     * @public
     */
    path: string;

    /**
     * @docid FileSystemItemFields.pathKeys
     * @type Array<string>
     * @prevFileNamespace DevExpress.fileManagement
     * @public
     */
    pathKeys: Array<string>;

    /**
     * @docid FileSystemItemFields.key
     * @type string
     * @prevFileNamespace DevExpress.fileManagement
     * @public
     */
    key: string;

    /**
     * @docid FileSystemItemFields.name
     * @type string
     * @prevFileNamespace DevExpress.fileManagement
     * @public
     */
    name: string;

    /**
     * @docid FileSystemItemFields.dateModified
     * @type Date
     * @prevFileNamespace DevExpress.fileManagement
     * @public
     */
    dateModified: Date;

    /**
     * @docid FileSystemItemFields.size
     * @type number
     * @prevFileNamespace DevExpress.fileManagement
     * @public
     */
    size: number;

    /**
     * @docid FileSystemItemFields.isDirectory
     * @type boolean
     * @prevFileNamespace DevExpress.fileManagement
     * @public
     */
    isDirectory: boolean;

    /**
     * @docid FileSystemItemFields.hasSubDirectories
     * @type boolean
     * @prevFileNamespace DevExpress.fileManagement
     * @public
     */
    hasSubDirectories: boolean;

    /**
     * @docid FileSystemItemFields.thumbnail
     * @type string
     * @prevFileNamespace DevExpress.fileManagement
     * @public
     */
    thumbnail: string;

    /**
     * @docid FileSystemItemFields.dataItem
     * @type object
     * @prevFileNamespace DevExpress.fileManagement
     * @public
     */
    dataItem: any;

    /**
     * @docid FileSystemItemMethods.getFileExtension
     * @publicName getFileExtension()
     * @return string
     * @prevFileNamespace DevExpress.fileManagement
     * @public
     */
    getFileExtension(): string;
}
