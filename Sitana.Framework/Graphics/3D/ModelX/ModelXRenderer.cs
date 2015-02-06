// SITANA - Copyright (C) The Sitana Team.
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using Microsoft.Xna.Framework.Graphics;
using Sitana.Framework.Graphics.Model;
using Microsoft.Xna.Framework;

namespace Sitana.Framework.Graphics
{
    static class ModelXRenderer
    {
        public static void Render(ModelX model, IShaderEffect effect, Vector3 ambientColor, int materialsSet)
        {
            GraphicsDevice device = model.VertexBuffer.GraphicsDevice;

            Material[] materials = model.Materials(materialsSet);

            device.SetVertexBuffer(model.VertexBuffer);

            for (int steps = 0; steps < 2; ++steps)
            {
                for (int idx = 0; idx < model.Subsets.Length; ++idx)
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

                    int startIndex = 0;

                    while (startIndex < subset.Indices.Length)
                    {
                        int primitiveCount = Math.Min((subset.Indices.Length - startIndex) / 3, 30000);

                        device.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, model.Vertices.Length, startIndex, primitiveCount);
                        startIndex += primitiveCount * 3;
                    }
                }
            }
        }
    }
}

