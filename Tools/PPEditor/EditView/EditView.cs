using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sitana.Framework.Cs;
using Microsoft.Xna.Framework;
using Sitana.Framework;
using Sitana.Framework.PP.Elements;
using Sitana.Framework.Graphics;
using Microsoft.Xna.Framework.Graphics;

namespace Editor
{
    public class EditView: Singleton<EditView>
    {
        const int pixelUnit = 16;

        public const int MinUnit = 8;
        public const int MaxUnit = 256;

        protected static readonly Color PointSelectedColor = Color.White;
        protected static readonly Color PointColor = Color.Red;
        protected static readonly Color LineColor = Color.Orange;

        public Vector2 TopLeft { get; set; }
        public int DisplayUnit { get; set; }

        public MouseOperation Operation { get; set; }

        public PpElement Selection { get; set; }

        public bool SnapToGrid { get; set; }

        public EditView()
        {
            Reset();
        }

        public void Reset()
        {
            SnapToGrid = true;
            DisplayUnit = pixelUnit;
            TopLeft = Vector2.Zero;
        }

        public float GridMultiplier
        {
            get
            {
                int unit = DisplayUnit;

                while (unit < pixelUnit / 2)
                {
                    unit *= 2;
                }

                while (unit > pixelUnit * 2)
                {
                    unit /= 2;
                }

                return (float)unit / (float)DisplayUnit;
            }
        }

        public Vector2 PositionFromDisplay(Vector2 position, int height, bool snapToGrid = true)
        {
            position.Y = height - position.Y;

            position /= (float)DisplayUnit;

            position += TopLeft;

            if (snapToGrid && SnapToGrid)
            {
                position /= GridMultiplier;

                position.X += 0.5f;
                position.Y += 0.5f;

                position = position.TrimToIntValues();
                position *= GridMultiplier;
            }

            return position;
        }

        public Vector2 DisplayFromPosition(Vector2 position, int height)
        {
            position -= TopLeft;
            position *= (float)DisplayUnit;

            position.Y = (float)height - position.Y;

            return position;
        }

        public void DrawSelection(PrimitiveBatch batch, int height, PpElement _element, int selectedIndex)
        {
            batch.Begin(PrimitiveType.LineStrip);

            if (_element.Polygon.Count > 1)
            {
                for (int idx = 0; idx <= _element.Polygon.Count; ++idx)
                {
                    Vector2 pos = DisplayFromPosition(_element.Polygon[idx % _element.Polygon.Count], height);

                    batch.AddVertex(pos, LineColor);
                }
            }

            batch.End();

            batch.Begin(PrimitiveType.TriangleList, RasterizerState.CullNone);

            for (int idx = 0; idx < _element.Count; ++idx)
            {
                Vector2 vert = EditView.Instance.DisplayFromPosition(_element[idx], height);

                Color color = idx == selectedIndex ? PointSelectedColor : PointColor;

                batch.AddVertex(vert + new Vector2(-4, -4), color);
                batch.AddVertex(vert + new Vector2(-4, 4), color);
                batch.AddVertex(vert + new Vector2(4, 4), color);

                batch.AddVertex(vert + new Vector2(-4, -4), color);
                batch.AddVertex(vert + new Vector2(4, 4), color);
                batch.AddVertex(vert + new Vector2(4, -4), color);
            }

            batch.End();
        }

        public void DrawElement(PrimitiveBatch batch, int height, PpElement _element, bool selected)
        {
            if (_element.Triangles.Count == 0)
            {
                _element.GenerateTriangles();
            }

            Color color = _element.Color;

            if (selected)
            {
                color = color * 0.5f;
            }

            batch.Begin(PrimitiveType.TriangleList, RasterizerState.CullNone);

            for (int idx = 0; idx < _element.Triangles.Count; ++idx)
            {
                Vector2 pos = DisplayFromPosition(_element.Triangles[idx], height);

                batch.AddVertex(pos, color);
            }

            batch.End();
        }
    }
}
