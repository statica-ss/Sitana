using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Text.RegularExpressions;

namespace Sitana.Framework.IO
{
    public class ZipReadStorageManager : StorageManager
    {
        ZipArchive _zipFile;

        public ZipReadStorageManager(Stream stream)
        {
            _zipFile = new ZipArchive(stream);
        }

        public override bool FileExists(string path)
        {
            var entry = _zipFile.GetEntry(path);
            return entry != null;
        }

        public override bool DirectoryExists(string path)
        {
            path = path.Replace('\\', '/').Trim('/') + '/';

            var entry = _zipFile.GetEntry(path);
            return entry != null;
        }

        public override string[] GetFileNames(string wildcard)
        { 
            string pattern = "^" + Regex.Escape(wildcard.Replace('\\', '/')).Replace(@"\*", ".*").Replace(@"\?", ".") + "$";

            // Now, run the Regex as you already know
            Regex regex;
            regex = new Regex(pattern, RegexOptions.IgnoreCase);

            List<string> files = new List<string>();

            foreach (var entry in _zipFile.Entries)
            {
                if(regex.IsMatch(entry.FullName))
                {
                    files.Add(Path.GetFileName(entry.Name));
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

            var entry = _zipFile.GetEntry(name.Replace('\\', '/'));

            if(entry == null)
            {
                throw new FileNotFoundException("Cannot find file in zip archive.", name);
            }

            return entry.Open();
        }

        public override void Dispose()
        {
            if (_zipFile != null)
            {
                _zipFile.Dispose();
                _zipFile = null;
            }
        }
    }
}
