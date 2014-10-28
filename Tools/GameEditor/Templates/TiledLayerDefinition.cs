using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sitana.Framework.Games;
using Sitana.Framework.Xml;

namespace GameEditor
{
    class TiledLayerDefinition: LayerDefinition
    {
        public int Width;
        public int Height;

        public bool TiledWidth;
        public bool TiledHeight;

        public string Tileset;

        public static TiledLayerDefinition FromNode(XNode node)
        {
            XNodeAttributes attr = new XNodeAttributes(node);

            return new TiledLayerDefinition()
            {
                Selected = attr.AsBoolean("Selected"),
                Name = attr.AsString("Name", "Tiled Layer"),
                HorizontalSpeed = attr.AsInt32("HorizontalSpeed", 100),
                VerticalSpeed = attr.AsInt32("VerticalSpeed", 100),
                Width = attr.AsInt32("Width", 128),
                Height = attr.AsInt32("Height", 128),
                TiledWidth = attr.AsBoolean("TiledWidth", false),
                TiledHeight = attr.AsBoolean("TiledHeight", false),
                Tileset = attr.AsString("Tileset")
            };
        }

        protected override DocLayer Create()
        {
            return new DocTiledLayer(Name);
        }

        protected override void FillWithData(DocLayer layer)
        {
            base.FillWithData(layer);

            TiledLayer tl = layer.Layer as TiledLayer;

            tl.Resize(Width, Height);
            
            tl.TiledWidth = TiledWidth;
            tl.TiledHeight = TiledHeight;

            tl.Tileset = Tileset;
        }
    }
}
