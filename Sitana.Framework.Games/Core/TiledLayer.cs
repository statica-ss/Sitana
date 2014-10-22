using System;

namespace Sitana.Framework.Games
{
    public class TiledLayer: Layer
    {
        public string Tileset { get; set;}

        public ushort[,] Content { get; private set;}

        public int Width { get; private set;}
        public int Height { get; private set;}

        public bool TiledWidth { get; set;}
        public bool TiledHeight { get; set;}

        public TiledLayer()
        {
            Content = new ushort[1, 1];
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
                }

                Content = newContent;
            }

            Width = width;
            Height = height;
        }
    }
}

