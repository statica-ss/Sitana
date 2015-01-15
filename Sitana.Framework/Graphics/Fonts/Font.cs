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
        struct StringProvider
        {
            private string _stringText;
            private StringBuilder _builderText;

            public StringProvider(string text)
            {
                _stringText = text;
                _builderText = null;
            }

            public StringProvider(StringBuilder text)
            {
                _builderText = text;
                _stringText = null;
            }

            public char this[int index]
            {
                get
                {
                    return _stringText != null ? _stringText[index] : _builderText[index];
                }
            }

            public int Length
            {
                get
                {
                    return _stringText != null ? _stringText.Length : _builderText.Length;
                }
            }
        }

        public short Height;
        public short BaseLine;
        public short CapLine;

        public string FontSheetPath;

        public Texture2D FontSheet { get; internal set; }

        private Dictionary<char, Glyph> _glyphs = new Dictionary<char, Glyph>();

        public void AddGlyph(Glyph glyph)
        {
            _glyphs.Add(glyph.Character, glyph);
        }

        public void Save(BinaryWriter writer)
        {
            writer.Write(FontSheetPath);

            writer.Write(Height);

            writer.Write(CapLine);
            writer.Write(BaseLine);

            writer.Write(_glyphs.Count);

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

                _glyphs.Add(glyph.Character, glyph);

                count--;
            }
        }

        internal void Draw(PrimitiveBatch primitiveBatch, StringBuilder text, Vector2 position, Color color, float spacing, float lineHeight, Vector2 scale)
        {
            StringProvider provider = new StringProvider(text);
            DrawInternal(primitiveBatch, provider, position, color, spacing, lineHeight, scale);
        }

        internal void Draw(PrimitiveBatch primitiveBatch, string text, Vector2 position, Color color, float spacing, float lineHeight, Vector2 scale)
        {
            StringProvider provider = new StringProvider(text);
            DrawInternal(primitiveBatch, provider, position, color, spacing, lineHeight, scale);
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

        void DrawInternal(PrimitiveBatch primitiveBatch, StringProvider text, Vector2 targetPosition, Color color, float spacing, float lineHeight, Vector2 scale)
        {
            if ( primitiveBatch.PrimitiveType != PrimitiveType.TriangleList )
            {
                throw new Exception("PrimitiveBatch has to be started with TriangleList primitive type.");
            }

            int count = text.Length;
            char previousChar = '\0';

            spacing = (float)Height * spacing * scale.X;

            Vector2 position = targetPosition;

            for (int idx = 0; idx < count; ++idx)
            {
                char character = text[idx];
                Glyph glyph = Find(character);

                if (glyph != null)
                {
                    if (previousChar != '\0')
                    {
                        
                        float kerning = (float)glyph.Kerning(previousChar) / 10f;
                        position.X += kerning * scale.X;
                        position.X += spacing;
                    }

                    previousChar = character;

                    Vector2 pos = new Vector2(position.X, position.Y + scale.Y * (glyph.Top - CapLine));

                    DrawGlyph(primitiveBatch, glyph, ref pos, ref color, scale);

                    position.X += glyph.Width * scale.X;
                }
                else if (text[idx] == '\n')
                {
                    position.X = targetPosition.X;
                    position.Y += lineHeight * Height * scale.Y;
                    previousChar = '\0';
                }
            }
        }

        void DrawGlyph(PrimitiveBatch primitiveBatch, Glyph glyph, ref Vector2 position, ref Color color, Vector2 scale)
        {
            Vector2 topLeft = new Vector2(glyph.X, glyph.Y) / new Vector2(FontSheet.Width, FontSheet.Height);
            Vector2 bottomRight = new Vector2(glyph.X + glyph.Width, glyph.Y + glyph.Height) / new Vector2(FontSheet.Width, FontSheet.Height);

            Vector2 positionBR = new Vector2(position.X + glyph.Width * scale.X, position.Y + glyph.Height * scale.X);

            primitiveBatch.AddVertex(new Vector2(position.X, position.Y), color, new Vector2(topLeft.X, topLeft.Y));
            primitiveBatch.AddVertex(new Vector2(position.X, positionBR.Y), color, new Vector2(topLeft.X, bottomRight.Y));
            primitiveBatch.AddVertex(new Vector2(positionBR.X, position.Y), color, new Vector2(bottomRight.X, topLeft.Y));

            primitiveBatch.AddVertex(new Vector2(position.X, positionBR.Y), color, new Vector2(topLeft.X, bottomRight.Y));
            primitiveBatch.AddVertex(new Vector2(positionBR.X, position.Y), color, new Vector2(bottomRight.X, topLeft.Y));
            primitiveBatch.AddVertex(new Vector2(positionBR.X, positionBR.Y), color, new Vector2(bottomRight.X, bottomRight.Y));
        }

        Glyph Find(char character)
        {
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
