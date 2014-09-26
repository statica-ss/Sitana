/// This file is a part of the EBATIANOS.ESSENTIALS class library.
/// (c)2013-2014 EBATIANO'S a.k.a. Sebastian Sejud. All rights reserved.
///
/// THIS SOURCE FILE IS THE PROPERTY OF EBATIANO'S A.K.A. SEBASTIAN SEJUD 
/// AND IS NOT TO BE RE-DISTRIBUTED BY ANY MEANS WHATSOEVER WITHOUT 
/// THE EXPRESSED WRITTEN CONSENT OF EBATIANO'S A.K.A. SEBASTIAN SEJUD.
///
/// THIS SOURCE CODE CAN ONLY BE USED UNDER THE TERMS AND CONDITIONS OUTLINED
/// IN THE EBATIANOS.ESSENTIALS LICENSE AGREEMENT. 
/// EBATIANO'S A.K.A. SEBASTIAN SEJUD GRANTS TO YOU (ONE SOFTWARE DEVELOPER) 
/// THE LIMITED RIGHT TO USE THIS SOFTWARE ON A SINGLE COMPUTER.
///
/// CONTACT INFORMATION:
/// contact@ebatianos.com
/// www.ebatianos.com/essentials-library
/// 
///---------------------------------------------------------------------------

using System;
using System.IO;
using System.Collections.Generic;

namespace Ebatianos
{
    public static class PathHelper
    {
        static List<String> _helperList = new List<String>();
        static Object _lock = new Object();

        public static String UnifySeparators(String path)
        {
            return path.Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar);
        }

        public static String CleanPath(String path)
        {
            String[] elements = path.Split('/', '\\');

            lock (_lock)
            {
                List<String> dirs = _helperList;
                dirs.Clear();

                foreach (var el in elements)
                {
                    if (el == "..")
                    {
                        if (dirs.Count > 0)
                        {
                            if (dirs[dirs.Count - 1] == "..")
                            {
                                dirs.Add("..");
                            }
                            else
                            {
                                dirs.RemoveAt(dirs.Count - 1);
                            }
                        }
                        else
                        {
                            dirs.Add("..");
                        }

                    }
                    else
                    {
                        dirs.Add(el);
                    }
                }

                String newPath = String.Empty;

                foreach (var dir in dirs)
                {
                    newPath = Path.Combine(newPath, dir);
                }

                return newPath;
            }
        }
    }
}
