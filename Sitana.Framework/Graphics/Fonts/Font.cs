using System.Collections.Generic;
using System.IO;

namespace Sitana.Framework.Graphics
{
    public class Font
    {
        public short Height;
        public short BaseLine;
        public short CapLine;

        private Dictionary<char, Glyph> _glyphs = new Dictionary<char, Glyph>();

        public void AddGlyph(Glyph glyph)
        {
            _glyphs.Add(glyph.Character, glyph);
        }

        public void Save(BinaryWriter writer)
        {
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
            Height = reader.ReadInt16();

            CapLine = reader.ReadInt16();
            BaseLine = reader.ReadInt16();

            int count = reader.ReadInt32();

            while(count>0)
            {
                Glyph glyph = new Glyph();
                glyph.Load(reader);

                _glyphs.Add(glyph.Character, glyph);

                count--;
            }
        }
    }
}
