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
            _storage = Platform.GetUserStoreForApplication();
        }

        public async override Task<bool> FileExists(string path)
        {
            var file = await _storage.GetFileAsync(path);
            return file != null;
        }

        public async override Task<bool> DirectoryExists(string path)
        {
            var folder = await _storage.GetFolderAsync(path);
            return folder != null;
        }

        public async override Task<string[]> GetFileNames(string pattern)
        {
            var files = await _storage.GetFilesAsync(CommonFileQuery.DefaultQuery);

			return await Task.Run<string[]>( ()=>
				{
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
                });
        }

        public async override Task CreateDirectory(string name)
        {
            await _storage.CreateFolderAsync(name);
        }

        public async override Task DeleteFile(string name)
        {
            var file = await _storage.GetFileAsync(name);
            await file.DeleteAsync();
        }

        public async override Task DeleteDirectory(string name)
        {
            var folder = await _storage.GetFolderAsync(name);
            await folder.DeleteAsync();
        }

        public async override Task<Stream> OpenFile(string name, FileMode mode)
        {
			switch(mode)
            {
				case FileMode.Create:
                    return await _storage.OpenStreamForWriteAsync(name, CreationCollisionOption.ReplaceExisting);

                case FileMode.Open:
                    return await _storage.OpenStreamForReadAsync(name);

                case FileMode.Append:
                    return await _storage.OpenStreamForWriteAsync(name, CreationCollisionOption.OpenIfExists);
            }

            throw new NotImplementedException();
        }

        public override void Dispose()
        {
			_storage = null;   
        }
    }
}
