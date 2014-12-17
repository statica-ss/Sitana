using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Sitana.Framework.Graphics
{
    public class Glyph
    {
        public char Character;

        public short X;
        public short Y;

        public short Width;
        public short Height;

        public short Top;

        // Attention: Kerning unit is 0.1px
        public short DefaultKerning = short.MinValue;
        private Dictionary<char, short> _kerning = new Dictionary<char, short>();

        public short Kerning(char previousChar)
        {
            short kerning;
            if (_kerning.TryGetValue(previousChar, out kerning))
            {
                return kerning;
            }

            return DefaultKerning;
        }

        public void AddKerning(char character, short value)
        {
            _kerning.Remove(character);
            _kerning.Add(character, value);
        }

        internal void Save(BinaryWriter writer)
        {
            Cleanup();

            writer.Write(Character);
            writer.Write(X);
            writer.Write(Y);
            writer.Write(Width);
            writer.Write(Height);
            writer.Write(Top);
            writer.Write(DefaultKerning);

            writer.Write(_kerning.Count);

            foreach(var kern in _kerning)
            {
                writer.Write(kern.Key);
                writer.Write(kern.Value);
            }
        }

        internal void Load(BinaryReader reader)
        {
            Character = reader.ReadChar();
            X = reader.ReadInt16();
            Y = reader.ReadInt16();
            Width = reader.ReadInt16();
            Height = reader.ReadInt16();
            Top = reader.ReadInt16();
            DefaultKerning = reader.ReadInt16();

            int count = reader.ReadInt32();
            _kerning.Clear();

            while(count>0)
            {
                char character = reader.ReadChar();
                short kerning = reader.ReadInt16();

                _kerning.Add(character, kerning);

                count--;
            }
        }

        internal void Cleanup()
        {
            if (_kerning.Count > 0 && DefaultKerning == short.MinValue)
            {
                short most = (from i in _kerning.Values
                              group i by i into grp
                              orderby grp.Count() descending
                              select grp.Key).First();

                var elements = _kerning.ToList();
                _kerning.Clear();

                for(int idx = 0; idx < elements.Count; ++idx)
                {
                    var element = elements[idx];
                    if (element.Value != most)
                    {
                        _kerning.Add(element.Key, element.Value);
                    }
                }

                DefaultKerning = most;
            }
        }
    }
}
