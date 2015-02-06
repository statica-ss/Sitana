using Microsoft.Xna.Framework;
using Sitana.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sitana.Framework;
using Sitana.Framework.Games;

namespace GameEditor.Tools
{
    class EraseTile : Tool
    {
        Color _backgroundColor;

        public EraseTile()
        {
            _backgroundColor = ColorsManager.Instance[":ContentBackground"].Value;
        }

        public override void OnDown(Vector2 position)
        {
            TiledLayer layer = Document.Current.SelectedLayer.Layer as TiledLayer;

            if (layer != null)
            {
                int startX = (int)position.X;
                int startY = (int)position.Y;

                layer.Content[startX, startY] = TiledLayer.Empty;
            }
        }

        public override void OnMove(Vector2 position)
        {
            OnDown(position);
        }

        public override void OnUp()
        {
        }

        public override void Draw(AdvancedDrawBatch batch, Point startPosition, Vector2 position, float scale)
        {
            int size = UnitToPixels(scale);
            int unitSize = UnitToPixels(1);

            Rectangle target = new Rectangle(0, 0, size, size);

            position = position.TrimToIntValues() * size;

            position.X -= startPosition.X;
            position.Y -= startPosition.Y;

            target.X = (int)(position.X);
            target.Y = (int)(position.Y);

            batch.DrawRectangle(target, _backgroundColor * 0.5f);
        }
    }
}
