using Microsoft.Xna.Framework;
using TheArtOfDev.HtmlRenderer.Adapters;
using TheArtOfDev.HtmlRenderer.Adapters.Entities;

namespace Sitana.Framework.Ui.Views.HtmlRendererImpl
{
    public class HtmlViewBrush : RBrush
    {
        readonly Color _color;

        public Color Color
        {
            get
            {
                return _color;
            }
        }

        public HtmlViewBrush(RColor color)
        {
            _color = color.ToXnaColor();
        }

        public override void Dispose()
        {
        }
    }
}
