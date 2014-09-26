﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RemoteConsole
{
    class Program
    {
        static Object _lock = new Object();

        static void Main(string[] args)
        {
            AsynchronousSocketListener listener = new AsynchronousSocketListener(WriteConsole);
            listener.StartListening(3423);
        }

        static void WriteConsole(Byte[] data, Int32 len)
        {
            String text = Encoding.UTF8.GetString(data, 0, len);

            String[] lines = text.Split('\n');

            foreach (var line in lines)
            {
                WriteLine(line);
            }
        }

        static void WriteLine(String text)
        {
            ConsoleColor color = ConsoleColor.Black;

            if (text.StartsWith("{"))
            {
                Int32 end = text.IndexOf('}');

                if (end > 0)
                {
                    String colorStr = text.Substring(1, end - 1);
                    text = text.Substring(end + 1);

                    color = ParseColor(colorStr);
                }
            }

            WriteLine(text, color);
        }

        static void WriteLine(String text, ConsoleColor color)
        {
            lock (_lock)
            {
                Console.ForegroundColor = color;
                Console.BackgroundColor = ConsoleColor.White;
                Console.WriteLine(text);
                Console.ResetColor();
            }
        }

        static ConsoleColor ParseColor(String text)
        {
            switch (text)
            {
                case "Red":
                    return ConsoleColor.Red;

                case "Green":
                    return ConsoleColor.Green;

                case "Orange":
                    return ConsoleColor.DarkYellow;

                case "Blue":
                    return ConsoleColor.Blue;

                case "Purple":
                    return ConsoleColor.DarkMagenta;
            }

            return ConsoleColor.Black;
        }
    }
}
