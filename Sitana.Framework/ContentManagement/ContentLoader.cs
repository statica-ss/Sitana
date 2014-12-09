// SITANA - Copyright (C) The Sitana Team.
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Audio;
using Sitana.Framework;
using System.Diagnostics;
using Sitana.Framework.Cs;
using System.Text;
using System;
using Sitana.Framework.Ui.DefinitionFiles;
using Sitana.Framework.Xml;
using Sitana.Framework.Graphics;

#if SHARP_ZIP_LIB
using ICSharpCode.SharpZipLib.Zip;
using System.Reflection;
#else
using Sitana.Framework.DummyZipLib;
#endif

namespace Sitana.Framework.Content
{
    /// <summary>
    /// Loads content for app
    /// </summary>
    public sealed class ContentLoader
    {
        /// <summary>
        /// Delegate for Load method for additional content types
        /// </summary>
        /// <param name="name">Name of resource</param>
        /// <param name="contentLoader">ContentLoader object</param>
        /// <returns></returns>
        public delegate Object LoadMethod(String name);

        /// <summary>
        /// Type for additional content types, implements registration of type in ContentManager.
        /// Every additional content type should derive from this class
        /// </summary>
        public class AdditionalType
        {
            protected static void RegisterType(Type type, LoadMethod loadMethod, Boolean overwriteExisting)
            {
                if (ContentLoader._additionalTypeHandlers.ContainsKey(type))
                {
                    if (!overwriteExisting)
                    {
                        return;
                    }

                    ContentLoader._additionalTypeHandlers.Remove(type);
                }

                ContentLoader._additionalTypeHandlers.Add(type, loadMethod);
            }
        }

        /// <summary>
        /// Map of load methods for additional types
        /// </summary>
        private static Dictionary<Type, LoadMethod> _additionalTypeHandlers = new Dictionary<Type, LoadMethod>();

        /// <summary>
        /// Content objects buffer for faster secondary load
        /// </summary>
        private Dictionary<String, Object> _loadedContentObjects = new Dictionary<String, Object>();

        /// <summary>
        /// List of ContentManagers, first has highest priority when loading content
        /// </summary>
        private ContentManager _contentManager = null;

        public Texture2D OnePixelWhiteTexture { get; internal set; }

        public Int32 TextureRescaleFactor { get; private set; }

        private List<Tuple<string, string>> _specialFolders = new List<Tuple<string, string>>();

        public T GetService<T>()
        {
            return (T)_contentManager.ServiceProvider.GetService(typeof(T));
        }

        /// <summary>
        /// Object for synchronization.
        /// </summary>
        private Object _lockObj = new Object();

        public static ContentLoader Current { get; private set; }

        private StringBuilder _pathBuilder = new StringBuilder();

        private ZipFile _zipFile = null;
        

#if RESOURCE_MANAGER_AVALIABLE

        public static void Init(IServiceProvider serviceProvider, Assembly assembly, string root)
        {
            ContentManager manager = new ResourcesContentManager(serviceProvider, assembly, root);
            Current = new ContentLoader(manager);

            RegisterTypes();
        }
#endif

        public static void Init(IServiceProvider serviceProvider, string root)
        {
            ContentManager manager = new ContentManager(serviceProvider, root);
            Current = new ContentLoader(manager);

            RegisterTypes();
        }



        public static void Init(IServiceProvider serviceProvider, string zipPath, string password)
        {
            ZipFile zipFile = new ZipFile(zipPath);
            zipFile.Password = password;

            ContentManager manager = new ZipContentManager(serviceProvider, zipFile);

            Current = new ContentLoader(manager, zipFile);

            RegisterTypes();
        }

        private static void RegisterTypes()
        {
            NinePatchImage.Register();
            ModelXLoader.Register();
            XFile.Register();
            DefinitionFile.Register();
            FontLoader.Register();

            Current.TextureRescaleFactor = 0;
        }

        public void SetTextureRescaleFactor(Int32 factor)
        {
            TextureRescaleFactor = factor;
        }

        /// <summary>
        /// Initializes ContentLoader
        /// </summary>
        /// <param name="services">Services container from Game class - allows creating ContentManagers</param>
        /// <param name="contentPaths">Paths for each content manager</param>
        private ContentLoader(ContentManager manager)
        {
            _contentManager = manager;
        }

        /// <summary>
        /// Initializes ContentLoader
        /// </summary>
        /// <param name="services">Services container from Game class - allows creating ContentManagers</param>
        /// <param name="contentPaths">Paths for each content manager</param>
        private ContentLoader(ContentManager manager, ZipFile zipFile)
        {
            _contentManager = manager;
            _zipFile = zipFile;
        }

        private String ContentName(Type type, String name)
        {
            return type.Name + ":" + name;
        }

        private String ContentName<T>(String name)
        {
            return ContentName(typeof(T), name);
        }

        public void AddSpecialFolder(string id, string name)
        {
            _specialFolders.Add(new Tuple<string, string>('[' + id + ']', name));
        }

        /// <summary>
        /// Returns if content with given name and type was loaded.
        /// </summary>
        /// <typeparam name="T">Type of content.</typeparam>
        /// <param name="name">Name of resource.</param>
        /// <returns></returns>
        public Boolean IsContentLoaded<T>(String name)
        {
            return IsContentLoaded(typeof(T), name);
        }

        string FullPath(string name)
        {
            _pathBuilder.Clear();
            _pathBuilder.Append(name);
            
            for (int idx = 0; idx < _specialFolders.Count; ++idx)
            {
                _pathBuilder.Replace(_specialFolders[idx].Item1, _specialFolders[idx].Item2);
            }

            return _pathBuilder.ToString();
        }

        /// <summary>
        /// Returns if content with given name and type was loaded.
        /// </summary>
        /// <param name="type">Type of resource.</param>
        /// <param name="name">Name of resource.</param>
        /// <returns></returns>
        public Boolean IsContentLoaded(Type type, String name)
        {
            String searchName = ContentName(type, FullPath(name));

            lock (_lockObj)
            {
                return _loadedContentObjects.ContainsKey(searchName);
            }
        }

        /// <summary>
        /// Adds preloaded content to content container.
        /// </summary>
        /// <param name="name">Name of resource.</param>
        /// <param name="contentObj">Content object.</param>
        public void AddContent(String name, Type type, Object contentObj)
        {
            String searchName = ContentName(type, FullPath(name));

            lock (_lockObj)
            {
                // Add object to loaded objects.
                _loadedContentObjects.Add(searchName, contentObj);
            }
        }

        /// <summary>
        /// Loads content from resources
        /// </summary>
        /// <typeparam name="T">Type of content object</typeparam>
        /// <param name="name">Name of resource</param>
        /// <returns>Loaded content object</returns>
        public T Load<T>(String name)
        {
            name = FullPath(name);

            String searchName = ContentName<T>(name);

            // Check if this resource wasn't already loaded
            Object typeObject = null;

            lock (_lockObj)
            {
                if (_loadedContentObjects.TryGetValue(searchName, out typeObject))
                {
                    return (T)typeObject;
                }
            }

            // Search for registered additional types and their loaders
            LoadMethod loadMethod = null;

            lock (_lockObj)
            {
                if (_additionalTypeHandlers.TryGetValue(typeof(T), out loadMethod))
                {
                    // Call load method to load the object
                    typeObject = loadMethod(name);

                    // If loaded correctly, add object to the buffer; check if it already has object added by LoadNative method which may be called
                    // from loadMethod.
                    if (typeObject != null && !_loadedContentObjects.ContainsKey(searchName))
                    {
                        _loadedContentObjects.Add(searchName, typeObject);
                    }

                    // Return loaded object
                    return (T)typeObject;
                }
            }

            return LoadNative<T>(name);
        }

        /// <summary>
        /// Loads object using native method of ContentManagers
        /// </summary>
        /// <typeparam name="T">Tyoe of content object</typeparam>
        /// <param name="name">Name of resource</param>
        /// <returns>Loaded content object</returns>
        internal T LoadNative<T>(String name)
        {
            String searchName = ContentName<T>(name);

            lock (_lockObj)
            {
                T loadObject = _contentManager.Load<T>(name);

                if (loadObject != null)
                {
                    _loadedContentObjects.Add(searchName, loadObject);
                    return loadObject;
                }
            }

            // If no resource was loaded, that means the resource file doesn't exist
            throw new FileNotFoundException("Couldn't find file: " + name);
        }

        /// <summary>
        /// Opens stream for given file path
        /// </summary>
        /// <param name="name">file path</param>
        /// <returns></returns>
        public Stream Open(String name)
        {
            // Convert path from backslashes to slashes format
            name = FullPath(name);

            if (_zipFile != null)
            {
                var entry = _zipFile.GetEntry(name.Replace('\\', '/'));
                if (entry != null)
                {
                    return _zipFile.GetInputStream(entry);
                }
            }

            #if RESOURCE_MANAGER_AVALIABLE

            if (_contentManager is ResourcesContentManager)
            {
                return (_contentManager as ResourcesContentManager).Open(name);
            }

            #endif

            try
            {
                return TitleContainer.OpenStream(Path.Combine(_contentManager.RootDirectory, name));
            }
            catch (Exception)
            {
                try
                {
                    return new FileStream(Path.Combine(_contentManager.RootDirectory, name), FileMode.Open);
                }
                catch { }
            }

            // throw that file wasn't found
            throw new FileNotFoundException("Unable to open file \"" + name + "\"");
        }

        public bool IsFile(string name)
        {
            name = FullPath(name);
            return TitleContainerEx.FileExists(name);
        }

        public static Type FindType(String name)
        {
            foreach (var handler in _additionalTypeHandlers)
            {
                if (handler.Key.Name.ToUpperInvariant() == name.ToUpperInvariant())
                {
                    return handler.Key;
                }
            }

            switch (name.ToUpperInvariant())
            {
                case "TEXTURE":
                case "TEXTURE2D":
                    return typeof(Texture2D);

                case "SONG":
                    return typeof(Song);

                case "SOUND":
                case "SOUNDEFFECT":
                    return typeof(SoundEffect);
            }

            return null;
        }
    }
}
