using System;
using Sitana.Framework.Games;

namespace GameEditor
{
    public class DocTiledLayer: DocLayer
    {
        public new TiledLayer Layer{get{return _layer as TiledLayer;}}

        public override int Width { get { return Layer.Width; } }
        public override int Height { get { return Layer.Height; } }

        public DocTiledLayer(string name): base("Tiled")
        {
            Name.StringValue = name;
            _layer = new TiledLayer();
        }
    }
}

