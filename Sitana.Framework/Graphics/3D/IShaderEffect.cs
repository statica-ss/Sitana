// SITANA - Copyright (C) The Sitana Team.
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Sitana.Framework.Graphics
{
    public interface IShaderEffect
    {
        Texture2D Texture{set;}
        Vector3 DiffuseColor{set;}
        Vector3 SpecularColor{set;}
        Vector3 EmissiveColor{set;}
        Vector3 AmbientLightColor{set;}
        float Alpha {set;}
        float SpecularPower{set;}
        Boolean TextureEnabled {set;}
        Boolean PreferPerPixelLighting{ set;}
        Boolean LightingEnabled {set;}
        Boolean VertexColorEnabled {set;}

        Matrix World { set;}
        Matrix View { set; }
        Matrix Projection { set;}

        void Apply(int pass);
        void EnableDefaultLighting();
    }
}

