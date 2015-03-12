// SITANA - Copyright (C) The Sitana Team.
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using System.Reflection;

namespace Sitana.Framework.Settings
{
    /// <summary>
    /// Serialization automated singleton base, which serializes all fields of derrived type to isolated storage. 
    /// </summary>
    /// <typeparam name="T">Type to be serialized, it must be the type you derrive from this class.</typeparam>
    public abstract class SingletonSettings<T>
    {
        // Defines if type has been loaded or created.
        [System.Xml.Serialization.XmlIgnoreAttribute]
        public bool Loaded { get; private set; }

        // Instance of type.
        private static T _instance;

        // Flag to avoid creation of an instance using new.
        private static bool _allowCreation = false;

        // LockedListener object for synchronization.
        private static object _lockObj = new object();

        // Returns instance of singleton class.
        public static T Instance
        {
            get
            {
                lock (_lockObj)
                {
                    // If no instance yet, try load it from file, otherwise create new one.
                    if (_instance == null)
                    {
                        // Set created flag to true to avoid wild creation of class.
                        _allowCreation = true;

                        // Path is full type name.
                        var path = Serializator.PathFromType(typeof(T));

                        // Try deserialze.
                        _instance = (T)Serializator.Deserialize<T>(path);

                        // If not deserialized create instance of type and set it wasn't loaded.
                        if (_instance == null)
                        {
                            _instance = (T)Activator.CreateInstance(typeof(T));

                            (_instance as SingletonSettings<T>).Loaded = false;
                        }
                        else
                        {
                            // Set that settings have been loaded.
                            (_instance as SingletonSettings<T>).Loaded = true;
                        }

                        (_instance as SingletonSettings<T>).Init();

                        _allowCreation = false;
                    }

                    // return singleton instance.
                    return _instance;
                }
            }
        }

        /// <summary>
        /// Protected constructor throwing if additional not singleton instance is created.
        /// </summary>
        protected SingletonSettings()
        {
            lock (_lockObj)
            {
                if (!_allowCreation)
                {
                    throw new Exception("Cannot create an instance of singleton.");
                }
            }
        }

        /// <summary>
        /// Serializes setings.
        /// </summary>
        public void Serialize()
        {
            try
            {
                _allowCreation = true;

                // Create path.
                var path = Serializator.PathFromType(typeof(T));

                // Serialize with serializator.
                Serializator.Serialize(path, Instance);
            }
            finally
            {
                _allowCreation = false;
            }

        }

        protected virtual void Init()
        {
        }
    }
}
