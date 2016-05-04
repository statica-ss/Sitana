using System;
using System.IO;
using System.IO.Compression;

namespace Sitana.Framework.IO
{
    public class ZipWriteStorageManager: StorageManager
    {
        ZipArchive _zipFile;
        CompressionLevel _level = CompressionLevel.Optimal;

        public ZipWriteStorageManager(Stream output)
            : this(output, CompressionLevel.Optimal, null)
        {
        }

        public ZipWriteStorageManager(Stream output, CompressionLevel level)
            : this(output, level, null)
        {
        }

        public ZipWriteStorageManager(Stream output, CompressionLevel level, string password)
        {
            _zipFile = new ZipArchive(output, ZipArchiveMode.Create);
            _level = level;
        }

        public override bool FileExists(string path)
        {
            throw new NotImplementedException();
        }

        public override bool DirectoryExists(string path)
        {
            throw new NotImplementedException();
        }

        public override string[] GetFileNames(string wildcard)
        {
            throw new NotImplementedException();
        }

        public override void CreateDirectory(string name)
        {
            _zipFile.CreateEntry(name.Replace('\\', '/'), _level);
        }

        public override void DeleteFile(string name)
        {
            throw new NotImplementedException();
        }

        public override void DeleteDirectory(string name)
        {
            throw new NotImplementedException();
        }

        public override Stream OpenFile(string name, OpenFileMode mode)
        {
            if(mode != OpenFileMode.Create)
            {
                throw new NotImplementedException();
            }

            string entryName = name.Replace('\\', '/');
            var newEntry = _zipFile.CreateEntry(entryName, _level);

            return newEntry.Open();
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
