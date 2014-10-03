// SITANA - Copyright (C) The Sitana Team.
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using System.Diagnostics;
using System.Reflection;
using System.Text;

namespace Sitana.Framework.Cs
{
    public abstract class ToolkitClass{}

    public static class CsToolkit
    {
        [Conditional("ETRACE")]
        public static void TestAndThrow(Boolean condition)
        {
            if (!condition)
            {
                throw new Exception("Condition is false.");
            }
        }

        [Conditional("ETRACE")]
        public static void TestAndThrow(Boolean condition, String message)
        {
            if (!condition)
            {
                throw new Exception(message);
            }
        }

        public static void ConsolePrint(String category, String format, params object[] args)
        {
            Console.Write("{0}: ", category);
            Console.Write(format, args);
        }

        public static void ConsolePrintLine(String category, String format, params object[] args)
        {
            ConsolePrint(category, format, args);
            Console.WriteLine();
        }

        [Conditional("ETRACE")]
        public static void TracePrint(String category, String format, params object[] args)
        {
            ConsolePrint(category, format, args);
        }

        [Conditional("ETRACE")]
        public static void TracePrintLine(String category, String format, params object[] args)
        {
            ConsolePrintLine(category, format, args);
        }

        [Conditional("ELOGGING")]
        public static void WriteLog(MethodBase method, String format, params object[] args)
        {
            StringBuilder builder = new StringBuilder();

            var parameters = method.GetParameters();

            builder.AppendLine(DateTime.Now.ToString("\n[yyyy-MM-dd hh:mm:ss]"));

            builder.AppendFormat("{0}.{1}(", method.DeclaringType.Name, method.Name);

            for (Int32 idx = 0; idx < parameters.Length; ++idx)
            {
                if ( idx > 0 )
                {
                    builder.Append(", ");
                }
                builder.AppendFormat("{1} {0}", parameters[idx].Name, parameters[idx].ParameterType.Name);
            }

            builder.AppendLine(")");
            builder.AppendFormat(format, args);

            Console.WriteLine(builder.ToString());
        }
    }
}
