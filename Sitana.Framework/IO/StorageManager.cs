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
        public virtual bool FileExists(string path)
        {
            return false;
        }

        public virtual bool DirectoryExists(string path)
        {
            return false;
        }

        public virtual string[] GetFileNames(string pattern)
        {
            return null;
        }

        public virtual void DeleteFile(string name)
        {
            
        }
        public virtual void DeleteDirectory(string name)
        {
        }

        public virtual void CreateDirectory(string name)
        {
        }

        public virtual Stream OpenFile(string name, FileMode mode)
        {
            return null;
        }

        public abstract void Dispose();

        void IDisposable.Dispose()
        {
            Dispose();
        }
    }

}
