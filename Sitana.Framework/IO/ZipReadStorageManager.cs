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

        public override bool FileExists(string path)
        {
            bool exists = _zipFile.FindEntry(path, false) != -1;
            return exists;
        }

        public override bool DirectoryExists(string path)
        {
            path = path.Replace('\\', '/').Trim('/') + '/';

            bool exists = _zipFile.FindEntry(path, false) != -1;
            return exists;
        }

        public override string[] GetFileNames(string wildcard)
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

        public override void CreateDirectory(string name)
        {
            throw new NotImplementedException();
        }

        public override void DeleteFile(string name)
        {
            throw new NotImplementedException();
        }

        public override void DeleteDirectory(string name)
        {
            throw new NotImplementedException();
        }

        public override Stream OpenFile(string name, FileMode mode)
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
