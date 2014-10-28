using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace GameEditor
{
    public abstract class LayerDefinition
    {
        public string Name;
        public int HorizontalSpeed;
        public int VerticalSpeed;
        public bool Selected;

        public DocLayer Generate()
        {
            DocLayer layer = Create();
            FillWithData(layer);

            return layer;
        }

        protected abstract DocLayer Create();

        protected virtual void FillWithData(DocLayer layer)
        {
            layer.Layer.ScrollSpeed = new Vector2((float)HorizontalSpeed / 100f, (float)VerticalSpeed / 100f);
        }
    }
}
