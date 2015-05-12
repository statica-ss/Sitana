using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sitana.Framework.IO
{
    public abstract class StorageManager: IDisposable
    {
        public abstract bool FileExists(string path);
        public abstract bool DirectoryExists(string path);

        public abstract string[] GetFileNames(string pattern);

        public abstract void DeleteFile(string name);
        public abstract void DeleteDirectory(string name);

        public abstract void CreateDirectory(string name);

        public abstract Stream OpenFile(string name, FileMode mode);

        public abstract void Dispose();

        void IDisposable.Dispose()
        {
            Dispose();
        }
    }
}
