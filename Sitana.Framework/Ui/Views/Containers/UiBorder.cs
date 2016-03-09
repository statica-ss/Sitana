// SITANA - Copyright (C) The Sitana Team.
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using Sitana.Framework.Ui.DefinitionFiles;
using Sitana.Framework.Xml;
using Sitana.Framework.Ui.Controllers;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using System;

namespace Sitana.Framework.Ui.Views
{
    public class UiBorder : UiContainer
    {
        public new static void Parse(XNode node, DefinitionFile file)
        {
            UiContainer.Parse(node, file);

            var parser = new DefinitionParser(node);
            file["FitChildren"] = parser.ParseBoolean("FitChildren");
        }

        bool _fitChildren = false;
        bool _updateBounds = false;

        protected override bool Init(object controller, object binding, DefinitionFile definition)
        {
            if (!base.Init(controller, binding, definition))
            {
                return false;
            }

            var file = new DefinitionFileWithStyle(definition, typeof(UiContainer));

            _fitChildren = DefinitionResolver.Get<bool>(Controller, Binding, file["FitChildren"], false);

            TryInitChildren(definition);
            return true;
        }

        protected override Rectangle CalculateChildBounds(UiView view)
        {
            Point size = view.ComputeSize(Bounds.Width, Bounds.Height);
            PositionParameters pos = view.PositionParameters;

            Rectangle childRect = new Rectangle(0, 0, size.X, size.Y);

            int posX = pos.X.Compute(Bounds.Width);
            int posY = pos.Y.Compute(Bounds.Height);

            switch(pos.HorizontalAlignment)
            {
            case HorizontalAlignment.Center:
                childRect.X = posX - size.X / 2;
                break;

            case HorizontalAlignment.Left:
                childRect.X = posX;
                break;

            case HorizontalAlignment.Right:
                childRect.X = posX - size.X;
                break;

            case HorizontalAlignment.Stretch:
                childRect.X = 0;
                childRect.Width = Bounds.Width;
                break;
            }

            switch(pos.VerticalAlignment)
            {
            case VerticalAlignment.Center:
                childRect.Y = posY - size.Y / 2;
                break;

            case VerticalAlignment.Top:
                childRect.Y = posY;
                break;

            case VerticalAlignment.Bottom:
                childRect.Y = posY - size.Y;
                break;

            case VerticalAlignment.Stretch:
                childRect.Y = 0;
                childRect.Height = Bounds.Height;
                break;
            }

            pos.Margin.RepairRect(ref childRect, Bounds.Width, Bounds.Height);
            return childRect;
        }

        public override Point ComputeSize(int width, int height)
        {
            if (!_fitChildren)
            {
                return base.ComputeSize(width, height);
            }

            if (_shouldRecalcLayout)
            {
                RecalcLayout();
                _shouldRecalcLayout = false;
            }

            Point value = base.ComputeSize(width, height);

            int sizeX = 0;
            int sizeY = 0;

            for (int idx = 0; idx < _children.Count; ++idx)
            {
                var child = _children[idx];

                if (child.DisplayVisibility > 0)
                {
                    if (child.PositionParameters.HorizontalAlignment != HorizontalAlignment.Stretch)
                    {
                        sizeX = Math.Max(sizeX, child.Bounds.Width + child.Margin.Right + child.Bounds.X);
                    }

                    if (child.PositionParameters.VerticalAlignment != VerticalAlignment.Stretch)
                    {
                        sizeY = Math.Max(sizeY, child.Bounds.Height + child.Margin.Bottom + child.Bounds.Y);
                    }
                }
            }

            if (PositionParameters.Height.IsAuto)
            {
                value.Y = sizeY;

                if(Bounds.Height != sizeY)
                {
                    _updateBounds = true;
                }
            }

            if (PositionParameters.Width.IsAuto)
            {
                value.X = sizeX;

                if (Bounds.Width != sizeX)
                {
                    _updateBounds = true;
                }
            }

            return value;
        }

        protected override void Update(float time)
        {
            base.Update(time);

            if(_updateBounds)
            {
                if (Parent != null)
                {
                    SetForceRecalcFlag();
                    Parent.RecalcLayout();
                }
                _updateBounds = false;
            }
        }
    }

    
}
