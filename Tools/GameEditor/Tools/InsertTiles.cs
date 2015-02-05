using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sitana.Framework.Graphics;
using Sitana.Framework;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using GameEditor.Views;
using Sitana.Framework.Games;

namespace GameEditor.Tools
{
    class InsertTiles: Tool
    {
        readonly Texture2D _tileset;
        readonly ushort[,] _tiles;

        Rectangle _invalidRect = Rectangle.Empty;

        public InsertTiles(Texture2D tileset, ushort[,] tiles)
        {
            _tileset = tileset;
            _tiles = tiles;
        }

        Vector2 RoundPosition(Vector2 position)
        {
            return new Vector2((int)(position.X + 0.5), (int)(position.Y + 0.5));
        }

        public override void Draw(AdvancedDrawBatch batch, Point startPosition, Vector2 position, float scale)
        {
            int size = UnitToPixels(scale);

            int unitSize = UnitToPixels(1);

            Rectangle target = new Rectangle(0, 0, size, size);

            position = position.TrimToIntValues() * size;

            position.X -= startPosition.X;
            position.Y -= startPosition.Y;

            SamplerState oldSamplerState = batch.SamplerState;
            batch.SamplerState = SamplerState.PointClamp;

            for (int idxX = 0; idxX < _tiles.GetLength(0); ++idxX)
            {
                target.X = (int)(idxX * size + position.X);

                for (int idxY = 0; idxY < _tiles.GetLength(1); ++idxY)
                {
                    target.Y = (int)(idxY * size + position.Y);

                    ushort tile = _tiles[idxX, idxY];

                    Point src = new Point(tile&0xff, (tile>>8)&0xff);

                    Rectangle source = new Rectangle(src.X * unitSize, src.Y * unitSize, unitSize - 1, unitSize - 1);

                    batch.DrawImage(_tileset, target, source, Color.White * 0.5f);
                }
            }

            batch.SamplerState = oldSamplerState;
        }

        public override void OnDown(Vector2 position)
        {
            PaintTiles(position);
        }

        public override void OnMove(Vector2 position)
        {
            PaintTiles(position);
        }

        public override void OnUp()
        {
            _invalidRect = Rectangle.Empty;
        }

        void PaintTiles(Vector2 position)
        {
            TiledLayer layer = Document.Current.SelectedLayer.Layer as TiledLayer;

            if (layer != null)
            {
                int startX = (int)position.X;
                int startY = (int)position.Y;

                int endX = startX + _tiles.GetLength(0);
                int endY = startY + _tiles.GetLength(1);

                Rectangle rect = Rectangle.Empty;

                rect.X = startX;
                rect.Y = startY;

                rect.Width = endX - startX;
                rect.Height = endY - startY;

                if(_invalidRect.Intersects(rect))
                {
                    return;
                }

                _invalidRect = rect;

                endX = Math.Min(endX, layer.Width);
                endY = Math.Min(endY, layer.Height);

                for (int idxX = startX; idxX < endX; ++idxX)
                {
                    for (int idxY = startY; idxY < endY; ++idxY)
                    {
                        ushort tile = _tiles[idxX-startX, idxY-startY];

                        layer.Content[idxX, idxY] = tile;
                    }
                }
            }
        }
    }
}
