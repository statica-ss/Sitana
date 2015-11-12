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
    #pragma warning disable 1998
    public class IsolatedStorageManager: StorageManager
    {
        IsolatedStorageFile _storage;

        public IsolatedStorageManager()
        {
            _storage = Platform.GetUserStoreForApplication();
        }

        public async override Task<bool> FileExists(string path)
        {
            bool result = _storage.FileExists(path);
            return result;
        }

        public async override Task<bool> DirectoryExists(string path)
        {
            bool result = _storage.DirectoryExists(path);
            return result;
        }

        public async override Task<string[]> GetFileNames(string pattern)
        {
            string[] result = _storage.GetFileNames(pattern);
            return result;
        }

        public async override Task CreateDirectory(string name)
        {
            _storage.CreateDirectory(name);
        }

        public async override Task DeleteFile(string name)
        {
            _storage.DeleteFile(name);
        }

        public async override Task DeleteDirectory(string name)
        {
            _storage.DeleteDirectory(name);
        }

        public async override Task<Stream> OpenFile(string name, FileMode mode)
        {
            Stream result = _storage.OpenFile(name, mode);
            return result;
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
    #pragma warning restore 1998
}
