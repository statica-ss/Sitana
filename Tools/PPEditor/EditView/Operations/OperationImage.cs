using Microsoft.Xna.Framework;
using Sitana.Framework.Games.Elements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Editor
{
    class OperationImage : OperationAdd
    {
        public override MouseOperationType Type { get { return MouseOperationType.Image; } }

        protected override void CreateElement(Vector2 pos)
        {
            _element = new PpRectangle(pos, pos);
        }
    }
}
