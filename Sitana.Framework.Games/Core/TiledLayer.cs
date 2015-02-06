using System;
using System.IO;

namespace Sitana.Framework.Games
{
    public class TiledLayer: Layer
    {
        public const ushort Empty = 0xffff;

        public string Tileset { get; set;}

        public ushort[,] Content { get; private set;}

        public int Width { get; private set;}
        public int Height { get; private set;}

        public bool TiledWidth { get; set;}
        public bool TiledHeight { get; set;}

        public TiledLayer()
        {
            Content = new ushort[1, 1];
            Content[0, 0] = Empty;
            Tileset = string.Empty;
            Width = 1;
            Height = 1;
            TiledWidth = false;
            TiledHeight = false;
        }

        public void Resize(int width, int height)
        {
            int newSizeX = Math.Max(width, Content.GetLength(0));
            int newSizeY = Math.Max(height, Content.GetLength(1));

            if ( newSizeX > Content.GetLength(0) || newSizeY > Content.GetLength(1))
            {
                ushort[,] newContent = new ushort[newSizeX, newSizeY];

                int copyWidth = Math.Min(newSizeX, Content.GetLength(0));
                int copyHeight = Math.Min(newSizeY, Content.GetLength(1));

                for(int idxX = 0; idxX < copyWidth; ++idxX)
                {
                    for(int idxY = 0; idxY < copyHeight; ++idxY)
                    {
                        newContent[idxX, idxY] = Content[idxX, idxY];
                    }

                    for (int idxY = copyHeight; idxY < newSizeY; ++idxY)
                    {
                        newContent[idxX, idxY] = Empty;
                    }
                }

                for (int idxX = copyWidth; idxX < newSizeX; ++idxX )
                {
                    for (int idxY = 0; idxY < newSizeY; ++idxY )
                    {
                        newContent[idxX, idxY] = Empty;
                    }
                }
                Content = newContent;
            }

            Width = width;
            Height = height;
        }

        public override void Serialize(BinaryWriter writer)
        {
            base.Serialize(writer);

            writer.Write(Tileset);

            writer.Write(TiledWidth);
            writer.Write(TiledHeight);

            writer.Write(Width);
            writer.Write(Height);

            for(int idxX = 0; idxX < Width; ++idxX)
            {
                for (int idxY = 0; idxY < Height; ++idxY)
                {
                    writer.Write(Content[idxX, idxY]);
                }
            }
        }

        public override void Deserialize(BinaryReader reader)
        {
            base.Deserialize(reader);

            Tileset = reader.ReadString();

            TiledWidth = reader.ReadBoolean();
            TiledHeight = reader.ReadBoolean();

            int width = reader.ReadInt32();
            int height = reader.ReadInt32();

            Resize(width, height);

            for (int idxX = 0; idxX < width; ++idxX)
            {
                for (int idxY = 0; idxY < height; ++idxY)
                {
                    Content[idxX, idxY] = reader.ReadUInt16();
                }
            }
        }

        
    }
}

