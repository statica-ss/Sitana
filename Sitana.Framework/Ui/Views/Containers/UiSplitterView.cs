// SITANA - Copyright (C) The Sitana Team.
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Sitana.Framework.Diagnostics;
using Sitana.Framework.Ui.Controllers;
using Sitana.Framework.Ui.DefinitionFiles;
using Sitana.Framework.Ui.Views.Parameters;
using Sitana.Framework.Xml;

namespace Sitana.Framework.Ui.Views
{
    /// <summary>
    /// Parameters:
    /// SplitterPosition - splitter position in units (0..1)
    /// Mode = Horizontal | Vertical
    /// SplitterSize - size of splitter in pixels
    /// </summary>
    public class UiSplitterView: UiContainer
    {
        public new static void Parse(XNode node, DefinitionFile file)
        {
            UiContainer.Parse(node, file);

            DefinitionParser parser = new DefinitionParser(node);

            file["Mode"] = parser.ParseEnum<Mode>("Mode");
            file["Position"] = parser.ParseLength("Position");
            file["SplitterSize"] = parser.ParseLength("SplitterSize");
        }

        public enum Mode
        {
            Horizontal,
            Vertical
        }

        Length _splitterPosition;
        bool _verticalSplit = false;
        int _splitterSize = 4;

        public Length SplitterPosition
        {
            get
            {
                return _splitterPosition;
            }

            set
            {
                _splitterPosition = value;
                RecalcLayout();
            }
        }

        public Mode SplitMode
        {
            get
            {
                return _verticalSplit ? Mode.Vertical : Mode.Horizontal;
            }

            set
            {
                _verticalSplit = value == Mode.Vertical;

                RecalcLayout();
                OnChildrenModified();
            }
        }

        protected override Rectangle CalculateChildBounds(UiView view)
        {
            if (_children.Count < 2)
            {
                return Bounds;
            }

            int width = Bounds.Width;
            int height = Bounds.Height;

            if (_verticalSplit)
            {
                height -= _splitterSize;
            }
            else
            {
                width -= _splitterSize;
            }

            int index = _children.IndexOf(view);
            int divide = _verticalSplit ? _splitterPosition.Compute(height) : _splitterPosition.Compute(width);

            PositionParameters parameters = _children[index].PositionParameters;

            if (index == 0)
            {   
                if (_verticalSplit)
                {
                    return new Rectangle(parameters.Margin.Left, parameters.Margin.Top, width - parameters.Margin.Width, divide - parameters.Margin.Height);
                }
                else
                {
                    return new Rectangle(parameters.Margin.Left, parameters.Margin.Top, divide - parameters.Margin.Width, height - parameters.Margin.Height);
                }
            }
            else
            {
                if (_verticalSplit)
                {
                    return new Rectangle(parameters.Margin.Left, parameters.Margin.Top + divide + _splitterSize, width - parameters.Margin.Width, height - divide - parameters.Margin.Height);
                }
                else
                {
                    return new Rectangle(parameters.Margin.Left + divide + _splitterSize, parameters.Margin.Top, width - divide - parameters.Margin.Width, height - parameters.Margin.Height);
                }
            }
        }

        protected override void OnChildrenModified()
        {
            if (_children.Count > 2)
            {
                throw new InvalidOperationException("UiSplitView cannot have more than 2 children.");
            }

            if (_children.Count == 0)
            {
                _minSizeFromChildren = Point.Zero;
                return;
            }

            if (_children.Count == 1)
            {
                _minSizeFromChildren = _children[0].MinSize;
                return;
            }

            int minSizeX = 0;
            int minSizeY = 0;

            //if (_verticalSplit)
            //{
            //    minSizeX = Math.Max(_children[0].MinSize.X, _children[1].MinSize.X);
            //    minSizeY = Math.Max((int)(_children[0].MinSize.Y / _splitterPosition), (int)(_children[1].MinSize.Y / (1 - _splitterPosition)));
            //}
            //else
            //{
            //    minSizeX = Math.Max((int)(_children[0].MinSize.X / _splitterPosition), (int)(_children[1].MinSize.X / (1 - _splitterPosition)));
            //    minSizeY = Math.Max(_children[0].MinSize.Y, _children[1].MinSize.Y);
            //}

            _minSizeFromChildren = new Point(minSizeX, minSizeY);
        }

        protected override void Init(object controller, object binding, DefinitionFile definition)
        {
            base.Init(controller, binding, definition);

            DefinitionFileWithStyle file = new DefinitionFileWithStyle(definition, typeof(UiSplitterView));

            SplitMode = DefinitionResolver.Get<Mode>(Controller, Binding, file["Mode"], Mode.Vertical);
            _splitterPosition = DefinitionResolver.Get<Length>(Controller, Binding, file["Position"], new Length(0, 0.5));
            _splitterSize = DefinitionResolver.Get<Length>(Controller, Binding, file["SplitterSize"], Length.Default).Compute(100);

            InitChildren(Controller, Binding, definition);
        }
    }
}
