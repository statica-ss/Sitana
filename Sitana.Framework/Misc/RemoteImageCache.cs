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
            if (Image != AdvancedDrawBatch.OnePixelWhiteTexture)
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
            WebClient client = new WebClient();

            client.DownloadDataCompleted += client_DownloadDataCompleted;
            client.DownloadDataAsync(uri, uri);
        }

        void client_DownloadDataCompleted(object sender, DownloadDataCompletedEventArgs args)
        {
            if (args.Error != null)
            {
                Console.WriteLine(args.Error);
            }
            else if (!args.Cancelled)
            {
                Uri uri = args.UserState as Uri;
                string path = uri.ToString();

                UiTask.BeginInvoke(() =>
                {
                    GraphicsDevice device = AdvancedDrawBatch.OnePixelWhiteTexture.GraphicsDevice;

                    MemoryStream stream = new MemoryStream(args.Result);
                    Texture2D texture = Texture2D.FromStream(device, stream);

                    Image = texture;

                    foreach (var client in _clients)
                    {
                        client.ImageUpdated();
                    }
                });
            }
        }

        

        
    }
}
