using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Sitana.Framework.DummyZipLib
{
    class ZipFile
    {
        public String Password{set{}}

        public ZipFile(string path)
        {
            throw new NotImplementedException("Sitana.Framework doesn't use SharpZipLib on this platform.");
        }

        public object GetEntry(string name)
        {
            return null;
        }

        public Stream GetInputStream(object entry)
        {
            return null;
        }
    }
}
