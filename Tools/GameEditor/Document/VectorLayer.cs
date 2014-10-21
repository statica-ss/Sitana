using System;

namespace GameEditor
{
    public class VectorLayer: Layer
    {
        public VectorLayer(string name): base("Vector")
        {
            Name.StringValue = name;
        }
    }
}

