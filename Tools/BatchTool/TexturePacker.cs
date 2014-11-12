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
        enum CropType
        {
            None = 0,
            Width = 1,
            Height = 2
        }

        enum BorderType
        {
            None,
            Smear
        }

        public static void Pack(XNode node)
        {
            Console.WriteLine("\nTexturePacker v.1.0.0");

            int maxWidth;
            int margin;
            CropType crop;
            BorderType border;

            if (!int.TryParse(node.Attribute("MaxWidth"), out maxWidth))
            {
                maxWidth = 2048;
            }

            if (!int.TryParse(node.Attribute("Margin"), out margin))
            {
                margin = 4;
            }

            if (!Enum.TryParse<CropType>(node.Attribute("Crop"), out crop))
            {
                crop = CropType.None;
            }

            if (!Enum.TryParse<BorderType>(node.Attribute("Border"), out border))
            {
                border = BorderType.None;
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

                    if (crop != CropType.None)
                    {
                        image = Crop(image, crop);
                    }

                    Console.WriteLine("File: {0} = {1} x {2} -> {3}", file, image.Width, image.Height, id);

                    images.Add(new Tuple<string, Image>(id, image));
                }

                images.Sort((i1, i2) => i1.Item2.Height - i2.Item2.Height);

                int posX = 0;
                int posY = 0;

                foreach (var img in images)
                {
                    
                    Bitmap image = img.Item2 as Bitmap;
                    
                    string id = img.Item1;

                    if (posX + image.Width >= maxWidth)
                    {
                        posX = 0;
                        posY = height;
                    }

                    Rectangle rect = new Rectangle(posX + margin, posY + margin, image.Width, image.Height);
                    textures.Add(new Tuple<string, Rectangle>(id, rect));

                    if (border == BorderType.Smear)
                    {
                        // topleft
                        Color color = image.GetPixel(0, 0);
                        graphics.FillRectangle(new SolidBrush(color), new Rectangle(posX, posY, margin, margin));

                        // bottomleft
                        color = image.GetPixel(0, image.Height-1);
                        graphics.FillRectangle(new SolidBrush(color), new Rectangle(posX, posY+margin+image.Height, margin, margin));

                        // topright
                        color = image.GetPixel(image.Width-1, 0);
                        graphics.FillRectangle(new SolidBrush(color), new Rectangle(posX + margin + image.Width, posY, margin, margin));

                        // bottomleft
                        color = image.GetPixel(image.Width-1, image.Height - 1);
                        graphics.FillRectangle(new SolidBrush(color), new Rectangle(posX + margin + image.Width, posY + margin + image.Height, margin, margin));

                        // top element
                        for (int idx = 0; idx < margin; ++idx)
                        {
                            Rectangle src = new Rectangle(0, 0, image.Width, 1);
                            Rectangle target = new Rectangle(posX + margin, posY + idx, src.Width, src.Height);

                            graphics.DrawImage(image, target, src, GraphicsUnit.Pixel);
                        }

                        // left element
                        for (int idx = 0; idx < margin; ++idx)
                        {
                            Rectangle src = new Rectangle(0, 0, 1, image.Height);
                            Rectangle target = new Rectangle(posX + idx, posY + margin, src.Width, src.Height);

                            graphics.DrawImage(image, target, src, GraphicsUnit.Pixel);
                        }

                        // bottom element
                        for (int idx = 0; idx < margin; ++idx)
                        {
                            Rectangle src = new Rectangle(0, image.Height - 1, image.Width, 1);
                            Rectangle target = new Rectangle(posX + margin, posY + margin + image.Height + idx, src.Width, src.Height);

                            graphics.DrawImage(image, target, src, GraphicsUnit.Pixel);
                        }

                        // right element
                        for (int idx = 0; idx < margin; ++idx)
                        {
                            Rectangle src = new Rectangle(image.Width - 1, 0, 1, image.Height);
                            Rectangle target = new Rectangle(posX + margin + image.Width + idx, posY + margin, src.Width, src.Height);
                            graphics.DrawImage(image, target, src, GraphicsUnit.Pixel);
                        }
                    }

                    graphics.DrawImage(image, posX + margin, posY + margin, image.Width, image.Height);

                    width = Math.Max(width, posX + image.Width);

                    posX += image.Width + margin * 2;

                    height = Math.Max(height, posY + image.Height + margin * 2);

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

        static Bitmap Crop(Bitmap bmp, CropType crop)
        {
            int w = bmp.Width;
            int h = bmp.Height;

            Func<int, bool> allTransparentRow = row =>
            {
                for (int i = 0; i < w; ++i)
                    if (bmp.GetPixel(i, row).A != 0)
                        return false;
                return true;
            };

            Func<int, bool> allTransparentColumn = col =>
            {
                for (int i = 0; i < h; ++i)
                    if (bmp.GetPixel(col, i).A != 0)
                        return false;
                return true;
            };

            int topmost = 0;
            for (int row = 0; row < h; ++row)
            {
                if (allTransparentRow(row))
                    topmost = row;
                else break;
            }

            int bottommost = 0;
            for (int row = h - 1; row >= 0; --row)
            {
                if (allTransparentRow(row))
                    bottommost = row;
                else break;
            }

            int leftmost = 0, rightmost = 0;
            for (int col = 0; col < w; ++col)
            {
                if (allTransparentColumn(col))
                    leftmost = col;
                else
                    break;
            }

            for (int col = w - 1; col >= 0; --col)
            {
                if (allTransparentColumn(col))
                    rightmost = col;
                else
                    break;
            }

            if (rightmost == 0) rightmost = w; // As reached left
            if (bottommost == 0) bottommost = h; // As reached top.

            rightmost = Math.Min(rightmost + 1, w);
            leftmost = Math.Max(0, leftmost-1);
            topmost = Math.Max(0, topmost-1);
            bottommost = Math.Min(bottommost + 1, h);

            int croppedWidth = rightmost - leftmost;
            int croppedHeight = bottommost - topmost;

            if (!crop.HasFlag(CropType.Width) || croppedWidth == 0) // No border on left or right
            {
                leftmost = 0;
                croppedWidth = w;
            }

            if (!crop.HasFlag(CropType.Height) || croppedHeight == 0) // No border on top or bottom
            {
                topmost = 0;
                croppedHeight = h;
            }

            try
            {
                var target = new Bitmap(croppedWidth, croppedHeight);
                using (Graphics g = Graphics.FromImage(target))
                {
                    g.Clear(Color.Transparent);
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
