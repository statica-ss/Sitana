using System;
using Sitana.Framework.Games;

namespace GameEditor
{
    public class DocVectorLayer: DocLayer
    {
        public DocVectorLayer(string name): base("Vector")
        {
            Name.StringValue = name;
            _layer = new VectorLayer();
        }
    }
}

