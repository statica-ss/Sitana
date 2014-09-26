using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FarseerPhysics.Dynamics;
using Microsoft.Xna.Framework;
using FarseerPhysics.Factories;

namespace Ebatianos.PP.Elements
{
    public class PpCircle: PpElement
    {
        Vector2 A;

        public override Vector2 this[int index]
        {
            set
            {
                if (index == 0)
                {
                    MoveAll(value);
                }
                else
                {
                    A = value;
                }

                UpdateVertices();
            }

            get
            {
                return index == 0 ? Center : A;
            }
        }

        public override bool Valid
        {
            get
            {
                return A != Center;
            }
        }

        public override int Count { get { return 2; } }

        public PpCircle()
        {
        }

        public PpCircle(Vector2 center, Vector2 pointOnCircle)
        {
            Center = center;
            A = pointOnCircle;
        }

        public PpCircle(Vector2 center, float radius)
        {
            Center = center;
            A = center + new Vector2(radius, 0);
        }

        public override Fixture GenerateFixture(World world)
        {
            Body body = BodyFactory.CreateBody(world);
            SetBodyProperties(body);

            float radius = (A-Center).Length();

            return FixtureFactory.AttachCircle(radius, Density, body, this);
        }

        protected void UpdateVertices()
        {
            const int steps = 32;

            float step = (float)(Math.PI * 2 / (double)steps);

            float radius = (A-Center).Length();

            _vertices.Clear();

            for (int idx = 0; idx < steps; ++idx)
            {
                float alpha = (float)idx * step;
                Vector2 rad = new Vector2((float)Math.Cos(alpha), (float)Math.Sin(alpha)) * radius + Center;

                _vertices.Add(rad);
            }

            Triangles.Clear();
        }
    }
}
