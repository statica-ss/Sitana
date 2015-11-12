using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System
{
    public static class Console
    {
        public static void WriteLine(string format, params object[] args)
        {
            Debug.WriteLine(format, args);
        }

        public static void WriteLine()
        {
            Debug.WriteLine(string.Empty);
        }

        public static void WriteLine(object obj)
        {
            Debug.WriteLine(obj);
        }
    }
}
