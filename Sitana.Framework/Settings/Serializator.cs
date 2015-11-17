// SITANA - Copyright (C) The Sitana Team.
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using Sitana.Framework.IO;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;


namespace Sitana.Framework.Settings
{
    /// <summary>
    /// Serializes any kind of objects with XmlSerializer to isolated storage file.
    /// </summary>
    public static class Serializator
    {
        public static void Serialize(string fileName, Object obj)
        {
            Serialize(null, fileName, obj);
        }

        /// <summary>
        /// Serializes object to isolated storage file.
        /// </summary>
        /// <param name="fileName">file name.</param>
        /// <param name="obj">Object to serialize.</param>
        public static void Serialize(string subDirectory, string fileName, Object obj)
        {
            // Write to the Isolated Storage
            var xmlWriterSettings = new XmlWriterSettings { Indent = true };

            // Open isolated storage.
            using (var storageManager = new IsolatedStorageManager())
            {
                fileName = Prepare(storageManager, subDirectory, fileName);

                // Open file from storage.
                using (Stream stream = storageManager.OpenFile(fileName, FileMode.Create))
                {
                    // Create serializer for type.
                    var serializer = new XmlSerializer(obj.GetType());

                    // Create XmlWriter.
                    using (var xmlWriter = XmlWriter.Create(stream, xmlWriterSettings))
                    {
                        // Serialize object.
                        serializer.Serialize(xmlWriter, obj);
                    }
                }
            }
        }

        public static T Deserialize<T>(string fileName)
        {
            return Deserialize<T>(null, fileName);
        }

        /// <summary>
        /// Deserializes object from isolated storage file.
        /// </summary>
        /// <typeparam name="T">Type of object.</typeparam>
        /// <param name="fileName">Path to isolated storage file.</param>
        /// <returns></returns>
        public static T Deserialize<T>(string subDirectory, string fileName)
        {
            T obj = default(T);

            try
            {
                // Open isolated storage.
                using (var storageManager = new IsolatedStorageManager())
                {
                    fileName = Prepare(storageManager, subDirectory, fileName);

                    if (storageManager.FileExists(fileName))
                    {
                        // Open file from storage.
                        using(var stream = storageManager.OpenFile(fileName, FileMode.Open))
                        {
                            // Create serializer for type.
                            var serializer = new XmlSerializer(typeof(T));

                            // Deserialize object.
                            obj = (T)serializer.Deserialize(stream);
                        }
                    }
                }
            }
            catch(Exception ex)
            {
                Debug.WriteLine(ex.ToString());
            }

            return obj;
        }

        public static bool FileExist(string path)
        {
            using (var storageManager = new IsolatedStorageManager())
            {
                return storageManager.FileExists(path);
            }
        }

        public static string PathFromType(Type type)
        {
            return type.Name + "__(" + type.Namespace + ").xml";
        }

        static string Prepare(StorageManager storageManager, string subDirectory, string fileName)
        {
            if(!string.IsNullOrWhiteSpace(subDirectory))
            {
                if (! storageManager.DirectoryExists(subDirectory))
                {
                    storageManager.CreateDirectory(subDirectory);
                }

                fileName = Path.Combine(subDirectory, fileName);
            }

            return fileName;
        }

        public static object Deserialize(string subDirectory, string fileName, Type[] possibleTypes)
        {
            try
            {
                if(possibleTypes != null && possibleTypes.Length > 0)
                {
                    using(var storageManager = new IsolatedStorageManager())
                    {
                        fileName = Prepare(storageManager, subDirectory, fileName);

                        if (storageManager.FileExists(fileName))
                        {
                            using (var stream = storageManager.OpenFile(fileName, FileMode.Open))
                            {                                
                                using(var reader = XmlReader.Create(stream))
                                {
                                    foreach(var extraType in possibleTypes)
                                    {
                                        var extraTypeSerializer = new XmlSerializer(extraType);

                                        if(extraTypeSerializer.CanDeserialize(reader))
                                        {
                                            stream.Position = 0;

                                            return extraTypeSerializer.Deserialize(stream);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch(Exception ex)
            {
                Debug.WriteLine(ex.ToString());
            }

            return null;
        }
    }
}
