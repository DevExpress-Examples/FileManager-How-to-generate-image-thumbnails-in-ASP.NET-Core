/**
* DevExtreme (file_management/upload_info.d.ts)
* Version: 20.1.3
* Build date: Fri Apr 24 2020
*
* Copyright (c) 2012 - 2020 Developer Express Inc. ALL RIGHTS RESERVED
* Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
*/
/** Warning! This type is used for internal purposes. Do not import it directly. */
/** An object that provides information about the file upload session. */
export default interface UploadInfo {
    /**
     * @docid UploadInfo.bytesUploaded
     * @type Number
     * @prevFileNamespace DevExpress.fileManagement
     * @public
     */
    bytesUploaded: number;

    /**
     * @docid UploadInfo.chunkCount
     * @type Number
     * @prevFileNamespace DevExpress.fileManagement
     * @public
     */
    chunkCount: number;

    /**
     * @docid UploadInfo.customData
     * @type object
     * @prevFileNamespace DevExpress.fileManagement
     * @public
     */
    customData: any;

    /**
     * @docid UploadInfo.chunkBlob
     * @type Blob
     * @prevFileNamespace DevExpress.fileManagement
     * @public
     */
    chunkBlob: Blob;

    /**
     * @docid UploadInfo.chunkIndex
     * @type Number
     * @prevFileNamespace DevExpress.fileManagement
     * @public
     */
    chunkIndex: number;
}