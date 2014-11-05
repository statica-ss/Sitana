// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using System.IO;
using Microsoft.Xna.Framework;


#if WINRT
using System.Threading.Tasks;
#elif IOS
using MonoTouch.Foundation;
using MonoTouch.UIKit;
#elif MONOMAC
using MonoMac.Foundation;
#elif PSM
using Sce.PlayStation.Core;
#endif

namespace Sitana.Framework.Content
{
    public static class TitleContainerEx
    {
#if WINRT
            static char notSeparator = '/';
            static char separator = '\\';
#else
        static char notSeparator = '\\';
        static char separator = System.IO.Path.DirectorySeparatorChar;
#endif

        static TitleContainerEx()
        {
#if WINDOWS || LINUX
            Location = AppDomain.CurrentDomain.BaseDirectory;
#elif WINRT
            Location = Windows.ApplicationModel.Package.Current.InstalledLocation.Path;
#elif IOS || MONOMAC
			Location = NSBundle.MainBundle.ResourcePath;
#elif PSM
			Location = "/Application";
#else
            Location = string.Empty;
#endif

#if IOS
			SupportRetina = UIScreen.MainScreen.Scale == 2.0f;
#endif
        }

        static internal string Location { get; private set; }
#if IOS
        static internal bool SupportRetina { get; private set; }
#endif

#if WINRT

        private static async Task<Stream> OpenStreamAsync(string name)
        {
            var package = Windows.ApplicationModel.Package.Current;

            try
            {
                var storageFile = await package.InstalledLocation.GetFileAsync(name);
                var randomAccessStream = await storageFile.OpenReadAsync();
                return randomAccessStream.AsStreamForRead();
            }
            catch (IOException)
            {
                // The file must not exist... return a null stream.
                return null;
            }
        }

#endif // WINRT

        /// <summary>
        /// Returns an open stream to an exsiting file in the title storage area.
        /// </summary>
        /// <param name="name">The filepath relative to the title storage area.</param>
        /// <returns>A open stream or null if the file is not found.</returns>
        public static bool FileExists(string name)
        {
            // Normalize the file path.
            var safeName = GetFilename(name);

            if (Path.IsPathRooted(safeName))
            {
                return false;
            }

#if WINRT
            var stream = Task.Run( () => OpenStreamAsync(safeName).Result ).Result;

            if (stream == null)
                return false;

            stream.Close();
            return true;

#elif ANDROID
			return false;
#elif IOS
            var absolutePath = Path.Combine(Location, safeName);
            return File.Exists(absolutePath);
#else
            var absolutePath = Path.Combine(Location, safeName);
            return File.Exists(absolutePath);
#endif
        }

        // TODO: This is just path normalization.  Remove this
        // and replace it with a proper utility function.  I'm sure
        // this same logic is duplicated all over the code base.
        internal static string GetFilename(string name)
        {
            return NormalizeFilePathSeparators(new Uri("file:///" + name).LocalPath.Substring(1));
        }

        static string NormalizeFilePathSeparators(string name)
        {
            return name.Replace(notSeparator, separator);
        }
    }
}