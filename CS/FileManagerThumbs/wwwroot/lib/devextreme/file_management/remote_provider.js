/**
 * DevExtreme (file_management/remote_provider.js)
 * Version: 20.1.3
 * Build date: Fri Apr 24 2020
 *
 * Copyright (c) 2012 - 2020 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
"use strict";
var _renderer = require("../core/renderer");
var _renderer2 = _interopRequireDefault(_renderer);
var _ajax = require("../core/utils/ajax");
var _ajax2 = _interopRequireDefault(_ajax);
var _common = require("../core/utils/common");
var _guid = require("../core/guid");
var _guid2 = _interopRequireDefault(_guid);
var _window = require("../core/utils/window");
var _iterator = require("../core/utils/iterator");
var _deferred = require("../core/utils/deferred");
var _events_engine = require("../events/core/events_engine");
var _events_engine2 = _interopRequireDefault(_events_engine);
var _provider_base = require("./provider_base");
var _provider_base2 = _interopRequireDefault(_provider_base);
var _data = require("../core/utils/data");

function _interopRequireDefault(obj) {
    return obj && obj.__esModule ? obj : {
        "default": obj
    }
}

function _typeof(obj) {
    "@babel/helpers - typeof";
    if ("function" === typeof Symbol && "symbol" === typeof Symbol.iterator) {
        _typeof = function(obj) {
            return typeof obj
        }
    } else {
        _typeof = function(obj) {
            return obj && "function" === typeof Symbol && obj.constructor === Symbol && obj !== Symbol.prototype ? "symbol" : typeof obj
        }
    }
    return _typeof(obj)
}

function _classCallCheck(instance, Constructor) {
    if (!(instance instanceof Constructor)) {
        throw new TypeError("Cannot call a class as a function")
    }
}

function _defineProperties(target, props) {
    for (var i = 0; i < props.length; i++) {
        var descriptor = props[i];
        descriptor.enumerable = descriptor.enumerable || false;
        descriptor.configurable = true;
        if ("value" in descriptor) {
            descriptor.writable = true
        }
        Object.defineProperty(target, descriptor.key, descriptor)
    }
}

function _createClass(Constructor, protoProps, staticProps) {
    if (protoProps) {
        _defineProperties(Constructor.prototype, protoProps)
    }
    if (staticProps) {
        _defineProperties(Constructor, staticProps)
    }
    return Constructor
}

function _inherits(subClass, superClass) {
    if ("function" !== typeof superClass && null !== superClass) {
        throw new TypeError("Super expression must either be null or a function")
    }
    subClass.prototype = Object.create(superClass && superClass.prototype, {
        constructor: {
            value: subClass,
            writable: true,
            configurable: true
        }
    });
    if (superClass) {
        _setPrototypeOf(subClass, superClass)
    }
}

function _setPrototypeOf(o, p) {
    _setPrototypeOf = Object.setPrototypeOf || function(o, p) {
        o.__proto__ = p;
        return o
    };
    return _setPrototypeOf(o, p)
}

function _createSuper(Derived) {
    return function() {
        var result, Super = _getPrototypeOf(Derived);
        if (_isNativeReflectConstruct()) {
            var NewTarget = _getPrototypeOf(this).constructor;
            result = Reflect.construct(Super, arguments, NewTarget)
        } else {
            result = Super.apply(this, arguments)
        }
        return _possibleConstructorReturn(this, result)
    }
}

function _possibleConstructorReturn(self, call) {
    if (call && ("object" === _typeof(call) || "function" === typeof call)) {
        return call
    }
    return _assertThisInitialized(self)
}

function _assertThisInitialized(self) {
    if (void 0 === self) {
        throw new ReferenceError("this hasn't been initialised - super() hasn't been called")
    }
    return self
}

function _isNativeReflectConstruct() {
    if ("undefined" === typeof Reflect || !Reflect.construct) {
        return false
    }
    if (Reflect.construct.sham) {
        return false
    }
    if ("function" === typeof Proxy) {
        return true
    }
    try {
        Date.prototype.toString.call(Reflect.construct(Date, [], function() {}));
        return true
    } catch (e) {
        return false
    }
}

function _getPrototypeOf(o) {
    _getPrototypeOf = Object.setPrototypeOf ? Object.getPrototypeOf : function(o) {
        return o.__proto__ || Object.getPrototypeOf(o)
    };
    return _getPrototypeOf(o)
}
var window = (0, _window.getWindow)();
var FILE_CHUNK_BLOB_NAME = "chunk";
var RemoteFileSystemProvider = function(_FileSystemProviderBa) {
    _inherits(RemoteFileSystemProvider, _FileSystemProviderBa);
    var _super = _createSuper(RemoteFileSystemProvider);

    function RemoteFileSystemProvider(options) {
        var _this;
        _classCallCheck(this, RemoteFileSystemProvider);
        options = (0, _common.ensureDefined)(options, {});
        _this = _super.call(this, options);
        _this._endpointUrl = options.endpointUrl;
        _this._hasSubDirsGetter = (0, _data.compileGetter)(options.hasSubDirectoriesExpr || "hasSubDirectories");
        return _this
    }
    _createClass(RemoteFileSystemProvider, [{
        key: "getItems",
        value: function(parentDir) {
            var _this2 = this;
            var pathInfo = parentDir.getFullPathInfo();
            return this._getEntriesByPath(pathInfo).then(function(result) {
                return _this2._convertDataObjectsToFileItems(result.result, pathInfo)
            })
        }
    }, {
        key: "renameItem",
        value: function(item, name) {
            return this._executeRequest("Rename", {
                pathInfo: item.getFullPathInfo(),
                isDirectory: item.isDirectory,
                name: name
            })
        }
    }, {
        key: "createDirectory",
        value: function(parentDir, name) {
            return this._executeRequest("CreateDir", {
                pathInfo: parentDir.getFullPathInfo(),
                name: name
            }).done(function() {
                if (parentDir && !parentDir.isRoot()) {
                    parentDir.hasSubDirectories = true
                }
            })
        }
    }, {
        key: "deleteItems",
        value: function(items) {
            var _this3 = this;
            return items.map(function(item) {
                return _this3._executeRequest("Remove", {
                    pathInfo: item.getFullPathInfo(),
                    isDirectory: item.isDirectory
                })
            })
        }
    }, {
        key: "moveItems",
        value: function(items, destinationDirectory) {
            var _this4 = this;
            return items.map(function(item) {
                return _this4._executeRequest("Move", {
                    sourcePathInfo: item.getFullPathInfo(),
                    sourceIsDirectory: item.isDirectory,
                    destinationPathInfo: destinationDirectory.getFullPathInfo()
                })
            })
        }
    }, {
        key: "copyItems",
        value: function(items, destinationFolder) {
            var _this5 = this;
            return items.map(function(item) {
                return _this5._executeRequest("Copy", {
                    sourcePathInfo: item.getFullPathInfo(),
                    sourceIsDirectory: item.isDirectory,
                    destinationPathInfo: destinationFolder.getFullPathInfo()
                })
            })
        }
    }, {
        key: "uploadFileChunk",
        value: function(fileData, chunksInfo, destinationDirectory) {
            if (0 === chunksInfo.chunkIndex) {
                chunksInfo.customData.uploadId = new _guid2.default
            }
            var args = {
                destinationId: destinationDirectory.relativeName,
                chunkMetadata: JSON.stringify({
                    UploadId: chunksInfo.customData.uploadId,
                    FileName: fileData.name,
                    Index: chunksInfo.chunkIndex,
                    TotalCount: chunksInfo.chunkCount,
                    FileSize: fileData.size
                })
            };
            var formData = new window.FormData;
            formData.append(FILE_CHUNK_BLOB_NAME, chunksInfo.chunkBlob);
            formData.append("arguments", JSON.stringify(args));
            formData.append("command", "UploadChunk");
            var deferred = new _deferred.Deferred;
            _ajax2.default.sendRequest({
                url: this._endpointUrl,
                method: "POST",
                dataType: "json",
                data: formData,
                upload: {
                    onprogress: _common.noop,
                    onloadstart: _common.noop,
                    onabort: _common.noop
                },
                cache: false
            }).done(function(result) {
                !result.success && deferred.reject(result) || deferred.resolve()
            }).fail(deferred.reject);
            return deferred.promise()
        }
    }, {
        key: "abortFileUpload",
        value: function(fileData, chunksInfo, destinationDirectory) {
            return this._executeRequest("AbortUpload", {
                uploadId: chunksInfo.customData.uploadId
            })
        }
    }, {
        key: "downloadItems",
        value: function(items) {
            var args = this._getDownloadArgs(items);
            var $form = (0, _renderer2.default)("<form>").css({
                display: "none"
            }).attr({
                method: "post",
                action: args.url
            });
            ["command", "arguments"].forEach(function(name) {
                (0, _renderer2.default)("<input>").attr({
                    type: "hidden",
                    name: name,
                    value: args[name]
                }).appendTo($form)
            });
            $form.appendTo("body");
            _events_engine2.default.trigger($form, "submit");
            setTimeout(function() {
                return $form.remove()
            })
        }
    }, {
        key: "getItemsContent",
        value: function(items) {
            var args = this._getDownloadArgs(items);
            var formData = new window.FormData;
            formData.append("command", args.command);
            formData.append("arguments", args.arguments);
            return _ajax2.default.sendRequest({
                url: args.url,
                method: "POST",
                responseType: "arraybuffer",
                data: formData,
                upload: {
                    onprogress: _common.noop,
                    onloadstart: _common.noop,
                    onabort: _common.noop
                },
                cache: false
            })
        }
    }, {
        key: "_getDownloadArgs",
        value: function(items) {
            var pathInfoList = items.map(function(item) {
                return item.getFullPathInfo()
            });
            var args = {
                pathInfoList: pathInfoList
            };
            var argsStr = JSON.stringify(args);
            return {
                url: this._endpointUrl,
                arguments: argsStr,
                command: "Download"
            }
        }
    }, {
        key: "_getItemsIds",
        value: function(items) {
            return items.map(function(it) {
                return it.relativeName
            })
        }
    }, {
        key: "_getEntriesByPath",
        value: function(pathInfo) {
            return this._executeRequest("GetDirContents", {
                pathInfo: pathInfo
            })
        }
    }, {
        key: "_executeRequest",
        value: function(command, args) {
            var method = "GetDirContents" === command ? "GET" : "POST";
            var deferred = new _deferred.Deferred;
            _ajax2.default.sendRequest({
                url: this._getEndpointUrl(command, args),
                method: method,
                dataType: "json",
                cache: false
            }).then(function(result) {
                !result.success && deferred.reject(result) || deferred.resolve(result)
            }, function(e) {
                return deferred.reject(e)
            });
            return deferred.promise()
        }
    }, {
        key: "_getEndpointUrl",
        value: function(command, args) {
            var queryString = this._getQueryString({
                command: command,
                arguments: JSON.stringify(args)
            });
            var separator = this._endpointUrl && this._endpointUrl.indexOf("?") > 0 ? "&" : "?";
            return this._endpointUrl + separator + queryString
        }
    }, {
        key: "_getQueryString",
        value: function(params) {
            var pairs = [];
            var keys = Object.keys(params);
            for (var i = 0; i < keys.length; i++) {
                var key = keys[i];
                var value = params[key];
                if (void 0 === value) {
                    continue
                }
                if (null === value) {
                    value = ""
                }
                if (Array.isArray(value)) {
                    this._processQueryStringArrayParam(key, value, pairs)
                } else {
                    var pair = this._getQueryStringPair(key, value);
                    pairs.push(pair)
                }
            }
            return pairs.join("&")
        }
    }, {
        key: "_processQueryStringArrayParam",
        value: function(key, array, pairs) {
            var _this6 = this;
            (0, _iterator.each)(array, function(_, item) {
                var pair = _this6._getQueryStringPair(key, item);
                pairs.push(pair)
            })
        }
    }, {
        key: "_getQueryStringPair",
        value: function(key, value) {
            return encodeURIComponent(key) + "=" + encodeURIComponent(value)
        }
    }, {
        key: "_hasSubDirs",
        value: function(dataObj) {
            var hasSubDirs = this._hasSubDirsGetter(dataObj);
            return "boolean" === typeof hasSubDirs ? hasSubDirs : true
        }
    }, {
        key: "_getKeyExpr",
        value: function(options) {
            return options.keyExpr || "key"
        }
    }]);
    return RemoteFileSystemProvider
}(_provider_base2.default);
module.exports = RemoteFileSystemProvider;
module.exports.default = module.exports;
