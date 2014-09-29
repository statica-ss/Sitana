// SITANA - Copyright (C) The Sitana Team.
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
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
        /// <summary>
        /// Serializes object to isolated storage file.
        /// </summary>
        /// <param name="path">Path to isolated storage file.</param>
        /// <param name="obj">Object to serialize.</param>
        public static void Serialize(string path, object obj)
        {
            // Write to the Isolated Storage
            var xmlWriterSettings = new XmlWriterSettings { Indent = true };

            // Open isolated storage.
            using (var isolatedStorageFile = SystemWrapper.GetUserStoreForApplication())
            {
                // Open file from storage.
                using (Stream stream = isolatedStorageFile.OpenFile(path, FileMode.Create))
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

        /// <summary>
        /// Deserializes object from isolated storage file.
        /// </summary>
        /// <typeparam name="T">Type of object.</typeparam>
        /// <param name="path">Path to isolated storage file.</param>
        /// <returns></returns>
        public static T Deserialize<T>(string path)
        {
            T obj = default(T);

            try
            {
                // Open isolated storage.
                using (var isolatedStorageFile = SystemWrapper.GetUserStoreForApplication())
                {
                    // Open file from storage.
                    using (var stream = isolatedStorageFile.OpenFile(path, FileMode.Open))
                    {
                        // Create serializer for type.
                        var serializer = new XmlSerializer(typeof(T));

                        // Deserialize object.
                        obj = (T)serializer.Deserialize(stream);
                    }
                }
            }
            catch
            {// If deserialization fails method returns default(T) that is usually null.
            }

            return obj;
        }

        public static string PathFromType(Type type)
        {
            return type.Name + "__(" + type.Namespace + ").xml";
        }
    }
}
