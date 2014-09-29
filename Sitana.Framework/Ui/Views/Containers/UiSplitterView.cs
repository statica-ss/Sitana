// SITANA - Copyright (C) The Sitana Team.
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Sitana.Framework.Diagnostics;
using Sitana.Framework.Essentials.Ui.DefinitionFiles;
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

            file["Mode"] = parser.ParseEnum<Mode>("Mode", Mode.Horizontal);
            file["Position"] = parser.ParseLength("Position");
            file["SplitterSize"] = parser.ParseLength("SplitterSize");

            foreach (var cn in node.Nodes)
            {
                switch (cn.Tag)
                {
                    case "UiSplitterView.Children":

                        if ( cn.Nodes.Count != 2 )
                        {
                            string error = node.NodeError("UiSplitterView must have exactly 2 children.");
                            if (DefinitionParser.EnableCheckMode)
                            {
                                ConsoleEx.WriteLine(error);
                            }
                            else
                            {
                                throw new Exception(error);
                            }
                        }

                        ParseChildren(cn, file);
                        break;
                }
            }
        }

        public enum Mode
        {
            Horizontal,
            Vertical
        }

        float _splitterPosition = 0.5f;
        bool _verticalSplit = false;
        int _splitterSize = 4;

        public float SplitterPosition
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

        protected override void Draw(ref UiViewDrawParameters parameters)
        {
            if (DisplayOpacity == 0)
            {
                return;
            }

            UiViewDrawParameters drawParams = parameters;

            parameters.DrawBatch.DrawRectangle(ScreenBounds, BackgroundColor * DisplayOpacity);

            for (int idx = 0; idx < _children.Count; ++idx)
            {
                parameters.DrawBatch.PushClip(_children[idx].ScreenBounds);
                _children[idx].ViewDraw(ref drawParams);
                parameters.DrawBatch.PopClip();
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
            int divide = _verticalSplit ? (int)(_splitterPosition * (float)height) : (int)(_splitterPosition * (float)width);

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

            if (_verticalSplit)
            {
                minSizeX = Math.Max(_children[0].MinSize.X, _children[1].MinSize.X);

                minSizeY = Math.Max((int)(_children[0].MinSize.Y / _splitterPosition), (int)(_children[1].MinSize.Y / (1 - _splitterPosition)));
            }
            else
            {
                minSizeX = Math.Max((int)(_children[0].MinSize.X / _splitterPosition), (int)(_children[1].MinSize.X / (1 - _splitterPosition)));

                minSizeY = Math.Max(_children[0].MinSize.Y, _children[1].MinSize.Y);
            }

            _minSizeFromChildren = new Point(minSizeX, minSizeY);
        }

        protected override void Init(UiController controller, object binding, DefinitionFile file)
        {
            base.Init(ref controller, binding, file);

            SplitMode = DefinitionResolver.Get<Mode>(controller, binding, file["Mode"]);
            _splitterPosition = (float)DefinitionResolver.Get<Length>(controller, binding, file["Position"]).Compute(100) / 100.0f;
            _splitterSize = DefinitionResolver.Get<Length>(controller, binding, file["SplitterSize"]).Compute(100);

            List<DefinitionFile> children = file["Children"] as List<DefinitionFile>;

            if (children != null)
            {
                for (int idx = 0; idx < children.Count; ++idx)
                {
                    var childFile = children[idx];
                    var child = childFile.CreateInstance(controller, binding) as UiView;
                    child.CreatePositionParameters(controller, binding, childFile, typeof(PositionParameters));

                    if (child != null)
                    {
                        Add(child);
                    }
                }
            }
        }
    }
}
