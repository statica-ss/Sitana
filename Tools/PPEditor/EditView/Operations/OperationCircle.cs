﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using System.Windows.Forms;
using Ebatianos.Graphics;
using Microsoft.Xna.Framework.Graphics;
using Ebatianos.PP.Elements;

namespace Editor
{
    class OperationCircle : OperationRectangle
    {
        public override MouseOperationType Type { get { return MouseOperationType.Circle; } }

        protected override void CreateElement(Vector2 pos)
        {
            _element = new PpCircle(pos, pos);
        }
        
    }
}
