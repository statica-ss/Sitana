using Microsoft.Xna.Framework.Graphics;
using Sitana.Framework.Cs;
using Sitana.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Sitana.Framework.Misc
{
    public interface ICacheClient
    {
        void ImageUpdated();
    }

    public class RemoteImageCache: Singleton<RemoteImageCache>
    {
        Texture2D _noImage;

        public Texture2D NoImage
        {
            get
            {
                return _noImage == null ? AdvancedDrawBatch.OnePixelWhiteTexture : _noImage;
            }

            set
            {
                _noImage = value;
            }
        }

        List<string> _toRemove = new List<string>();
        Dictionary<string, RemoteImage> _images = new Dictionary<string, RemoteImage>();

        object _cacheLock = new object();

        public Texture2D RegisterClient(Uri uri, ICacheClient client)
        {
            lock (_cacheLock)
            {
                RemoteImage image;
                if (!_images.TryGetValue(uri.ToString(), out image))
                {
                    image = new RemoteImage(uri);
                    _images.Add(uri.ToString(), image);
                }

                image.RegisterClient(client);

                return image.Image;
            }
        }

        public void RemoveClient(ICacheClient client)
        {
            lock (_cacheLock)
            {
                _toRemove.Clear();
                foreach (var img in _images)
                {
                    img.Value.RemoveClient(client);

                    if (!img.Value.HasClients)
                    {
                        _toRemove.Add(img.Key);
                        img.Value.Dispose();
                    }
                }

                foreach (var key in _toRemove)
                {
                    _images.Remove(key);
                }
                _toRemove.Clear();
            }
        }
    }

    public class RemoteImage: IDisposable
    {
        public Texture2D Image { get; private set; }
        readonly List<ICacheClient> _clients = new List<ICacheClient>();

        public bool HasClients
        {
            get
            {
                return _clients.Count > 0;
            }
        }

        public RemoteImage(Uri uri)
        {
            Image = AdvancedDrawBatch.OnePixelWhiteTexture;
            DownloadImageAsync(uri);
        }

        public void Dispose()
        {
            if (Image != AdvancedDrawBatch.OnePixelWhiteTexture && Image != RemoteImageCache.Instance.NoImage)
            {
                Image.Dispose();
                Image = null;
            }
        }

        public void RegisterClient(ICacheClient client)
        {
            if(!_clients.Contains(client))
            {
                _clients.Add(client);
            }
        }

        public void RemoveClient(ICacheClient client)
        {
            _clients.Remove(client);
        }

        void DownloadImageAsync(Uri uri)
        {
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(uri);

            request.BeginGetResponse(OnImageResponse, request);
        }

        void OnImageResponse(IAsyncResult state)
        {
            try
            {
                HttpWebRequest request = state.AsyncState as HttpWebRequest;

                WebResponse response = request.EndGetResponse(state);

                if (!response.ContentType.ToLowerInvariant().Contains("image"))
                {
                    UiTask.BeginInvoke(() =>
                    {
                        Image = RemoteImageCache.Instance.NoImage;
                        foreach (var client in _clients)
                        {
                            client.ImageUpdated();
                        }
                    });

                    response.Dispose();
                    return;
                }

                MemoryStream stream = new MemoryStream();

                Stream responseStream = response.GetResponseStream();

                responseStream.CopyTo(stream);
                response.Dispose();

                stream.Seek(0, SeekOrigin.Begin);

                UiTask.BeginInvoke(() =>
                {
                    GraphicsDevice device = AdvancedDrawBatch.OnePixelWhiteTexture.GraphicsDevice;

                    try
                    {
                        Image = Texture2D.FromStream(device, stream);
                    }
                    catch
                    {
                        Image = RemoteImageCache.Instance.NoImage;
                    }

                    foreach (var client in _clients)
                    {
                        client.ImageUpdated();
                    }
                });
            }
            catch
            {
                UiTask.BeginInvoke(() =>
                {
                    Image = RemoteImageCache.Instance.NoImage;
                    foreach (var client in _clients)
                    {
                        client.ImageUpdated();
                    }
                });
            }
        }
    }
}
