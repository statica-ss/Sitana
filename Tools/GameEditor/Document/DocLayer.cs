using System;
using Sitana.Framework.Cs;
using Sitana.Framework.Games;
using System.IO;

namespace GameEditor
{
    public abstract class DocLayer
    {
		public readonly SharedString Name = new SharedString();

		public readonly string Type;

		public readonly SharedValue<bool> Selected = new SharedValue<bool>();

        protected Layer _layer = null;

        public Layer Layer {get{return _layer;}}

        public bool PropertiesDirty
        {
            get
            {
                bool dirty = _propertiesDirty;
                _propertiesDirty = false;
                return dirty;
            }
        }

        bool _propertiesDirty = false;

        public abstract int Width {get;}
        public abstract int Height {get;}

        public void InvalidateProperties()
        {
            _propertiesDirty = true;
        }

        protected DocLayer(string type)
        {
            Type = type;
        }

        public void Serialize(BinaryWriter writer)
        {
            Layer.Name = Name.StringValue;
            Layer.Serialize(writer);
        }

        public void Deserialize(BinaryReader reader)
        {
            Layer.Deserialize(reader);
            Name.StringValue = Layer.Name;
        }
    }
}

