using System;
using Sitana.Framework.Games;

namespace GameEditor
{
    public class DocTiledLayer: DocLayer
    {

        public DocTiledLayer(string name): base("Tiled")
        {
            Name.StringValue = name;
            _layer = new TiledLayer();
        }
    }
}

