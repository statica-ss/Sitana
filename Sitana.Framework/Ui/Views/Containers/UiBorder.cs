// SITANA - Copyright (C) The Sitana Team.
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using Sitana.Framework.Ui.DefinitionFiles;
using Sitana.Framework.Xml;
using Sitana.Framework.Ui.Controllers;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Sitana.Framework.Ui.Views
{
    public class UiBorder : UiContainer
    {
        public new static void Parse(XNode node, DefinitionFile file)
        {
            UiContainer.Parse(node, file);

            foreach(var cn in node.Nodes)
            {
                switch ( cn.Tag )
                {
                    case "UiBorder.Children":
                        ParseChildren(cn, file);
                        break;
                }
            }
        }

        protected override void Init(object controller, object binding, DefinitionFile definition)
        {
            base.Init(controller, binding, definition);
            InitChildren(Controller, binding, definition);
        }

        protected override Rectangle CalculateChildBounds(UiView view)
        {
            Point size = view.ComputeSize(Bounds.Width, Bounds.Height);
            PositionParameters pos = view.PositionParameters;

            Rectangle childRect = new Rectangle(0, 0, size.X, size.Y);

            int posX = pos.X.Compute(Bounds.Width);
            int posY = pos.Y.Compute(Bounds.Height);

            switch (pos.Align & Align.Horz)
            {
                case Align.Center:
                    childRect.X = posX - size.X / 2;
                    break;

                case Align.Left:
                    childRect.X = posX;
                    break;

                case Align.Right:
                    childRect.X = posX - size.X;
                    break;

                case Align.StretchHorz:
                    childRect.X = 0;
                    childRect.Width = Bounds.Width;
                    break;
            }

            switch (pos.Align & Align.Vert)
            {
                case Align.Middle:
                    childRect.Y = posY - size.Y / 2;
                    break;

                case Align.Top:
                    childRect.Y = posY;
                    break;

                case Align.Bottom:
                    childRect.Y = posY - size.Y;
                    break;

                case Align.StretchVert:
                    childRect.Y = 0;
                    childRect.Width = Bounds.Height;
                    break;
            }

            pos.Margin.RepairRect(ref childRect, Bounds.Width, Bounds.Height);
            return childRect;
        }
    }
}
