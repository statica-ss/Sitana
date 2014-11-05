using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sitana.Framework.Graphics;
using Sitana.Framework;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GameEditor.Tools
{
    class InsertTiles: Tool
    {
        readonly Texture2D _tileset;
        readonly ushort[,] _tiles;
        readonly int _tileSize;

        public InsertTiles(Texture2D tileset, ushort[,] tiles, int tileSize)
        {
            _tileset = tileset;
            _tiles = tiles;
            _tileSize = tileSize;
        }

        public override void Draw(AdvancedDrawBatch batch, Point position, float scale)
        {
            Point pos = Point.Zero;
            Rectangle target = new Rectangle(0, 0, (int)(_tileSize * scale), (int)(_tileSize * scale));

            SamplerState oldSamplerState = batch.SamplerState;
            batch.SamplerState = SamplerState.PointClamp;

            for (int idxX = 0; idxX < _tiles.GetLength(0); ++idxX)
            {
                target.X = (int)(idxX * _tileSize * scale + position.X);

                for (int idxY = 0; idxY < _tiles.GetLength(1); ++idxY)
                {
                    target.Y = (int)(idxY * _tileSize * scale + position.Y);

                    ushort tile = _tiles[idxX, idxY];

                    Point src = new Point(tile&0xff, (tile>>8)&0xff);

                    Rectangle source = new Rectangle(src.X * _tileSize, src.Y*_tileSize, _tileSize-1, _tileSize-1);

                    batch.DrawImage(_tileset, target, source, Color.White * 0.5f);
                }
            }

            batch.SamplerState = oldSamplerState;
        }
    }
}
