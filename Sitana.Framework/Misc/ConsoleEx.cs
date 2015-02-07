using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sitana.Framework.Diagnostics
{
    public static class ConsoleEx
    {
        public const String Error = "{Error}";
        public const String Warning = "{Warning}";
        public const String Info = "{Info}";
        public const String Asterisk = "{Asterisk}";

        public static void EnableRemoteConsole(String address, int port)
        {
            RemoteConsoleClient.Instance.Initialize(address, port);
        }

        public static void WriteLine(String format, params Object[] args)
        {
            WriteLine(String.Empty, format, args);
        }

        public static void WriteLine(String type, String format, params Object[] args)
        {
            String text = String.Format(format, args);

            Console.WriteLine(text);

            if (RemoteConsoleClient.Instance.ConsoleAttached)
            {
                text = type + text.Replace("\r", "").Replace("\n", "\n" + type);
                RemoteConsoleClient.Instance.WriteText(text + "\n");
            }
        }
    }
}
