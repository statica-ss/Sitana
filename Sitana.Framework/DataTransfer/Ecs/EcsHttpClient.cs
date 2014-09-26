using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Ebatianos.DataTransfer
{
    public class EcsHttpClient
    {
        public delegate void ResponseHandler(EcsResponse response);

        private string _url;

        private Type[] _dataTypes;

        private volatile HttpWebRequest _httpRequest;
        private Queue<EcsRequest> _requestsToSend = new Queue<EcsRequest>();

        private object _lock = new object();

        public EcsHttpClient(string url, Type[] dataTypes)
        {
            List<Type> types = new List<Type>();

            if (dataTypes != null)
            {
                foreach (var type in dataTypes)
                {
                    types.Add(type);
                }
            }

            types.Add(typeof(EcsResponse));

            _dataTypes = types.ToArray();

            _url = url;
        }

        public void Request(EcsRequest request, ResponseHandler handler)
        {
            request.Handler = handler;

            lock (_lock)
            {
                _requestsToSend.Enqueue(request);
            }

            
            SendOne();
        }

        private void SendOne()
        {
            EcsRequest request = null;

            lock (_lock)
            {
                if (_requestsToSend.Count > 0 && _httpRequest == null)
                {
                    request = _requestsToSend.Dequeue();
                }
            }

            if (request != null)
            {
                SendRequest(request);
            }
        }

        private void SendRequest(EcsRequest request)
        {
            lock (_lock)
            {
                byte[] data = EcsProtocol.ToBytes(request, Encoding.UTF8);

                _httpRequest = (HttpWebRequest)WebRequest.Create(_url);

                _httpRequest.Method = "POST";
                _httpRequest.ContentType = "application/octet-stream";

                byte[] postData = data;// Encoding.UTF8.GetBytes(data.UrlEncode());

                _httpRequest.ContentLength = postData.Length;

                using (Stream stream = _httpRequest.GetRequestStream())
                {
                    stream.Write(postData, 0, postData.Length);
                    stream.Flush();
                    stream.Close();
                }

                try
                {
                    _httpRequest.BeginGetResponse(new AsyncCallback(OnEndGetResponse), request);
                }
                catch(WebException ex)
                {
                    ResponseHandler handler = request.Handler;
                    handler(new EcsResponse() { Status = -1, StateObject = request.StateObject, Message = ex.ToString() });

                    _httpRequest = null;
                }
            }
        }

        private void OnEndGetResponse(IAsyncResult result)
        {
            EcsRequest request = (EcsRequest)result.AsyncState;

            try
            {
                using (WebResponse response = _httpRequest.EndGetResponse(result))
                {

                    int length = (int)response.ContentLength;

                    MemoryStream stream = new MemoryStream(length);

                    ReadContentHelper.ReadWithTimeout(response.GetResponseStream(), stream, length, 2);
                    byte[] data = stream.GetBuffer();

                    EcsResponse responseObject = EcsProtocol.GetData<EcsResponse>(data, 0, length, Encoding.UTF8, _dataTypes);

                    responseObject.StateObject = request.StateObject;

                    ResponseHandler handler = request.Handler;
                    handler(responseObject);
                }
            }
            catch(Exception ex)
            {
                ResponseHandler handler = request.Handler;
                handler(new EcsResponse() { Status = -1, StateObject = request.StateObject, Message = ex.ToString() });
            }

            lock (_lock)
            {
                _httpRequest = null;
            }

            Task.Factory.StartNew(SendOne);
        }
    }
}
