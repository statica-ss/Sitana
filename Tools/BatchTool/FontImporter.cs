using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sitana.Framework.Xml;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;

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
            string output = node.Attribute("Output");

            Bitmap bmp = Bitmap.FromFile(input) as Bitmap;
            Bitmap bmpOut = new Bitmap(bmp.Width, bmp.Height, PixelFormat.Format32bppArgb);

            if (bmp.GetPixel(0, 0) != Color.FromArgb(255, 0, 255))
            {
                Console.WriteLine("Invalid image");
            }

            ImageAttributes ia = new ImageAttributes();
            ia.SetColorKey(Color.FromArgb(254, 0, 254), Color.FromArgb(255, 1, 255));

            Graphics g = Graphics.FromImage(bmpOut);
            g.DrawImage(bmp, new Rectangle(0, 0, bmp.Width, bmp.Height), 0, 0, bmp.Width, bmp.Height, GraphicsUnit.Pixel, ia);

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

            bmpOut.Save(output + ".png", ImageFormat.Png);
            using (Stream stream = new FileStream(output + ".sft", FileMode.Create))
            {
                BinaryWriter writer = new BinaryWriter(stream);
                writer.Write(Path.GetFileNameWithoutExtension(output));

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
