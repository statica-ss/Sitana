using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;
using Microsoft.Xna.Framework;
using FarseerPhysics.Common;

namespace Sitana.Framework.Games.Elements
{
    public class PpRectangle: PpElement
    {
        protected Vector2 A { get; set; }
        protected Vector2 B { get; set; }

        public override Vector2 this[int index]
        {
            get
            {
                if (index == 0)
                {
                    return Center;
                }
                else if (index == 1)
                {
                    return A;
                }

                return B;
            }

            set
            {
                if (index == 0)
                {
                    MoveAll(value);
                }
                else if (index == 1)
                {
                    A = value;
                }
                else
                {
                    B = value;
                }

                Center = (A + B) / 2;
                UpdateVertices();
            }
        }

        public override bool Valid
        {
            get
            {
                return A.X != B.X && A.Y != B.Y;
            }
        }

        public override int Count{get{return 3;}}

        public PpRectangle()
        {
        }

        public PpRectangle(Vector2 a, Vector2 b)
        {
            A = a;
            B = b;
            Center = (A + B) / 2;
            UpdateVertices();
        }

        public override Fixture GenerateFixture(World world)
        {
            Body body = BodyFactory.CreateBody(world);
            SetBodyProperties(body);

            return FixtureFactory.AttachRectangle(Math.Abs(A.X - B.X), Math.Abs(A.Y - B.Y), Density, (A + B) / 2, body, this);
        }

        protected virtual void UpdateVertices()
        {
            _vertices.Clear();
            _vertices.Add(new Vector2(A.X, A.Y));
            _vertices.Add(new Vector2(A.X, B.Y));
            _vertices.Add(new Vector2(B.X, B.Y));
            _vertices.Add(new Vector2(B.X, A.Y));

            Triangles.Clear();
        }
    }
}
