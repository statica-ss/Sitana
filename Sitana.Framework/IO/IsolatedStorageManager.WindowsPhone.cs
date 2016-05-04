using Sitana.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Search;
using Microsoft.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;
using Sitana.Framework.Common.Misc;

namespace Sitana.Framework.IO
{
    public class IsolatedStorageManager: StorageManager
    {
        StorageFolder _storage;

        public IsolatedStorageManager()
        {
            _storage = ApplicationData.Current.LocalFolder;
        }

        public override bool FileExists(string path)
        {
            return Task.Run<bool>(async () =>
                {
                    try
                    {
                        var file = await _storage.GetFileAsync(path);
                        return file != null;
                    }
                    catch
                    {

                    }

                    return false;
                }).Result;
        }

        public override bool DirectoryExists(string path)
        {
            return Task.Run<bool>(async () =>
            {
                try
                {
                    var folder = await _storage.GetFolderAsync(path);
                    return folder != null;
                }
                catch
                {

                }

                return false;
            }).Result;
        }

        public override string[] GetFileNames(string pattern)
        {
            string dir = Path.GetDirectoryName(pattern);
            pattern = Path.GetFileName(pattern);

            IReadOnlyList<StorageFile> files = null;

            if (dir.IsNullOrWhiteSpace())
            {
                files = Task.Run(async () => await _storage.GetFilesAsync(CommonFileQuery.DefaultQuery)).Result;
            }
            else
            {
                var folder = Task.Run(async () => await _storage.GetFolderAsync(dir)).Result;
                files = Task.Run(async () => await folder.GetFilesAsync()).Result;
            }

			List<string> filePaths = new List<string>();

			Wildcard wildcard = new Wildcard(pattern, System.Text.RegularExpressions.RegexOptions.IgnoreCase);

			foreach(var file in files)
			{
				if(wildcard.IsMatch(file.Name))
				{
					filePaths.Add(file.Name);
				}
			}

			return filePaths.ToArray();
        }

        public override void CreateDirectory(string name)
        {
            Task.Run(async () => await _storage.CreateFolderAsync(name)).Wait();
        }

        public override void DeleteFile(string name)
        {
            Task.Run(async () =>
                {
                    var file = await _storage.GetFileAsync(name);
                    await file.DeleteAsync();
                }).Wait();
        }

        public override void DeleteDirectory(string name)
        {
            Task.Run(async () =>
            {
                var folder = await _storage.GetFolderAsync(name);
                await folder.DeleteAsync();
            }).Wait();
        }

        public override Stream OpenFile(string name, OpenFileMode mode)
        {
            switch (mode)
            {
                case OpenFileMode.Create:
                    return Task.Run(() => _storage.OpenStreamForWriteAsync(name, CreationCollisionOption.ReplaceExisting).Result).Result;

                case OpenFileMode.Open:
                    return Task.Run(() => _storage.OpenStreamForReadAsync(name)).Result;

                case OpenFileMode.Append:
                    return Task.Run(() => _storage.OpenStreamForWriteAsync(name, CreationCollisionOption.OpenIfExists)).Result;
            }
            

            throw new NotImplementedException();
        }

        public override void Dispose()
        {
			_storage = null;   
        }
    }
}
