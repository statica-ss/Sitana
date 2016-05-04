// SITANA - Copyright (C) The Sitana Team.
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using Sitana.Framework.IO;
using Sitana.Framework.Serialization;
using Sitana.Framework.Xml;
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
            // Open isolated storage.
            using (var storageManager = new IsolatedStorageManager())
            {
                fileName = Prepare(storageManager, subDirectory, fileName);

                // Open file from storage.
                using (Stream stream = storageManager.OpenFile(fileName, IO.OpenFileMode.Create))
                {
                    XFile file = XFile.Create(fileName);

                    XNode fileNode = file;

                    var node = new XNode(file, "Serializator");
                    ((XNode)file).Nodes.Add(node);

                    // Create serializer for type.
                    var serializer = new XSerializer(node);
                    serializer.Serialize("Data", obj);

                    file.WriteBinary(stream);
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
            try
            {
                // Open isolated storage.
                using (var storageManager = new IsolatedStorageManager())
                {
                    fileName = Prepare(storageManager, subDirectory, fileName);

                    if (storageManager.FileExists(fileName))
                    {
                        // Open file from storage.
                        using(var stream = storageManager.OpenFile(fileName, IO.OpenFileMode.Open))
                        {
                            XFile file = XFile.LoadBinary(stream, fileName);

                            var serializer = new XSerializer(file);

                            return serializer.Deserialize<T>("Data");
                        }
                    }
                }
            }
            catch(Exception ex)
            {
                Debug.WriteLine(ex.ToString());
            }

            return default(T);
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
    }
}
