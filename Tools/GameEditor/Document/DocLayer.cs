using System;
using Sitana.Framework.Cs;
using Sitana.Framework.Games;

namespace GameEditor
{
    public class DocLayer
    {
		public readonly SharedString Name = new SharedString();

		public readonly string Type;

		public readonly SharedValue<bool> Selected = new SharedValue<bool>();

        protected Layer _layer = null;

        public Layer Layer {get{return _layer;}}

        public DocLayer(string type)
        {
            Type = type;
        }
    }
}

