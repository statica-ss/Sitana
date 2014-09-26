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
using Ebatianos.Graphics.Model;
using Microsoft.Xna.Framework;

namespace Ebatianos.Graphics
{
    static class ModelXRenderer
    {
        public static void Render(ModelX model, IShaderEffect effect, Vector3 ambientColor, Int32 materialsSet)
        {
            GraphicsDevice device = model.VertexBuffer.GraphicsDevice;

            Material[] materials = model.Materials(materialsSet);

            device.SetVertexBuffer(model.VertexBuffer);

            for (Int32 steps = 0; steps < 2; ++steps)
            {
                for (Int32 idx = 0; idx < model.Subsets.Length; ++idx)
                {
                    var subset = model.Subsets[idx];

                    Material material = materials[subset.Material];

                    if (steps == 0)
                    {
                        if (material.Opacity < 1)
                        {
                            continue;
                        }
                    }

                    if (steps == 1)
                    {
                        if (material.Opacity == 1)
                        {
                            continue;
                        }
                    }

                    device.Indices = subset.IndexBuffer;

                    //// Draw the triangle.
                    effect.DiffuseColor = material.Diffuse;
                    effect.AmbientLightColor = ambientColor * material.Ambient;
                    effect.SpecularColor = material.Specular;
                    effect.EmissiveColor = material.Emissive;
                    effect.Alpha = material.Opacity;

					MaterialTextures textures = material.Textures;

                    effect.SpecularPower = material.SpecularExponent;

                    Texture2D texture = textures != null ? textures.Diffuse : null;

                    effect.Texture = texture;
                    effect.TextureEnabled = texture != null;

                    effect.Apply(0);

                    Int32 startIndex = 0;

                    while (startIndex < subset.Indices.Length)
                    {
                        Int32 primitiveCount = Math.Min((subset.Indices.Length - startIndex) / 3, 30000);

                        device.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, model.Vertices.Length, startIndex, primitiveCount);
                        startIndex += primitiveCount * 3;
                    }
                }
            }
        }
    }
}

