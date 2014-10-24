using System;
using Sitana.Framework.Games;

namespace GameEditor
{
    public class DocTiledLayer: DocLayer
    {
        public new TiledLayer Layer{get{return _layer as TiledLayer;}}

        public DocTiledLayer(string name): base("Tiled")
        {
            Name.StringValue = name;
            _layer = new TiledLayer();
        }
    }
}

