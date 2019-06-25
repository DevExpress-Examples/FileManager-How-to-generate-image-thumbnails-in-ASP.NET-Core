using DevExtreme.AspNet.Mvc.FileManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FileManagerThumbs.Models {
    public class ClientFileSystemItem : IClientFileSystemItem {
        public ClientFileSystemItem() {
            CustomFields = new Dictionary<string, object>();
        }
        public string Id { get; set; }
        public string Name { get; set; }
        public DateTime DateModified { get; set; }
        public bool IsDirectory { get; set; }
        public long Size { get; set; }
        public bool HasSubDirectories { get; set; }
        public IDictionary<string, object> CustomFields { get; }
    }
}
