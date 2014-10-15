using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Sitana.Framework.Graphics;

namespace Sitana.Framework.Diagnostics
{
    public class SevenSegmentDisplay
    {
        static Dictionary<char, int> _chars = new Dictionary<char, int>()
        {
            {' ', 0x00},
            {'0', 0x3f},
            {'1', 0x06},
            {'2', 0x5b},
            {'3', 0x4f},
            {'4', 0x66},
            {'5', 0x6d},
            {'6', 0x7d},
            {'7', 0x07},
            {'8', 0x7f},
            {'9', 0x6f},
            {'A', 0x77},
            {'B', 0x7c},
            {'C', 0x39},
            {'D', 0x5e},
            {'E', 0x79},
            {'F', 0x71},
            {'=', 0x48},
            {':', 0x48},
            {'-', 0x40},
        };

        Vector2 _position;
        int _size;
        int _thick;

        public SevenSegmentDisplay(int size, out int height)
        {
            _thick = size / 10;
            _thick = Math.Max(1, _thick);
            size = _thick * 10;
            _size = size;

            height = size * 16 / 10;
        }

        public void Reset(Point position)
        {
            _position = position.ToVector2();
        }

        public void Draw(AdvancedDrawBatch batch, StringBuilder text, Color color)
        {
            for (int idx = 0; idx < text.Length; ++idx)
            {
                int code;
                _chars.TryGetValue(text[idx], out code);
                Draw(batch, code, color);
            }
        }

        void Draw(AdvancedDrawBatch batch, int code, Color color)
        {
            Color c = Color.Black;

            batch.BeginPrimitive(Microsoft.Xna.Framework.Graphics.PrimitiveType.TriangleList, null);

            Color offColor = color * 0.2f;

            // a
            c = (code & 0x01) != 0 ? color : offColor;
            
            PushVertex(batch, 2, 2, ref c);
            PushVertex(batch, 3, 1, ref c);
            PushVertex(batch, 3, 3, ref c);

            PushVertex(batch, 8, 2, ref c);
            PushVertex(batch, 7, 1, ref c);
            PushVertex(batch, 7, 3, ref c);

            PushVertex(batch, 3, 1, ref c);
            PushVertex(batch, 3, 3, ref c);
            PushVertex(batch, 7, 1, ref c);

            PushVertex(batch, 3, 3, ref c);
            PushVertex(batch, 7, 1, ref c);
            PushVertex(batch, 7, 3, ref c);

            // b
            c = (code & 0x02) != 0 ? color : offColor;

            PushVertex(batch, 8, 2, ref c);
            PushVertex(batch, 9, 3, ref c);
            PushVertex(batch, 7, 3, ref c);

            PushVertex(batch, 8, 8, ref c);
            PushVertex(batch, 9, 7, ref c);
            PushVertex(batch, 7, 7, ref c);

            PushVertex(batch, 7, 3, ref c);
            PushVertex(batch, 9, 3, ref c);
            PushVertex(batch, 7, 7, ref c);

            PushVertex(batch, 9, 3, ref c);
            PushVertex(batch, 7, 7, ref c);
            PushVertex(batch, 9, 7, ref c);

            // c
            c = (code & 0x04) != 0 ? color : offColor;

            PushVertex(batch, 8, 8, ref c);
            PushVertex(batch, 9, 9, ref c);
            PushVertex(batch, 7, 9, ref c);

            PushVertex(batch, 8, 14, ref c);
            PushVertex(batch, 9, 13, ref c);
            PushVertex(batch, 7, 13, ref c);

            PushVertex(batch, 7, 9, ref c);
            PushVertex(batch, 9, 9, ref c);
            PushVertex(batch, 7, 13, ref c);

            PushVertex(batch, 9, 9, ref c);
            PushVertex(batch, 7, 13, ref c);
            PushVertex(batch, 9, 13, ref c);

            // d
            c = (code & 0x08) != 0 ? color : offColor;

            PushVertex(batch, 2, 14, ref c);
            PushVertex(batch, 3, 13, ref c);
            PushVertex(batch, 3, 15, ref c);

            PushVertex(batch, 8, 14, ref c);
            PushVertex(batch, 7, 13, ref c);
            PushVertex(batch, 7, 15, ref c);

            PushVertex(batch, 3, 13, ref c);
            PushVertex(batch, 3, 15, ref c);
            PushVertex(batch, 7, 13, ref c);

            PushVertex(batch, 3, 15, ref c);
            PushVertex(batch, 7, 13, ref c);
            PushVertex(batch, 7, 15, ref c);

            // e
            c = (code & 0x10) != 0 ? color : offColor;

            PushVertex(batch, 2, 8, ref c);
            PushVertex(batch, 3, 9, ref c);
            PushVertex(batch, 1, 9, ref c);

            PushVertex(batch, 2, 14, ref c);
            PushVertex(batch, 3, 13, ref c);
            PushVertex(batch, 1, 13, ref c);

            PushVertex(batch, 1, 9, ref c);
            PushVertex(batch, 3, 9, ref c);
            PushVertex(batch, 1, 13, ref c);

            PushVertex(batch, 3, 9, ref c);
            PushVertex(batch, 1, 13, ref c);
            PushVertex(batch, 3, 13, ref c);

            // f
            c = (code & 0x20) != 0 ? color : offColor;

            PushVertex(batch, 2, 2, ref c);
            PushVertex(batch, 3, 3, ref c);
            PushVertex(batch, 1, 3, ref c);

            PushVertex(batch, 2, 8, ref c);
            PushVertex(batch, 3, 7, ref c);
            PushVertex(batch, 1, 7, ref c);

            PushVertex(batch, 1, 3, ref c);
            PushVertex(batch, 3, 3, ref c);
            PushVertex(batch, 1, 7, ref c);

            PushVertex(batch, 3, 3, ref c);
            PushVertex(batch, 1, 7, ref c);
            PushVertex(batch, 3, 7, ref c);

            // g
            c = (code & 0x40) != 0 ? color : offColor;

            PushVertex(batch, 2, 8, ref c);
            PushVertex(batch, 3, 7, ref c);
            PushVertex(batch, 3, 9, ref c);

            PushVertex(batch, 8, 8, ref c);
            PushVertex(batch, 7, 7, ref c);
            PushVertex(batch, 7, 9, ref c);

            PushVertex(batch, 3, 7, ref c);
            PushVertex(batch, 3, 9, ref c);
            PushVertex(batch, 7, 7, ref c);

            PushVertex(batch, 3, 9, ref c);
            PushVertex(batch, 7, 7, ref c);
            PushVertex(batch, 7, 9, ref c);

            _position.X += _size;
        }

        void PushVertex(AdvancedDrawBatch batch, int x, int y, ref Color c)
        {
            batch.PushVertex(new Vector2(x * _thick, y * _thick) + _position, c);
        }
    }
}
