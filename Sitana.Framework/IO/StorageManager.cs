using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sitana.Framework.IO
{
    #pragma warning disable 1998
    public abstract class StorageManager: IDisposable
    {
        public async virtual Task<bool> FileExists(string path)
        {
            return false;
        }

        public async virtual Task<bool> DirectoryExists(string path)
        {
            return false;
        }

        public async virtual Task<string[]> GetFileNames(string pattern)
        {
            return null;
        }

        public async virtual Task DeleteFile(string name)
        {
            
        }
        public async virtual Task DeleteDirectory(string name)
        {
        }

        public async virtual Task CreateDirectory(string name)
        {
        }

        public async virtual Task<Stream> OpenFile(string name, FileMode mode)
        {
            return null;
        }

        public abstract void Dispose();

        void IDisposable.Dispose()
        {
            Dispose();
        }
    }
    #pragma warning restore 1998
}
