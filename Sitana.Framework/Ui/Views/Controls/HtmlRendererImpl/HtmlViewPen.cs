using Microsoft.Xna.Framework;
using System;
using TheArtOfDev.HtmlRenderer.Adapters;
using TheArtOfDev.HtmlRenderer.Adapters.Entities;

namespace Sitana.Framework.Ui.Views.HtmlRendererImpl
{
    public class HtmlViewPen : RPen
    {
        public readonly Color Color;

        public override RDashStyle DashStyle
        {
            set
            {
                
            }
        }

        public override double Width { get; set; }

        public HtmlViewPen(RColor color)
        {
            Color = color.ToXnaColor();
        }
    }
}
