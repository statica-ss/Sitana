// SITANA - Copyright (C) The Sitana Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.using System;
using System.IO;
using System.Collections.Generic;
using System;

namespace Sitana.Framework
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
