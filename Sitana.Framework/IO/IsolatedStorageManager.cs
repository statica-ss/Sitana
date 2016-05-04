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
            _storage = Platform.UserStore;
        }

        public override bool FileExists(string path)
        {
            bool result = _storage.FileExists(path);
            return result;
        }

        public override bool DirectoryExists(string path)
        {
            bool result = _storage.DirectoryExists(path);
            return result;
        }

        public override string[] GetFileNames(string pattern)
        {
            string[] result = _storage.GetFileNames(pattern);
            return result;
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

        public override Stream OpenFile(string name, OpenFileMode mode)
        {
            FileMode fileMode = FileMode.Open;

            switch(mode)
            {
                case OpenFileMode.Create:
                    fileMode = FileMode.Create;
                    break;

                case OpenFileMode.Append:
                    fileMode = FileMode.Append;
                    break;
            }

            Stream result = _storage.OpenFile(name, fileMode);
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
}
