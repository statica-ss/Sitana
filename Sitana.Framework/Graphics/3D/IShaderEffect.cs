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
    public interface IShaderEffect
    {
        Texture2D Texture{set;}
        Vector3 DiffuseColor{set;}
        Vector3 SpecularColor{set;}
        Vector3 EmissiveColor{set;}
        Vector3 AmbientLightColor{set;}
        Single Alpha {set;}
        Single SpecularPower{set;}
        Boolean TextureEnabled {set;}
        Boolean PreferPerPixelLighting{ set;}
        Boolean LightingEnabled {set;}
        Boolean VertexColorEnabled {set;}

        Matrix World { set;}
        Matrix View { set; }
        Matrix Projection { set;}

        void Apply(Int32 pass);
        void EnableDefaultLighting();
    }
}

