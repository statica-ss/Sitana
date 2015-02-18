// SITANA - Copyright (C) The Sitana Team.
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using System.Diagnostics;
using System.IO;
using System.IO.IsolatedStorage;
using System.Xml;
using System.Xml.Serialization;

namespace Sitana.Framework.Settings
{
    /// <summary>
    /// Serializes any kind of objects with XmlSerializer to isolated storage file.
    /// </summary>
    public static class Serializator
    {
        public static void Serialize(String fileName, Object obj)
        {
            Serialize(null, fileName, obj);
        }

        /// <summary>
        /// Serializes object to isolated storage file.
        /// </summary>
        /// <param name="fileName">file name.</param>
        /// <param name="obj">Object to serialize.</param>
        public static void Serialize(String subDirectory, String fileName, Object obj)
        {
            // Write to the Isolated Storage
            var xmlWriterSettings = new XmlWriterSettings { Indent = true };

            // Open isolated storage.
            using (var isolatedStorageFile = Platform.GetUserStoreForApplication())
            {
                Prepare(isolatedStorageFile, subDirectory, ref fileName);

                // Open file from storage.
                using(Stream stream = isolatedStorageFile.OpenFile(fileName, FileMode.Create))
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

        public static T Deserialize<T>(String fileName)
        {
            return Deserialize<T>(null, fileName);
        }

        /// <summary>
        /// Deserializes object from isolated storage file.
        /// </summary>
        /// <typeparam name="T">Type of object.</typeparam>
        /// <param name="fileName">Path to isolated storage file.</param>
        /// <returns></returns>
        public static T Deserialize<T>(String subDirectory, String fileName)
        {
            T obj = default(T);

            try
            {
                // Open isolated storage.
                using (var isolatedStorageFile = Platform.GetUserStoreForApplication())
                {
                    Prepare(isolatedStorageFile, subDirectory, ref fileName);

                    if(isolatedStorageFile.FileExists(fileName))
                    {
                        // Open file from storage.
                        using(var stream = isolatedStorageFile.OpenFile(fileName, FileMode.Open))
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

        public static bool FileExist(String path)
        {
            using (var isolatedStorageFile = Platform.GetUserStoreForApplication())
            {
                return isolatedStorageFile.FileExists(path);
            }
        }

        public static String PathFromType(Type type)
        {
            return type.Name + "__(" + type.Namespace + ").xml";
        }

        static void Prepare(IsolatedStorageFile  isolatedStorageFile, String subDirectory, ref String fileName)
        {
            if(!string.IsNullOrWhiteSpace(subDirectory))
            {
                if(!isolatedStorageFile.DirectoryExists(subDirectory))
                {
                    isolatedStorageFile.CreateDirectory(subDirectory);
                }

                fileName = Path.Combine(subDirectory, fileName);
            }
        }

        public static object Deserialize(String subDirectory, String fileName, Type[] possibleTypes)
        {
            try
            {
                if(possibleTypes != null && possibleTypes.Length > 0)
                {
                    using(var isolatedStorageFile = Platform.GetUserStoreForApplication())
                    {
                        Prepare(isolatedStorageFile, subDirectory, ref fileName);

                        if(isolatedStorageFile.FileExists(fileName))
                        {
                            using(var stream = isolatedStorageFile.OpenFile(fileName, FileMode.Open))
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
