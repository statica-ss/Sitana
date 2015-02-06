using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sitana.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace GameEditor.Tools
{
    abstract class Tool
    {
        public static Tool Current = null;

        public static int UnitToPixels(float zoom)
        {
            return (int)Math.Ceiling(CurrentTemplate.Instance.UnitSize * zoom);
        }

        public static Vector2 PositionToUnit(Point pos, Point scrollPosition, float zoom)
        {
            float unitSize = UnitToPixels(zoom);

            return new Vector2(
                (float)(pos.X + scrollPosition.X) / unitSize,
                (float)(pos.Y + scrollPosition.Y) / unitSize
                );
        }

        protected Tool()
        {
            Current = this;
        }

        public virtual void Draw(AdvancedDrawBatch batch, Point startPosition, Vector2 position, float scale)
        {
        }

        public virtual void OnDown(Vector2 position)
        {

        }

        public virtual void OnMove(Vector2 position)
        {

        }

        public virtual void OnUp()
        {

        }
    }
}
