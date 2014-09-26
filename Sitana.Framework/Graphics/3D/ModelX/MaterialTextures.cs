// SITANA - Copyright (C) The Sitana Team.
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using Microsoft.Xna.Framework.Graphics;

namespace Sitana.Framework.Graphics.Model
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
