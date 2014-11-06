using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sitana.Framework.Xml;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace BatchTool
{
    class TilesetPacker
    {
        public static void Pack(XNode node)
        {
            Console.WriteLine("\nTilesetPacker v.1.0.0");

            string input = node.Attribute("Input");
            string output = node.Attribute("Output");

            int tileSize;
            int border;

            if (!int.TryParse(node.Attribute("TileSize"), out tileSize))
            {
                tileSize = 32;
            }

            if (!int.TryParse(node.Attribute("Border"), out border))
            {
                border = 2;
            }


            Bitmap bmp = Bitmap.FromFile(input) as Bitmap;

            Console.WriteLine("File: {0} = {1} x {2}", input, bmp.Width, bmp.Height);

            int width = bmp.Width / tileSize;
            int height = bmp.Height / tileSize;

            int outWidth = (border * 2 + tileSize) * width;
            int outHeight = (border * 2 + tileSize) * height;

            Bitmap outBitmap = new Bitmap(outWidth, outHeight, bmp.PixelFormat);
            Graphics graphics = Graphics.FromImage(outBitmap);

            for (int idxX = 0; idxX < width; idxX++)
            {
                int x = idxX * (tileSize + border * 2);
                int srcX = idxX * tileSize;

                for (int idxY = 0; idxY < height; idxY++)
                {
                    int y = idxY * (tileSize + border * 2);
                    int srcY = idxY * tileSize;

                    // topleft
                    Color color = bmp.GetPixel(srcX, srcY);
                    graphics.FillRectangle(new SolidBrush(color), new Rectangle(x, y, border, border));

                    // bottomleft
                    color = bmp.GetPixel(srcX, srcY + tileSize - 1);
                    graphics.FillRectangle(new SolidBrush(color), new Rectangle(x, y + border + tileSize, border, border));

                    // topright
                    color = bmp.GetPixel(srcX + tileSize - 1, srcY);
                    graphics.FillRectangle(new SolidBrush(color), new Rectangle(x + border + tileSize, y, border, border));

                    // bottomleft
                    color = bmp.GetPixel(srcX + tileSize - 1, srcY + tileSize - 1);
                    graphics.FillRectangle(new SolidBrush(color), new Rectangle(x + border + tileSize, y + border + tileSize, border, border));

                    // top element
                    for (int idx = 0; idx < border; ++idx)
                    {
                        Rectangle src = new Rectangle(srcX, srcY, tileSize, 1);
                        Rectangle target = new Rectangle(x + border, y + idx, src.Width, src.Height);

                        graphics.DrawImage(bmp, target, src, GraphicsUnit.Pixel);
                    }

                    // left element
                    for (int idx = 0; idx < border; ++idx)
                    {
                        Rectangle src = new Rectangle(srcX, srcY, 1, tileSize);
                        Rectangle target = new Rectangle(x + idx, y + border, src.Width, src.Height);

                        graphics.DrawImage(bmp, target, src, GraphicsUnit.Pixel);
                    }

                    // bottom element
                    for (int idx = 0; idx < border; ++idx)
                    {
                        Rectangle src = new Rectangle(srcX, srcY + tileSize - 1, tileSize, 1);
                        Rectangle target = new Rectangle(x + border, y + border + tileSize + idx, src.Width, src.Height);

                        graphics.DrawImage(bmp, target, src, GraphicsUnit.Pixel);
                    }

                    // right element
                    for (int idx = 0; idx < border; ++idx)
                    {
                        Rectangle src = new Rectangle(srcX + tileSize - 1, srcY, 1, tileSize);
                        Rectangle target = new Rectangle(x + border + tileSize + idx, y + border, src.Width, src.Height);
                        graphics.DrawImage(bmp, target, src, GraphicsUnit.Pixel);
                    }

                    {
                        Rectangle src = new Rectangle(srcX, srcY, tileSize, tileSize);
                        Rectangle target = new Rectangle(x + border, y + border, src.Width, src.Height);
                        graphics.DrawImage(bmp, target, src, GraphicsUnit.Pixel);
                    }
                }
            }

            graphics.Dispose();
            bmp.Dispose();

            Console.WriteLine("Output bitmap is {0}x{1}", outBitmap.Width, outBitmap.Height);
            Console.WriteLine("Saving {0}...", output);

            outBitmap.Save(output, ImageFormat.Png);
            outBitmap.Dispose();

            string path = Path.GetDirectoryName(output);

            path = Path.Combine(path, Path.GetFileNameWithoutExtension(output)) + ".tileset";

            using (Stream stream = new FileStream(path, FileMode.Create))
            {
                using (BinaryWriter writer = new BinaryWriter(stream))
                {
                    writer.Write(tileSize);
                    writer.Write(border);
                }
            }
        }
    }
}
