using Microsoft.Xna.Framework.Graphics;
using Sitana.Framework.Content;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheArtOfDev.HtmlRenderer.Adapters;

namespace Sitana.Framework.Ui.Views.HtmlRendererImpl
{
    public class HtmlViewImage : RImage
    {
        Texture2D _texture;

        public Texture2D Texture
        {
            get
            {
                return _texture;
            }
        }

        public HtmlViewImage(Texture2D image)
        {
            _texture = image;
        }

        public HtmlViewImage(Stream stream)
        {
            IGraphicsDeviceService deviceService = ContentLoader.Current.GetService<IGraphicsDeviceService>();
            GraphicsDevice device = deviceService.GraphicsDevice;

            _texture = Texture2D.FromStream(device, stream);
        }

        public override double Height
        {
            get
            {
                return Texture.Height;
            }
        }

        

        public override double Width
        {
            get
            {
                return Texture.Width;
            }
        }

        public override void Dispose()
        {
            if (_texture != null)
            {
                _texture.Dispose();
                _texture = null;
            }
        }
    }
}
