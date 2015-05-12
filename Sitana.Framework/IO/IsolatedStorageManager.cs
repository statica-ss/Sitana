using Sitana.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sitana.Framework.IO
{
    public class IsolatedStorageManager: StorageManager
    {
        IsolatedStorageFile _storage;

        public IsolatedStorageManager()
        {
            _storage = Platform.GetUserStoreForApplication();
        }

        public override bool FileExists(string path)
        {
            return _storage.FileExists(path);
        }

        public override bool DirectoryExists(string path)
        {
            return _storage.DirectoryExists(path);
        }

        public override string[] GetFileNames(string pattern)
        {
            return _storage.GetFileNames(pattern);
        }

        public override void CreateDirectory(string name)
        {
            _storage.CreateDirectory(name);
        }

        public override void DeleteFile(string name)
        {
            _storage.DeleteFile(name);
        }

        public override void DeleteDirectory(string name)
        {
            _storage.DeleteDirectory(name);
        }

        public override Stream OpenFile(string name, FileMode mode)
        {
            return _storage.OpenFile(name, mode);
        }

        public override void Dispose()
        {
            if (_storage != null)
            {
                _storage.Dispose();
                _storage = null;
            }
        }
    }
}
