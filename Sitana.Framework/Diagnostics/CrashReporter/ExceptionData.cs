using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Sitana.Framework.Diagnostics
{
    public class ExceptionData
    {
        public readonly string AppVersion = null;
        public readonly string OsVersion = null;
        public readonly long Time = 0;
        public readonly string Message = null;
        public readonly string StackTrace = null;
        public readonly string Source = null;
        public readonly string Type = null;

        public ExceptionData(BinaryReader reader)
        {
            Time = reader.ReadInt64();
            AppVersion = reader.ReadString();
            OsVersion = reader.ReadString();
            Message = reader.ReadString();
            StackTrace = reader.ReadString();
            Source = reader.ReadString();
            Type = reader.ReadString();

			StackTrace = StackTrace.Replace('\\', '/').Replace('\n', ' ').Replace('\r', ' ');
        }

        public ExceptionData(string appVersion, DateTime time, object exception)
        {
            Time = time.ToBinary();
            AppVersion = appVersion;
            OsVersion = Platform.OsVersion;

            if(exception is Exception)
            {   
                Exception exc = exception as Exception;

                while (exc is TargetInvocationException)
                {
                    exc = exc.InnerException;
                }

                Type = exc.GetType().Name;
                Message = exc.Message;
                StackTrace = exc.StackTrace.Trim().Replace('\\', '/');
                Source = exc.Source;

                MethodBase site = exc.TargetSite;
                
                if(site == null)
                {
                    Source = string.Empty;
                }
                else
                {
                    int lineNumber = GetLineNumber(StackTrace);

                    if(lineNumber > 0)
                    {
                        Source = string.Format("{0}.{1}:{2}", site.DeclaringType.FullName, site.Name, lineNumber);
                    }
                    else
                    {
                        Source = string.Format("{0}.{1}", site.DeclaringType.FullName, site.Name );
                    }
                }

				Console.WriteLine(exc.ToString());
            }
            else
            {
                StackTrace = exception.ToString();
                Message = string.Empty;
                Source = string.Empty;
                Type = exception.GetType().Name;
            }
        }

        int GetLineNumber(string trace)
        {
            var lineNumber = 0;
            const string lineSearch = ".cs:";
            var index = trace.LastIndexOf(lineSearch);

            if (index != -1)
            {
                while (index < trace.Length && !char.IsDigit(trace[index]))
                {
                    index++;
                }

                var lineNumberText = trace.Substring(index);

                int.TryParse(lineNumberText, out lineNumber);
            }
            return lineNumber;
        }

        public void Serialize(BinaryWriter writer)
        {
            writer.Write(Time);
            writer.Write(AppVersion);
            writer.Write(OsVersion);
            writer.Write(Message);
            writer.Write(StackTrace);
            writer.Write(Source);
            writer.Write(Type);
        }
    }
}
