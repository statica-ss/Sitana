// /// This file is a part of the EBATIANOS.ESSENTIALS class library.
// /// (c)2013-2014 EBATIANO'S a.k.a. Sebastian Sejud. All rights reserved.
// ///
// /// THIS SOURCE FILE IS THE PROPERTY OF EBATIANO'S A.K.A. SEBASTIAN SEJUD 
// /// AND IS NOT TO BE RE-DISTRIBUTED BY ANY MEANS WHATSOEVER WITHOUT 
// /// THE EXPRESSED WRITTEN CONSENT OF EBATIANO'S A.K.A. SEBASTIAN SEJUD.
// ///
// /// THIS SOURCE CODE CAN ONLY BE USED UNDER THE TERMS AND CONDITIONS OUTLINED
// /// IN THE EBATIANOS.ESSENTIALS LICENSE AGREEMENT. 
// /// EBATIANO'S A.K.A. SEBASTIAN SEJUD GRANTS TO YOU (ONE SOFTWARE DEVELOPER) 
// /// THE LIMITED RIGHT TO USE THIS SOFTWARE ON A SINGLE COMPUTER.
// ///
// /// CONTACT INFORMATION:
// /// contact@ebatianos.com
// /// www.ebatianos.com/essentials-library
// /// 
// ///---------------------------------------------------------------------------
//
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

        Single IShaderEffect.Alpha
        {
            set
            {
                _effect.Alpha = value;
            }
        }

        Single IShaderEffect.SpecularPower
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

        void IShaderEffect.Apply(Int32 pass)
        {
            _effect.CurrentTechnique.Passes[pass].Apply();
        }

        void IShaderEffect.EnableDefaultLighting()
        {
            _effect.EnableDefaultLighting();
        }
    }
}

