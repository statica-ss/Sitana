// SITANA - Copyright (C) The Sitana Team.
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;

namespace Sitana.Framework.Cs
{
    public class Singleton<T> where T: class, new()
    {
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
                        _instance = new T();
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
        protected Singleton()
        {
            lock (_lockObj)
            {
                if (!_allowCreation)
                {
                    throw new Exception("Cannot create an instance of singleton.");
                }
            }
        }
    }
}
