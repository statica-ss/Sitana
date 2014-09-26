using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Common;
using Sitana.Framework.DataTransfer;
using FarseerPhysics.Common.Decomposition;

namespace Sitana.Framework.PP.Elements
{
    public abstract class PpElement: Triangulatable, IEcsStructure
    {
        public float Density { get; set; }
        public Color Color { get; set; }

        public BodyType? Type { get; set; }

        protected Vertices _vertices = new Vertices();

        public abstract bool Valid { get; }

        protected Vector2 Center { get; set; }

        public PpElement()
        {
            Color = Color.CornflowerBlue;
            Density = 1;
            Type = BodyType.Static;
        }

        public virtual void Insert(int index, Vector2 pos)
        {
            throw new NotImplementedException();
        }

        public virtual void Remove(int index)
        {
            throw new NotImplementedException();
        }

        public virtual bool CanAddVertices{get{return false;}}

        public void MoveAll(Vector2 pos)
        {
            Vector2 move = pos - Center;

            for (int idx = 1; idx < Count; ++idx)
            {
                this[idx] = this[idx] + move;
            }

            Center = Center + move;
        }

        protected void SetBodyProperties(Body body)
        {
            BodyType type = Type ?? BodyType.Static;
            body.BodyType = type;
            body.Enabled = Type.HasValue;
        }

        public abstract Fixture GenerateFixture(World world);

        public abstract Vector2 this[int index]{get;set;}
        
        public abstract int Count {get;}
        
        public Vertices Polygon
        {
            get
            {
                return _vertices;
            }
        }

        void IEcsStructure.Read(EcsReader reader)
        {
            Color = reader.ReadInt32(1).ToColor();
            Density = reader.ReadSingle(2);

            Type = reader.HasField(3) ? (BodyType?)reader.ReadInt32(3) : null;

            int count = reader.ReadInt32(100);

            for (int idx = 0; idx < count; ++idx)
            {
                this[idx+1] = new Vector2(reader.ReadSingle(idx * 2 + 101), reader.ReadSingle(idx * 2 + 102));
            }
        }

        void IEcsStructure.Write(EcsWriter writer)
        {
            writer.Write(1, Color.ToInt());
            writer.Write(2, Density);

            if (Type.HasValue)
            {
                writer.Write(3, (int)Type.Value);
            }

            writer.Write(100, Count-1);

            for (int idx = 0; idx < Count-1; ++idx)
            {
                writer.Write((byte)(idx * 2 + 101), this[idx+1].X);
                writer.Write((byte)(idx * 2 + 102), this[idx+1].Y);
            }
        }

        public override void GenerateTriangles()
        {
            Triangles.Clear();

            if (Polygon.Count > 2)
            {
                try
                {
                    List<Vertices> triangles = Triangulate.ConvexPartition(Polygon, TriangulationAlgorithm.Earclip);

                    foreach (var tri in triangles)
                    {
                        if (tri.Count == 3)
                        {
                            foreach (var vert in tri)
                            {
                                Triangles.Add(vert);
                            }
                        }
                    }
                }
                catch
                {
                }
            }
        }

        public virtual void Cleanup()
        {
        }

        public PpElement Clone()
        {
            var writer = new EcsWriter();
            writer.Write(0, this);

            byte[] data = writer.Data;

            var reader = new EcsReader(data, 0, data.Length);
            reader.RegisterTypes(PpScene.SceneTypes);

            return reader.ReadStructure<PpElement>(0);
        }

        public virtual void FlipHorizontal(){}
        public virtual void FlipVertical(){}
        public virtual void Rotate(int index, float angle){}

        public virtual PpElement ToPolygon(int vertices = 0)
        {
            if (this is PpPolygon)
            {
                return this;
            }

            var clone = new PpPolygon(Polygon);
            clone.Density = Density;
            clone.Color = Color;
            clone.Type = Type;

            return clone;
        }
    }
}
