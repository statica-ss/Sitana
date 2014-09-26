using System;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace Sitana.Framework.Graphics.Model
{
    public class ModelX
    {
        const Int32 Version = 2;

        VertexBuffer _vertexBuffer = null;

        private List<Material[]> _materials = new List<Material[]>();

        public readonly ModelSubset[] Subsets;
        public readonly VertexPositionNormalTexture[] Vertices;

        public readonly Vector3 BoundA;
        public readonly Vector3 BoundB;

        public Vector3 Up;

        public GraphicsDevice GraphicsDevice { get; private set; }

        public List<Material[]> MaterialsSets
        {
            get
            {
                return _materials;
            }
        }

        public Vector3 Size
        {
            get
            {
                return BoundB - BoundA;
            }
        }

        public Material[] Materials(Int32 set)
        {
            return _materials[set];
        }

        public Int32 MaterialsSetsCount
        {
            get
            {
                return _materials.Count;
            }
        }

        public VertexBuffer VertexBuffer
        {
            get
            {
                if ( _vertexBuffer == null )
                {
                    throw new Exception("Vertex buffer not created.");
                }
                return _vertexBuffer;
            }

        }

        public void PrepareForRender(GraphicsDevice device)
        {
            if (GraphicsDevice != device)
            {
                DisposeBuffers();
            }

            if ( _vertexBuffer == null )
            {
                GraphicsDevice = device;

                VertexBuffer buffer = new VertexBuffer(device, typeof(VertexPositionNormalTexture), Vertices.Length, BufferUsage.WriteOnly);
                buffer.SetData(Vertices);

                _vertexBuffer = buffer;

                device.DeviceReset += OnDeviceReset;
            }

            foreach (var subset in Subsets)
            {
                subset.PrepareForRender(device);
            }
        }

        private void OnDeviceReset(Object sender, EventArgs args)
        {
            DisposeBuffers();
            PrepareForRender(GraphicsDevice);
        }

        public void DisposeBuffers()
        {
            if (_vertexBuffer != null)
            {
                _vertexBuffer.GraphicsDevice.DeviceReset -= OnDeviceReset;
                _vertexBuffer.Dispose();
                _vertexBuffer = null;
            }

            foreach (var subset in Subsets)
            {
                subset.DisposeBuffers();
            }
        }

        public ModelX(List<Material[]> materialsSets, ModelSubset[] subsets, VertexPositionNormalTexture[] vertices)
        {
            Up = Vector3.Up;
            Subsets = subsets;
            Vertices = vertices;
            _materials = materialsSets;

            Vector3 min = vertices[0].Position;
            Vector3 max = vertices[0].Position;

            foreach (var vert in vertices)
            {
                min.X = Math.Min(vert.Position.X, min.X);
                min.Y = Math.Min(vert.Position.Y, min.Y);
                min.Z = Math.Min(vert.Position.Z, min.Z);

                max.X = Math.Max(vert.Position.X, max.X);
                max.Y = Math.Max(vert.Position.Y, max.Y);
                max.Z = Math.Max(vert.Position.Z, max.Z);
            }

            BoundA = min;
            BoundB = max;
        }

        public static ModelX Load(Stream stream)
        {
            using (BinaryReader reader = new BinaryReader(stream))
            {
                return Deserialize(reader);
            }
        }

        public void Save(Stream stream)
        {
            using (BinaryWriter writer = new BinaryWriter(stream))
            {
                Serialize(writer);
            }
        }

        private static ModelX Deserialize(BinaryReader reader)
        {
            if (reader.ReadChar() != 'E' || reader.ReadChar() != 'M' || reader.ReadChar() != 'X')
            {
                throw new InvalidDataException("Invalid file header. This is not Ebatiano's ModelX file.");
            }

            Int32 version = reader.ReadInt32();

            switch (version)
            {
                case 1:
                    return ReadVersion1(reader);

                case 2:
                    return ReadVersion2(reader);
            }

            return null;
        }

        private static ModelX ReadVersion2(BinaryReader reader)
        {
            ModelX model = ReadVersion1(reader);
            model.Up.X = reader.ReadSingle();
            model.Up.Y = reader.ReadSingle();
            model.Up.Z = reader.ReadSingle();

            return model;
        }

        private static ModelX ReadVersion1(BinaryReader reader)
        {
            ModelSubset[] subsets;
            VertexPositionNormalTexture[] vertices;

            vertices = new VertexPositionNormalTexture[reader.ReadInt32()];

            for (Int32 idx = 0; idx < vertices.Length; ++idx)
            {
                Vector3 position = new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
                Vector3 normal = new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
                Vector2 coords = new Vector2(reader.ReadSingle(), reader.ReadSingle());

                vertices[idx] = new VertexPositionNormalTexture(position, normal, coords);
            }

            subsets = new ModelSubset[reader.ReadInt32()];

            for (Int32 idx = 0; idx < subsets.Length; ++idx)
            {
                subsets[idx] = ModelSubset.Deserialize(reader);
            }

            Int32 materialSetsCount = reader.ReadInt32();
            Int32 materialsPerSet = reader.ReadInt32();

            List<Material[]> materialSets = new List<Material[]>();

            for ( Int32 idx = 0; idx < materialSetsCount; ++idx )
            {
                Material[] materials = new Material[materialsPerSet];
                materialSets.Add(materials);

                for ( Int32 idx2 = 0; idx2 < materialsPerSet; ++idx2 )
                {
                    materials[idx2] = Material.Deserialize(reader);
                }
            }

            return new ModelX(materialSets, subsets, vertices);
        }

        private void Serialize(BinaryWriter writer)
        {
            writer.Write('E');
            writer.Write('M');
            writer.Write('X');
            writer.Write(Version);

            writer.Write(Vertices.Length);

            for (Int32 idx = 0; idx < Vertices.Length; ++idx)
            {
                var vertex = Vertices[idx];

                writer.Write(vertex.Position.X);
                writer.Write(vertex.Position.Y);
                writer.Write(vertex.Position.Z);

                writer.Write(vertex.Normal.X);
                writer.Write(vertex.Normal.Y);
                writer.Write(vertex.Normal.Z);

                writer.Write(vertex.TextureCoordinate.X);
                writer.Write(vertex.TextureCoordinate.Y);
            }

            writer.Write(Subsets.Length);

            foreach (var subset in Subsets)
            {
                subset.Serialize(writer);
            }

            writer.Write(_materials.Count);
            writer.Write(_materials[0].Length);

            foreach(var set in _materials)
            {
                for ( Int32 idx = 0; idx < set.Length; ++idx )
                {
                    set[idx].Serialize(writer);
                }
            }

            writer.Write(Up.X);
            writer.Write(Up.Y);
            writer.Write(Up.Z);
        }

        public void Render(IShaderEffect effect, Vector3 ambientColor, Int32 materialsSet)
        {
            ModelXRenderer.Render(this, effect, ambientColor, materialsSet);
        }
    }
}
