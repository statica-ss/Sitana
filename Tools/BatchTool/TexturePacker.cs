using System;
using System.Linq;
using Sitana.Framework.Xml;
using System.IO;
using Sitana.Framework;
using System.Drawing;
using System.Drawing.Imaging;
using System.Collections.Generic;

namespace BatchTool
{
    class TexturePacker
    {
        public static void Pack(XNode node)
        {
            Console.WriteLine("\nTexturePacker v.1.0.0");

            int maxWidth;
            int margin;
            bool crop;

            if (!int.TryParse(node.Attribute("MaxWidth"), out maxWidth))
            {
                maxWidth = 2048;
            }

            if (!int.TryParse(node.Attribute("Margin"), out margin))
            {
                margin = 4;
            }

            if (!bool.TryParse(node.Attribute("Crop"), out crop))
            {
                crop = false;
            }

            string input = node.Attribute("Input");
            string output = node.Attribute("Output");
            string textureId = node.Attribute("TextureId");

            string[] args = input.Split('\\', '/');

            input = args.Merge("/", 0, args.Length-1);

            Console.WriteLine("Directory: {0}", input);

            string pattern = args.Last();

            Console.WriteLine("Files: {0}", pattern);

            string[] files = Directory.GetFiles(input, pattern, SearchOption.AllDirectories);

            Bitmap tempBitmap = new Bitmap(maxWidth, 4096, PixelFormat.Format32bppArgb);

            List<Tuple<string, Image>> images = new List<Tuple<string, Image>>();
            List<Tuple<string, Rectangle>> textures = new List<Tuple<string, Rectangle>>();

            int width = 0;
            int height = 0;

            using (Graphics graphics = Graphics.FromImage(tempBitmap))
            {
                graphics.Clear(Color.Transparent);

                foreach (var file in files)
                {
                    string id = Path.Combine(Path.GetDirectoryName(file), Path.GetFileNameWithoutExtension(file));
                    id = textureId.Replace("*", id.Substring(input.Length)).Replace('\\', '/').Replace("//", "/");
                    Bitmap image = (Bitmap)Bitmap.FromFile(file);

                    if (crop)
                    {
                        image = Crop(image);
                    }

                    Console.WriteLine("File: {0} = {1} x {2} -> {3}", file, image.Width, image.Height, id);

                    images.Add(new Tuple<string, Image>(id, image));
                }

                images.Sort((i1, i2) => i1.Item2.Height - i2.Item2.Height);

                int posX = 0;
                int posY = 0;

                foreach (var img in images)
                {
                    Image image = img.Item2;
                    string id = img.Item1;

                    if (posX + image.Width >= maxWidth)
                    {
                        posX = 0;
                        posY = height;
                    }

                    Rectangle rect = new Rectangle(posX, posY, image.Width, image.Height);
                    textures.Add(new Tuple<string, Rectangle>(id, rect));

                    graphics.DrawImage(image, posX, posY, image.Width, image.Height);

                    width = Math.Max(width, posX + image.Width);

                    posX += image.Width + margin;
                    height = Math.Max(height, posY + image.Height + margin);

                    image.Dispose();
                }
            }

            Console.WriteLine("Output bitmap is {0}x{1}", width, height);

            Bitmap outputBitmap = new Bitmap(width, height, PixelFormat.Format32bppArgb);

            using (Graphics graphics = Graphics.FromImage(outputBitmap))
            {
                graphics.Clear(Color.Transparent);
                graphics.DrawImage(tempBitmap, 0, 0, new Rectangle(0,0,width, height), GraphicsUnit.Pixel );
            }

            tempBitmap.Dispose();

            Console.WriteLine("Saving {0}...", output);

            outputBitmap.Save(output + ".png", ImageFormat.Png);
            outputBitmap.Dispose();

            using (Stream stream = new FileStream(output + ".ta", FileMode.Create))
            {
                using (var writer = new BinaryWriter(stream))
                {
                    writer.Write(textures.Count);

                    foreach (var tex in textures)
                    {
                        writer.Write(tex.Item1);
                        writer.Write(tex.Item2.X);
                        writer.Write(tex.Item2.Y);
                        writer.Write(tex.Item2.Width);
                        writer.Write(tex.Item2.Height);
                    }
                }
            }
        }

        static Bitmap Crop(Bitmap bmp)
        {
            int w = bmp.Width;
            int h = bmp.Height;

            Func<int, bool> allWhiteRow = row =>
            {
                for (int i = 0; i < w; ++i)
                    if (bmp.GetPixel(i, row).R != 255)
                        return false;
                return true;
            };

            Func<int, bool> allWhiteColumn = col =>
            {
                for (int i = 0; i < h; ++i)
                    if (bmp.GetPixel(col, i).R != 255)
                        return false;
                return true;
            };

            int topmost = 0;
            for (int row = 0; row < h; ++row)
            {
                if (allWhiteRow(row))
                    topmost = row;
                else break;
            }

            int bottommost = 0;
            for (int row = h - 1; row >= 0; --row)
            {
                if (allWhiteRow(row))
                    bottommost = row;
                else break;
            }

            int leftmost = 0, rightmost = 0;
            for (int col = 0; col < w; ++col)
            {
                if (allWhiteColumn(col))
                    leftmost = col;
                else
                    break;
            }

            for (int col = w - 1; col >= 0; --col)
            {
                if (allWhiteColumn(col))
                    rightmost = col;
                else
                    break;
            }

            rightmost = Math.Min(rightmost + 1, w);
            leftmost = Math.Max(0, leftmost);
            topmost = Math.Max(0, topmost);
            bottommost = Math.Min(bottommost + 1, h);

            if (rightmost == 0) rightmost = w; // As reached left
            if (bottommost == 0) bottommost = h; // As reached top.

            int croppedWidth = rightmost - leftmost;
            int croppedHeight = bottommost - topmost;

            if (croppedWidth == 0) // No border on left or right
            {
                leftmost = 0;
                croppedWidth = w;
            }

            if (croppedHeight == 0) // No border on top or bottom
            {
                topmost = 0;
                croppedHeight = h;
            }

            try
            {
                var target = new Bitmap(croppedWidth, croppedHeight);
                using (Graphics g = Graphics.FromImage(target))
                {
                    g.Clear(Color.Aqua);
                    g.DrawImage(bmp,
                      new RectangleF(0, 0, croppedWidth, croppedHeight),
                      new RectangleF(leftmost, topmost, croppedWidth, croppedHeight),
                      GraphicsUnit.Pixel);

                    bmp.Dispose();
                }
                return target;
            }
            catch (Exception ex)
            {
                throw new Exception(
                  string.Format("Values are topmost={0} btm={1} left={2} right={3} croppedWidth={4} croppedHeight={5}", topmost, bottommost, leftmost, rightmost, croppedWidth, croppedHeight),
                  ex);
            }
        }
    }
}
