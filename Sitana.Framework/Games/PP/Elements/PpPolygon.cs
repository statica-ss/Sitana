using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FarseerPhysics.Common;
using Microsoft.Xna.Framework;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;
using FarseerPhysics.Common.Decomposition;

namespace Sitana.Framework.PP.Elements
{
    public class PpPolygon: PpElement
    {
        public override Vector2 this[int index]
        {
            get
            {
                if (index == 0)
                {
                    return Center;
                }

                return _vertices[index-1];
            }

            set
            {
                if (index == 0)
                {
                    MoveAll(value);
                }
                else if (index > _vertices.Count)
                {
                    _vertices.Add(value);
                }
                else
                {
                    _vertices[index-1] = value;
                }

                Center = _vertices.GetCentroid();

                Triangles.Clear();
            }
        }

        public override int Count
        {
            get
            {
                return _vertices.Count + 1;
            }
        }

        public override bool Valid
        {
            get
            {
                PolygonError error = _vertices.CheckPolygon();

                switch (error)
                {
                    //case PolygonError.InvalidAmountOfVertices:
                    case PolygonError.AreaTooSmall:
                        return false;
                }

                return true;
            }
        }

        public PpPolygon()
        {
        }

        public PpPolygon(Vertices vertices)
        {
            _vertices.AddRange(vertices);
            Center = _vertices.GetCentroid();
        }

        public override Fixture GenerateFixture(World world)
        {
            Body body = BodyFactory.CreateBody(world);
            SetBodyProperties(body);

            return FixtureFactory.AttachPolygon(_vertices, Density, body, this);
        }

        public override void Insert(int index, Vector2 pos)
        {
            if (index < 1)
            {
                throw new Exception();
            }

            index--;

            if (index >= _vertices.Count)
            {
                _vertices.Add(pos);
            }
            else
            {
                _vertices.Insert(index, pos);
            }

            Triangles.Clear();
        }

        public override void Remove(int index)
        {
            if ( index > 0 )
            {
                _vertices.RemoveAt(index - 1);
            }

            Triangles.Clear();
        }

        public override void Cleanup()
        {
            for (int idx = 0; idx < _vertices.Count-1; )
            {
                if (_vertices[idx + 1] == _vertices[idx])
                {
                    _vertices.RemoveAt(idx);
                }
                else
                {
                    ++idx;
                }
            }
        }

        public override void FlipHorizontal()
        {
            var aabb = _vertices.GetAABB();

            float minx = aabb.LowerBound.X;
            float width = aabb.Width;

            for (int idx = 0; idx < _vertices.Count; ++idx)
            {
                Vector2 v = _vertices[idx];
                v.X = (width-(v.X-minx))+minx;
                _vertices[idx] = v;
            }

            Center = _vertices.GetCentroid();
            Triangles.Clear();
        }

        public override void FlipVertical()
        {
            var aabb = _vertices.GetAABB();

            float miny = aabb.LowerBound.Y;
            float height = aabb.Height;

            for (int idx = 0; idx < _vertices.Count; ++idx)
            {
                Vector2 v = _vertices[idx];
                v.Y = (height - (v.Y - miny)) + miny;
                _vertices[idx] = v;
            }

            Center = _vertices.GetCentroid();
            Triangles.Clear();
        }

        public override void Rotate(int index, float angle)
        {
            Vector2 pos = this[index];

            Matrix matrix = Matrix.CreateTranslation(-pos.X, -pos.Y, 0) * Matrix.CreateRotationZ(angle) * Matrix.CreateTranslation(pos.X, pos.Y, 0);

            _vertices.Transform(ref matrix);

            Center = _vertices.GetCentroid();
            Triangles.Clear();
        }

        public override bool CanAddVertices { get { return true; } }
    }
}
