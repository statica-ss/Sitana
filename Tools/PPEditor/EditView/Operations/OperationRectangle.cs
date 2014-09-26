using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using System.Windows.Forms;
using Sitana.Framework.Graphics;
using Microsoft.Xna.Framework.Graphics;
using Sitana.Framework.PP.Elements;

namespace Editor
{
    class OperationRectangle : OperationAdd
    {
        public override MouseOperationType Type { get { return MouseOperationType.Rectangle; } }

        protected override void CreateElement(Vector2 pos)
        {
            _element = new PpRectangle(pos, pos);
        }
    }
}
