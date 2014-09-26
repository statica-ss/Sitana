/// This file is a part of the EBATIANOS.ESSENTIALS class library.
/// (C)2013-2014 Sebastian Sejud. All rights reserved.
///
/// THIS SOURCE FILE IS THE PROPERTY OF SEBASTIAN SEJUD AND IS NOT TO BE 
/// RE-DISTRIBUTED BY ANY MEANS WHATSOEVER WITHOUT THE EXPRESSED WRITTEN 
/// CONSENT OF SEBASTIAN SEJUD.
/// 
/// THIS SOURCE CODE CAN ONLY BE USED UNDER THE TERMS AND CONDITIONS OUTLINED
/// IN THE EBATIANOS.ESSENTIALS LICENSE AGREEMENT. SEBASTIAN SEJUD GRANTS
/// TO YOU (ONE SOFTWARE DEVELOPER) THE LIMITED RIGHT TO USE THIS SOFTWARE 
/// ON A SINGLE COMPUTER.
///
/// CONTACT INFORMATION:
/// essentials@sejud.com
/// sejud.com/essentials-library
/// 
///---------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ebatianos.Cs
{
    public class Singleton<T>
    {
        // Instance of type.
        private static T _instance;

        // Flag to avoid creation of an instance using new.
        private static Boolean _allowCreation = false;

        // Lock object for synchronization.
        private static Object _lockObj = new Object();

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
                        _instance = (T)Activator.CreateInstance(typeof(T));
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
