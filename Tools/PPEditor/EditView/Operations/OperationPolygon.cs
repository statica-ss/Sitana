﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Ebatianos.Graphics;
using FarseerPhysics.Common;
using Microsoft.Xna.Framework;
using FarseerPhysics.Common.Decomposition;
using Microsoft.Xna.Framework.Graphics;
using Ebatianos.PP.Elements;

namespace Editor
{
    class OperationPolygon : OperationAdd
    {
        public override MouseOperationType Type { get { return MouseOperationType.Polygon; } }

        protected override void CreateElement(Vector2 pos)
        {
            _element = new PpPolygon(new Vertices() { pos });
            _index = 2;
        }
    }
}
