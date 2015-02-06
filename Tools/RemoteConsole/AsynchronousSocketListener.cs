using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace RemoteConsole
{
    // State object for reading client data asynchronously
    public class StateObject
    {
        // Client  socket.
        public Socket workSocket = null;
        // Size of receive buffer.
        public const int BufferSize = 1024;
        // Receive buffer.
        public byte[] buffer = new byte[BufferSize];
        // Received data string.
        public StringBuilder sb = new StringBuilder();
    }

    public class AsynchronousSocketListener
    {
        // Thread signal.
        public ManualResetEvent allDone = new ManualResetEvent(false);

        private Action<Byte[], Int32> _onData;

        private void WriteLog(String text)
        {
            Byte[] bytes = Encoding.UTF8.GetBytes(text+"\n");
            _onData(bytes, bytes.Length);
        }

        public AsynchronousSocketListener(Action<Byte[], Int32> onData)
        {
            _onData = onData;
        }

        IPAddress GetIpv4Address(IPHostEntry entry)
        {
            foreach (var addr in entry.AddressList)
            {
                if (addr.AddressFamily == AddressFamily.InterNetwork)
                {
                    return addr;
                }
            }

            return null;
        }

        public void StartListening(Int32 port)
        {
            // Data buffer for incoming data.
            byte[] bytes = new Byte[1024];

            // Establish the local endpoint for the socket.
            // The DNS name of the computer
            // running the listener is "host.contoso.com".
            IPHostEntry ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());
            IPAddress ipAddress = GetIpv4Address(ipHostInfo);
            IPEndPoint localEndPoint = new IPEndPoint(ipAddress, port);

            byte[] addr = ipAddress.GetAddressBytes();

            WriteLog(String.Format("{{Info}}Listening started at {0} at {1}:{2}", ipHostInfo.HostName, ipAddress.ToString(), port));

            // Create a TCP/IP socket.
            Socket listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            // Bind the socket to the local endpoint and listen for incoming connections.
            try
            {
                listener.Bind(localEndPoint);
                listener.Listen(100);

                while (true)
                {
                    // Set the event to nonsignaled state.
                    allDone.Reset();

                    WriteLog("Waiting for connection...");

                    listener.BeginAccept(
                        new AsyncCallback(AcceptCallback),
                        listener);

                    // Wait until a connection is made before continuing.
                    allDone.WaitOne();
                }

            }
            catch (Exception ex)
            {
                WriteLog(ex.ToString());
            }
        }

        public void AcceptCallback(IAsyncResult ar)
        {
            // Signal the main thread to continue.
            allDone.Set();

            // Get the socket that handles the client request.
            Socket listener = (Socket)ar.AsyncState;
            Socket handler = listener.EndAccept(ar);

            // Create the state object.
            StateObject state = new StateObject();
            state.workSocket = handler;

            WriteLog(String.Format("{{Info}}Connection from {0}", handler.RemoteEndPoint.ToString()));

            handler.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0,
                new AsyncCallback(ReadCallback), state);
        }

        public void ReadCallback(IAsyncResult ar)
        {
            
            String content = String.Empty;

            // Retrieve the state object and the handler socket
            // from the asynchronous state object.
            StateObject state = (StateObject)ar.AsyncState;
            Socket handler = state.workSocket;

            try
            {
                // Read data from the client socket. 
                int bytesRead = handler.EndReceive(ar);

                if (bytesRead > 0)
                {
                    _onData.Invoke(state.buffer, bytesRead);

                    // Not all data received. Get more.
                    handler.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0,
                    new AsyncCallback(ReadCallback), state);

                }
            }
            catch
            {
                WriteLog(String.Format("{{Info}}Disconnected from {0}", handler.RemoteEndPoint.ToString()));

                try
                {
                    state.workSocket.Close();
                }
                catch
                {
                }
            }
        }
    }
}