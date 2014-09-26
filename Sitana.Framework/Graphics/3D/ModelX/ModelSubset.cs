// SITANA - Copyright (C) The Sitana Team.
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using System.IO;
using Microsoft.Xna.Framework.Graphics;

namespace Sitana.Framework.Graphics.Model
{
    public class ModelSubset
    {
        public readonly Int16[] Indices;
        public readonly Int32   Material;

        IndexBuffer _indexBuffer = null;

        public IndexBuffer IndexBuffer
        {
            get
            {
                if (_indexBuffer == null)
                {
                    throw new Exception("Index buffer not created.");
                }
                return _indexBuffer;
            }
        }

        internal void PrepareForRender(GraphicsDevice device)
        {
            if ( _indexBuffer == null )
            {
                IndexBuffer buffer = new IndexBuffer(device, IndexElementSize.SixteenBits, Indices.Length, BufferUsage.WriteOnly);
                buffer.SetData(Indices);

                _indexBuffer = buffer;
            }
        }

        public void DisposeBuffers()
        {
            if (_indexBuffer != null)
            {
                _indexBuffer.Dispose();
                _indexBuffer = null;
            }
        }

        public ModelSubset(Int32 material, Int16[] indices)
        {
            Indices = indices;
            Material = material;
        }

        public void Serialize(BinaryWriter writer)
        {
            writer.Write(Material);
            writer.Write(Indices.Length);

            foreach (Int16 i in Indices)
            {
                writer.Write(i);
            }
        }

        public static ModelSubset Deserialize(BinaryReader reader)
        {
            Int32 material = reader.ReadInt32();

            Int16[] indices = new Int16[reader.ReadInt32()];

            for (Int32 idx = 0; idx < indices.Length; ++idx)
            {
                indices[idx] = reader.ReadInt16();
            }

            return new ModelSubset(material, indices);
        }
    }
}
