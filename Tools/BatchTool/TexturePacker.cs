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

            if (!int.TryParse(node.Attribute("MaxWidth"), out maxWidth))
            {
                maxWidth = 2048;
            }

            if (!int.TryParse(node.Attribute("Margin"), out margin))
            {
                margin = 4;
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
                    Image image = Bitmap.FromFile(file);
                    
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
    }
}
