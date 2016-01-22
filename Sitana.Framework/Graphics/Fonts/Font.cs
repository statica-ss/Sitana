using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework.Graphics;
using Sitana.Framework.Content;
using System.Text;
using Microsoft.Xna.Framework;
using System;

namespace Sitana.Framework.Graphics
{
    public class Font
    {
        public short Height;
        public short BaseLine;
        public short CapLine;

        public string FontSheetPath;

        public Texture2D FontSheet { get; internal set; }

        private Dictionary<char, Glyph> _glyphs = new Dictionary<char, Glyph>();
        private Glyph[] _glyphsIndexed = new Glyph[96];

        private Color[] _colors = new Color[1];

        public static void LoadFontsPack(string path)
        {
            Texture2D texture = ContentLoader.Current.Load<Texture2D>(path);

            using (Stream stream = ContentLoader.Current.Open(path + ".pft"))
            {
                using (BinaryReader reader = new BinaryReader(stream))
                {
                    int count = reader.ReadInt32();

                    while (count > 0)
                    {
                        count--;

                        string name = reader.ReadString();

                        Font font = new Font();
                        font.Load(reader);

                        font.FontSheet = texture;
                        ContentLoader.Current.AddContent(name, typeof(Font), font);
                    }
                }
            }
        }

        public List<Glyph> GetGlyphs()
        {
            List<Glyph> glyphs = new List<Glyph>();
            foreach(var gl in _glyphs)
            {
                glyphs.Add(gl.Value);
            }

            foreach(var gl in _glyphsIndexed)
            {
                if(gl != null)
                {
                    glyphs.Add(gl);
                }
            }

            return glyphs;
        }

        public void AddGlyph(Glyph glyph)
        {
            int index = (int)glyph.Character - 32;

            if (index < 96 && index >= 0)
            {
                _glyphsIndexed[index] = glyph;
            }
            else
            {
                _glyphs.Add(glyph.Character, glyph);
            }
        }

        public void Save(BinaryWriter writer)
        {
            writer.Write(FontSheetPath);

            writer.Write(Height);

            writer.Write(CapLine);
            writer.Write(BaseLine);

            int glyphsIndexedCount = 0;
            foreach (var glyph in _glyphsIndexed)
            {
                if (glyph != null)
                {
                    glyphsIndexedCount++;
                }
            }

            writer.Write(_glyphs.Count + glyphsIndexedCount);

            foreach (var glyph in _glyphsIndexed)
            {
                if (glyph != null)
                {
                    glyph.Save(writer);
                }
            }

            foreach(var glyphPair in _glyphs)
            {
                Glyph glyph = glyphPair.Value;
                glyph.Save(writer);
            }
        }

        public void Load(BinaryReader reader)
        {
            FontSheetPath = reader.ReadString();

            Height = reader.ReadInt16();

            CapLine = reader.ReadInt16();
            BaseLine = reader.ReadInt16();

            int count = reader.ReadInt32();

            while (count > 0)
            {
                Glyph glyph = new Glyph();
                glyph.Load(reader);

                AddGlyph(glyph);

                count--;
            }
        }

        internal void Draw(PrimitiveBatch primitiveBatch, StringBuilder text, Vector2 position, Color[] colors, float opacity, float spacing, float lineHeight, float scale, TextRotation rotation = TextRotation.None)
        {
            StringProvider provider = new StringProvider(text);
            DrawInternal(primitiveBatch, provider, position, colors, opacity, spacing, lineHeight, scale, rotation);
        }

        internal void Draw(PrimitiveBatch primitiveBatch, StringBuilder text, Vector2 position, Color color, float spacing, float lineHeight, float scale, TextRotation rotation = TextRotation.None)
        {
            StringProvider provider = new StringProvider(text);
            _colors[0] = color;
            DrawInternal(primitiveBatch, provider, position, _colors, 1, spacing, lineHeight, scale, rotation);
        }

        internal void Draw(PrimitiveBatch primitiveBatch, string text, Vector2 position, Color color, float spacing, float lineHeight, float scale, TextRotation rotation = TextRotation.None)
        {
            StringProvider provider = new StringProvider(text);
            _colors[0] = color;
            DrawInternal(primitiveBatch, provider, position, _colors, 1, spacing, lineHeight, scale, rotation);
        }

        public Vector2 MeasureString(StringBuilder text, float spacing, float lineHeight)
        {
            StringProvider provider = new StringProvider(text);
            return MeasureInternal(provider, spacing, lineHeight);
        }

        public Vector2 MeasureString(string text, float spacing, float lineHeight)
        {
            StringProvider provider = new StringProvider(text);
            return MeasureInternal(provider, spacing, lineHeight);
        }

        Vector2 MeasureInternal(StringProvider text, float spacing, float lineHeight)
        {
            int count = text.Length;
            char previousChar = '\0';

            Vector2 position = Vector2.Zero;

            Vector2 size = Vector2.Zero;

            spacing = (float)Height * spacing;

            for (int idx = 0; idx < count; ++idx)
            {
                char character = text[idx];
                Glyph glyph = Find(character);

                if (glyph != null)
                {
                    if (previousChar != '\0')
                    {
                        float kerning = (float)glyph.Kerning(previousChar) / 10f;
                        position.X += kerning;
                        position.X += spacing;
                    }

                    previousChar = character;

                    position.X += glyph.Width;

                    size.X = Math.Max(size.X, position.X);
                    size.Y = position.Y + BaseLine - CapLine;
                }
                else if (text[idx] == '\n')
                {
                    position.X = 0;
                    position.Y += Height * lineHeight;
                    previousChar = '\0';
                }
            }

            return size;
        }

        void DrawInternal(PrimitiveBatch primitiveBatch, StringProvider text, Vector2 targetPosition, Color[] colors, float opacity, float spacing, float lineHeight, float scale, TextRotation rotation)
        {
            if ( primitiveBatch.PrimitiveType != PrimitiveType.TriangleList )
            {
                throw new Exception("PrimitiveBatch has to be started with TriangleList primitive type.");
            }

            Vector2 right = new Vector2(1, 0);
            Vector2 down = new Vector2(0, 1);

            switch(rotation)
            {
                case TextRotation.Rotate180:
                    right = new Vector2(-1, 0);
                    down = new Vector2(0, -1);
                    break;

                case TextRotation.Rotate270:
                    right = new Vector2(0, 1);
                    down = new Vector2(-1, 0);
                    break;

                case TextRotation.Rotate90:
                    right = new Vector2(0, -1);
                    down = new Vector2(1, 0);
                    break;
            }

            int count = text.Length;
            char previousChar = '\0';

            spacing = (float)Height * spacing * scale;

            Vector2 position = targetPosition;

            Color color;

            for (int idx = 0; idx < count; ++idx)
            {
                char character = text[idx];
                Glyph glyph = Find(character);

                if (glyph != null)
                {
                    if (previousChar != '\0')
                    {
                        float kerning = (float)glyph.Kerning(previousChar) / 10f;
                        position += right * (kerning * scale + spacing);
                    }

                    previousChar = character;

                    Vector2 pos = position + down * (scale *(glyph.Top - CapLine));

                    color = colors[idx % colors.Length] * opacity;

                    DrawGlyph(primitiveBatch, glyph, ref pos, ref color, scale, ref right, ref down);

                    position += right * (glyph.Width * scale);
                }
                else if (text[idx] == '\n')
                {
                    if (rotation == TextRotation.Rotate270 || rotation == TextRotation.Rotate90)
                    {
                        position.Y = targetPosition.Y;
                    }
                    else
                    {
                        position.X = targetPosition.X;
                    }

                    position += down * (lineHeight * Height * scale);
                    previousChar = '\0';
                }
            }
        }

        void DrawGlyph(PrimitiveBatch primitiveBatch, Glyph glyph, ref Vector2 position, ref Color color, float scale, ref Vector2 right, ref Vector2 down)
        {
            float topLeftX = (glyph.X-1) / (float)FontSheet.Width;
            float topLeftY = (glyph.Y-1) / (float)FontSheet.Height;

            float bottomRightX = (float)(glyph.X + glyph.Width + 1) / (float)FontSheet.Width;
            float bottomRightY = (float)(glyph.Y + glyph.Height + 1) / (float)FontSheet.Height;

            Vector2 topLeft = position - (right + down) * scale;
            Vector2 topRight = position + (right - down) * scale + right * glyph.Width * scale;

            Vector2 bottomLeft = position - (right - down) * scale + down * glyph.Height * scale;
            Vector2 bottomRight = position + (right + down) * scale + right * glyph.Width * scale + down * glyph.Height * scale;
            
			primitiveBatch.AddVertex(topLeft.X, topLeft.Y, ref color, topLeftX, topLeftY);
			primitiveBatch.AddVertex(bottomLeft.X, bottomLeft.Y, ref color, topLeftX, bottomRightY);
			primitiveBatch.AddVertex(topRight.X, topRight.Y, ref color, bottomRightX, topLeftY);

			primitiveBatch.AddVertex(bottomLeft.X, bottomLeft.Y, ref color, topLeftX, bottomRightY);
			primitiveBatch.AddVertex(topRight.X, topRight.Y, ref color, bottomRightX, topLeftY);
			primitiveBatch.AddVertex(bottomRight.X, bottomRight.Y, ref color, bottomRightX, bottomRightY);
        }

        public Glyph Find(char character)
        {
            int index = (int)character - 32;

            if (index < 96 && index >=0)
            {
                return _glyphsIndexed[index];
            }

            if (character == 0xa0)
            {
                character = ' ';
            }

            Glyph glyph;
            _glyphs.TryGetValue(character, out glyph);

            return glyph;
        }
    }
}
