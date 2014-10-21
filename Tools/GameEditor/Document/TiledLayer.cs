using System;

namespace GameEditor
{
    public class TilesetLayer: Layer
    {
        public TilesetLayer(string name): base("Tiled")
        {
            Name.StringValue = name;
        }
    }
}

