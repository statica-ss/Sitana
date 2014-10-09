using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Sitana.Framework.Content;
using System.IO;

namespace Sitana.Framework.Graphics
{
    public class PartialTexture2D
    {
        public readonly Texture2D Texture;
        public readonly Rectangle Source;

        public int Width { get { return Source.Width; } }
        public int Height { get { return Source.Height; } }

        public PartialTexture2D(Texture2D texture, Rectangle source)
        {
            Texture = texture;
            Source = source;
        }

        public static void LoadTextureAtlas(string path)
        {
            Texture2D texture = ContentLoader.Current.Load<Texture2D>(path);

            Type type = typeof(PartialTexture2D);

            using (Stream stream = ContentLoader.Current.Open(path + ".ta"))
            {
                using (BinaryReader reader = new BinaryReader(stream))
                {
                    int count = reader.ReadInt32();

                    while (count > 0)
                    {
                        string id = reader.ReadString();
                        Rectangle rect = new Rectangle();

                        rect.X = reader.ReadInt32();
                        rect.Y = reader.ReadInt32();
                        rect.Width = reader.ReadInt32();
                        rect.Height = reader.ReadInt32();

                        PartialTexture2D mapped = new PartialTexture2D(texture, rect);
                        ContentLoader.Current.AddContent(id, type, mapped);

                        count--;
                    }
                }
            }
        }
    }
}
