using System;
using Sitana.Framework.Games;

namespace GameEditor
{
    public class DocVectorLayer: DocLayer
    {
        public new VectorLayer Layer {get {return _layer as VectorLayer;}}

        public DocVectorLayer(string name): base("Vector")
        {
            Name.StringValue = name;
            _layer = new VectorLayer();
        }
    }
}

