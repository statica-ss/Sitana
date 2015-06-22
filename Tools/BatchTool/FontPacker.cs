using Sitana.Framework.Graphics;
using Sitana.Framework.Xml;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SitanaFont = Sitana.Framework.Graphics.Font;
using SitanaGlyph = Sitana.Framework.Graphics.Glyph;


namespace BatchTool
{
    class FontPacker
    {
        public static void Pack(XNode node)
        {
            string[] input = node.Attribute("Input").Split(';');

            List<string> files = new List<string>();

            foreach(var file in input)
            {
                if(file.Contains('*'))
                {
                    foreach(var dirFile in Directory.GetFiles("./", file, SearchOption.TopDirectoryOnly))
                    {
                        files.Add(dirFile.Replace("./", string.Empty));
                    }
                }
                else
                {
                    files.Add(file);
                }
            }

            string outputFile = node.Attribute("Output");
            string outFont = outputFile + ".pft";
            string outTexture = outputFile + ".png";

            string fontId = node.Attribute("FontId");

            int margin = 0;

            int.TryParse(node.Attribute("Margin"), out margin);

            List<Bitmap> bitmaps = new List<Bitmap>();
            List<SitanaFont> fonts = new List<SitanaFont>();

            int height = 0;
            int width = 0;

            foreach(var file in files)
            {
                Bitmap bmp;
                SitanaFont font = LoadFont(file, out bmp);

                height += bmp.Height + margin;
                width = Math.Max(width, bmp.Width);

                bitmaps.Add(bmp);
                fonts.Add(font);
            }

            Bitmap newBitmap = new Bitmap(width, height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            using(Graphics graphics = Graphics.FromImage(newBitmap))
            {
                int posY = 0;

                for(int idx = 0; idx < bitmaps.Count; ++idx)
                {
                    SitanaFont font = fonts[idx];
                    Bitmap bitmap = bitmaps[idx];

                    graphics.DrawImage(bitmap, 0, posY, bitmap.Width, bitmap.Height);

                    OffsetTexture(font, posY);
                    font.FontSheetPath = outputFile;

                    posY += margin + bitmap.Height;
                }
            }

            newBitmap.Save(outTexture, ImageFormat.Png);

            using(Stream stream = File.OpenWrite(outFont))
            {
                using(BinaryWriter writer = new BinaryWriter(stream))
                {
                    writer.Write(fonts.Count);

                    for (int idx = 0; idx < fonts.Count; ++idx)
                    {
                        string name = Path.GetFileNameWithoutExtension(files[idx]);
                        name = fontId.Replace("*", name);
                        writer.Write(name);

                        fonts[idx].Save(writer);
                    }
                }
            }
        }

        static void OffsetTexture(SitanaFont font, int offset)
        {
            if (offset == 0)
            {
                return;
            }

            foreach(var glyph in font.GetGlyphs())
            {
                glyph.Y = (short)(glyph.Y + offset);
            }
        }

        static SitanaFont LoadFont(string name, out Bitmap bitmap)
        {
            SitanaFont font = new SitanaFont();

            using (Stream stream = File.OpenRead(name))
            {
                using (BinaryReader reader = new BinaryReader(stream))
                {
                    font.Load(reader);
                }
            }

            string sheetPath = Path.GetFileNameWithoutExtension(name);

            if (!string.IsNullOrWhiteSpace(font.FontSheetPath))
            {
                sheetPath = font.FontSheetPath;
            }

            sheetPath = Path.Combine(Path.GetDirectoryName(name), sheetPath) + ".png";

            bitmap = (Bitmap)Bitmap.FromFile(sheetPath);

            return font;
        }

    }
}
