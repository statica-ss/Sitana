using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sitana.Framework.Xml;
using System.IO;
using System.Drawing;

namespace BatchTool
{
    class FontImporter
    {
        class Glyph
        {
            public char Character;

            public short X;
            public short Y;

            public short Width;
            public short Height;
        }

        public static void Import(XNode node)
        {
            string input = node.Attribute("Input");
            string output = Path.Combine(Path.GetDirectoryName(input), Path.GetFileNameWithoutExtension(input)) + ".sft";

            Bitmap bmp = Bitmap.FromFile(input) as Bitmap;

            if (bmp.GetPixel(0, 0) != Color.FromArgb(255, 0, 255))
            {
                Console.WriteLine("Invalid image");
            }

            int height = 0;
            int character = 32;

            List<Glyph> glyphs = new List<Glyph>();

            for (int idx = 0; ; ++idx)
            {
                if (bmp.GetPixel(1, idx + 1) == Color.FromArgb(255, 0, 255))
                {
                    break;
                }
                height++;
            }

            int posy = 1;
            int start = 1;

            for (int idx = 1; ; idx++)
            {
                if (idx >= bmp.Width)
                {
                    posy += height + 1;
                    idx = 1;

                    if (posy >= bmp.Height)
                    {
                        break;
                    }
                }

                if (bmp.GetPixel(idx, posy) == Color.FromArgb(255, 0, 255))
                {
                    if (start != idx)
                    {
                        glyphs.Add(new Glyph()
                        {
                            X = (short)start,
                            Y = (short)posy,
                            Width = (short)(idx - start),
                            Height = (short)height,
                            Character = (char)character
                        });
                        character++;
                    }

                    start = idx + 1;
                }
            }

            using (Stream stream = new FileStream(output, FileMode.Create))
            {
                BinaryWriter writer = new BinaryWriter(stream);
                writer.Write(Path.GetFileNameWithoutExtension(input));

                writer.Write((short)height);

                writer.Write((short)0);
                writer.Write((short)height);

                writer.Write(glyphs.Count);

                foreach (var glyph in glyphs)
                {
                    writer.Write(glyph.Character);
                    writer.Write(glyph.X);
                    writer.Write(glyph.Y);
                    writer.Write(glyph.Width);
                    writer.Write(glyph.Height);
                    writer.Write((short)0);
                    writer.Write((short)0);
                    writer.Write(0);
                }
            }
        }
    }
}
