using System;
using System.IO;
using System.Threading;

namespace Sitana.Framework.DataTransfer
{
    public class ReadContentHelper
    {
        public interface IProgressBar
        {
            void Update(int progress);
            Boolean Cancel { get; }
        }

        public class CancelException : Exception
        {
            public CancelException() { }
            public CancelException(string message): base(message) { }
        }

        public class TimeoutException : Exception
        {
            public TimeoutException() { }
            public TimeoutException(string message) : base(message) { }
        }

        public static void ReadWithTimeout(Stream inputStream, Stream outputStream, int contentLength, int timeout, IProgressBar progressBar = null)
        {
            int bufferSize = Math.Min(1024, contentLength);

            int bytesReadTotal = 0;
            byte[] buffer = new byte[bufferSize];
            int maxNumberOfTries = timeout * 40;
            int numberOfTries = 0;

            while (true)
            {
                if (progressBar != null)
                {
                    if (progressBar.Cancel)
                    {
                        throw new CancelException();
                    }
                }

                int bytesToRead = Math.Min(bufferSize, contentLength - bytesReadTotal);
                int bytesRead = inputStream.Read(buffer, 0, bytesToRead);

                if (bytesRead == 0)
                {
                    if (numberOfTries > maxNumberOfTries)
                    {
                        throw new TimeoutException();
                    }

                    ++numberOfTries;
                    Thread.Sleep(25);
                }
                else
                {
                    numberOfTries = 0;

                    outputStream.Write(buffer, 0, bytesRead);
                    bytesReadTotal += bytesRead;

                    if (progressBar != null)
                    {
                        progressBar.Update(bytesReadTotal);
                    }
                }

                if (contentLength == bytesReadTotal)
                {
                    break;
                }
            }
        }
    }
}
