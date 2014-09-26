using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;

namespace Ebatianos.Graphics.Model
{
    public class MaterialTextures
    {
        public readonly Texture2D Diffuse;
        public readonly Texture2D Normal;

        public MaterialTextures(Texture2D diffuse): this(diffuse, null){}

        public MaterialTextures(Texture2D diffuse, Texture2D normal)
        {
            Diffuse = diffuse;
            Normal = normal;
        }

        public void Dispose()
        {
            if ( Diffuse != null )
            {
                Diffuse.Dispose();
            }

            if (Normal != null)
            {
                Normal.Dispose();
            }
        }
    }
}
