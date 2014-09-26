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
using System.Diagnostics;
using System.Reflection;
using System.Text;

namespace Ebatianos.Cs
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
