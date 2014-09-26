using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sitana.Framework.Cs;
using System.Net.Sockets;

namespace Sitana.Framework.Diagnostics
{
    class RemoteConsoleClient: Singleton<RemoteConsoleClient>
    {
        Socket _socket = null;

        Queue<Byte[]> _toSend = new Queue<Byte[]>();

        Object _consoleLock = new Object();
        Boolean _sending = false;

        String _ipAddress;
        Int32 _port;

        Boolean _initialized = false;

        public Boolean ConsoleAttached
        {
            get
            {
                return _initialized;
            }
        }

        public void Initialize(String ipAddress, Int32 port)
        {
            _ipAddress = ipAddress;
            _port = port;

            _initialized = true;

            Reconnect();
        }

        private void Reconnect()
        {
            _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IAsyncResult result = _socket.BeginConnect(_ipAddress, _port, null, null);

            bool success = result.AsyncWaitHandle.WaitOne(TimeSpan.FromSeconds(10));

            if (success)
            {
                try
                {
                    _socket.EndConnect(result);
                }
                catch(Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                    return;
                }

                Send();
            }
        }

        public void WriteText(String text)
        {
            if (!_initialized)
            {
                return;
            }

            Byte[] data = Encoding.UTF8.GetBytes(text);
            
            lock (_consoleLock)
            {
                _toSend.Enqueue(data);
            }

            Send();
        }

        private void Send()
        {
            lock(_consoleLock)
            {
                if ( _sending || _toSend.Count == 0)
                {
                    return;
                }
            }

            if (!_socket.Connected)
            {
                Reconnect();
                return;
            }
            
            Byte[] bytes = _toSend.Dequeue();

            IAsyncResult result = _socket.BeginSend(bytes, 0, bytes.Length, SocketFlags.None, null, null);
            bool success = result.AsyncWaitHandle.WaitOne();

            if (!success)
            {

            }
            else
            {
                Send();
            }
        }
    }
}
