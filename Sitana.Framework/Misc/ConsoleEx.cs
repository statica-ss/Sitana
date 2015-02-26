using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

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

			#if DEBUG
			switch (type)
			{
			case Error:
				Debug.WriteLine(text, "AppError");
				break;

			case Warning:
				Debug.WriteLine(text, "AppWarning");
				break;

			case Info:
				Debug.WriteLine(text, "AppInfo");
				break;

			default:
				Console.WriteLine(text);
				break;
			}
			#else
            	Console.WriteLine(text);
			#endif

            if (RemoteConsoleClient.Instance.ConsoleAttached)
            {
                text = type + text.Replace("\r", "").Replace("\n", "\n" + type);
                RemoteConsoleClient.Instance.WriteText(text + "\n");
            }
        }
    }
}
