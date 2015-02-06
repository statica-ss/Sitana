// SITANA - Copyright (C) The Sitana Team.
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Sitana.Framework.Graphics
{
    public class BasicShaderEffect: IShaderEffect
    {
        BasicEffect _effect;

        public BasicShaderEffect(GraphicsDevice device)
        {
            _effect = new BasicEffect(device);
        }

        Texture2D IShaderEffect.Texture
        {
            set
            {
                _effect.Texture = value;
            }
        }

        Vector3 IShaderEffect.DiffuseColor
        {
            set
            {
                _effect.DiffuseColor = value;
            }
        }

        Vector3 IShaderEffect.SpecularColor
        {
            set
            {
                _effect.SpecularColor = value;
            }
        }

        Vector3 IShaderEffect.EmissiveColor
        {
            set
            {
                _effect.EmissiveColor = value;
            }
        }

        Vector3 IShaderEffect.AmbientLightColor
        {
            set
            {
                _effect.AmbientLightColor = value;
            }
        }

        float IShaderEffect.Alpha
        {
            set
            {
                _effect.Alpha = value;
            }
        }

        float IShaderEffect.SpecularPower
        {
            set
            {
                _effect.SpecularPower = value;
            }
        }

        Boolean IShaderEffect.TextureEnabled
        {
            set
            {
                _effect.TextureEnabled = value;
            }
        }

        Boolean IShaderEffect.PreferPerPixelLighting
        {
            set
            {
                _effect.PreferPerPixelLighting = value;
            }
        }

        Boolean IShaderEffect.LightingEnabled
        {
            set
            {
                _effect.LightingEnabled = value;
            }
        }

        Boolean IShaderEffect.VertexColorEnabled
        {
            set
            {
                _effect.VertexColorEnabled = value;
            }
        }

        Matrix IShaderEffect.World
        {
            set
            {
                _effect.World = value;
            }
        }

        Matrix IShaderEffect.View
        {
            set
            {
                _effect.View = value;
            }
        }

        Matrix IShaderEffect.Projection
        {
            set
            {
                _effect.Projection = value;
            }
        }

        void IShaderEffect.Apply(int pass)
        {
            _effect.CurrentTechnique.Passes[pass].Apply();
        }

        void IShaderEffect.EnableDefaultLighting()
        {
            _effect.EnableDefaultLighting();
        }
    }
}

