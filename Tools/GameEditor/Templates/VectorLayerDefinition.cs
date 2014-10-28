using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sitana.Framework.Xml;

namespace GameEditor
{
    class VectorLayerDefinition: LayerDefinition
    {
        public static VectorLayerDefinition FromNode(XNode node)
        {
            XNodeAttributes attr = new XNodeAttributes(node);

            return new VectorLayerDefinition()
            {
                Selected = attr.AsBoolean("Selected"),
                Name = attr.AsString("Name", "Vector Layer"),
                HorizontalSpeed = attr.AsInt32("HorizontalSpeed", 100),
                VerticalSpeed = attr.AsInt32("VerticalSpeed", 100)
            };
        }

        protected override DocLayer Create()
        {
            return new DocVectorLayer(Name);
        }
    }
}
