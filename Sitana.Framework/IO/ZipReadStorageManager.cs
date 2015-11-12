using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using ICSharpCode.SharpZipLib.Zip;

namespace Sitana.Framework.IO
{
    #pragma warning disable 1998
    public class ZipReadStorageManager : StorageManager
    {
        ZipFile _zipFile;

        public ZipReadStorageManager(Stream stream)
        {
            _zipFile = new ZipFile(stream);
        }

        public async override Task<bool> FileExists(string path)
        {
            bool exists = _zipFile.FindEntry(path, false) != -1;
            return exists;
        }

        public async override Task<bool> DirectoryExists(string path)
        {
            path = path.Replace('\\', '/').Trim('/') + '/';

            bool exists = _zipFile.FindEntry(path, false) != -1;
            return exists;
        }

        public async override Task<string[]> GetFileNames(string wildcard)
        {
            string pattern = "^" + Regex.Escape(ZipEntry.CleanName(wildcard)).Replace(@"\*", ".*").Replace(@"\?", ".") + "$";

            // Now, run the Regex as you already know
            Regex regex;
            regex = new Regex(pattern, RegexOptions.IgnoreCase);

            List<string> files = new List<string>();

            foreach (ZipEntry entry in _zipFile)
            {
                if(!entry.IsDirectory)
                {
                    if(regex.IsMatch(entry.Name))
                    {
                        files.Add(Path.GetFileName(entry.Name));
                    }
                }
            }

            return files.ToArray();
        }

        public async override Task CreateDirectory(string name)
        {
            throw new NotImplementedException();
        }

        public async override Task DeleteFile(string name)
        {
            throw new NotImplementedException();
        }

        public async override Task DeleteDirectory(string name)
        {
            throw new NotImplementedException();
        }

        public async override Task<Stream> OpenFile(string name, FileMode mode)
        {
            if(mode != FileMode.Open)
            {
                throw new NotImplementedException();
            }

            ZipEntry entry = _zipFile.GetEntry(ZipEntry.CleanName(name));

            if(entry == null)
            {
                throw new FileNotFoundException("Cannot find file in zip archive.", name);
            }

            var stream = _zipFile.GetInputStream(entry);
            return stream;
        }

        public override void Dispose()
        {
            _zipFile.IsStreamOwner = true;
            _zipFile.Close();
        }
    }
    #pragma warning restore 1998
}
