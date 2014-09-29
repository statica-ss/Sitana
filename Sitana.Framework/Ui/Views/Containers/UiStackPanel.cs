using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sitana.Framework.Essentials.Ui.DefinitionFiles;
using Sitana.Framework.Xml;
using Sitana.Framework.Ui.DefinitionFiles;
using Microsoft.Xna.Framework;

namespace Sitana.Framework.Ui.Views
{
    public class UiStackPanel : UiContainer
    {
        public new static void Parse(XNode node, DefinitionFile file)
        {
            UiContainer.Parse(node, file);

            DefinitionParser parser = new DefinitionParser(node);

            file["Mode"] = parser.ParseEnum<Mode>("Mode");
            file["Spacing"] = parser.ParseInt("Spacing");
            file["Padding"] = parser.ParseInt("Padding");

            foreach (var cn in node.Nodes)
            {
                switch (cn.Tag)
                {
                    case "UiStackPanel.Children":
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

        bool _vertical = false;
        bool _updateBounds = true;

        int _spacing = 0;
        int _padding = 0;

        public int Spacing
        {
            get
            {
                return _spacing;
            }

            set
            {
                _spacing = value;
                RecalcLayout();
            }
        }

        public int Padding
        {
            get
            {
                return _padding;
            }

            set
            {
                _padding = value;
                RecalcLayout();
            }
        }

        public Mode StackMode
        {
            get
            {
                return _vertical ? Mode.Vertical : Mode.Horizontal;
            }

            set
            {
                _vertical = value == Mode.Vertical;

                RecalcLayout();
                OnChildrenModified();
            }
        }

        protected override void Update(float time)
        {
            if (_updateBounds)
            {
                UpdateBounds();
                _updateBounds = false;
            }

            base.Update(time);
        }

        void UpdateBounds()
        {
            Rectangle bounds = Bounds;

            int size = 0;

            for (int idx = 0; idx < _children.Count; ++idx)
            {
                var child = _children[idx];

                size += _vertical ? child.Bounds.Height : child.Bounds.Width;
            }

            if (_vertical)
            {
                bounds.Height = size;
            }
            else
            {
                bounds.Width = size;
            }

            if (Parent != null)
            {
                Parent.UpdateChildBounds(this, bounds);
            }
        }

        protected override Rectangle CalculateChildBounds(UiView view)
        {
            Rectangle childBounds = Bounds;

            

            int index = _children.IndexOf(view);

            PositionParameters parameters = _children[index].PositionParameters;

            int width = Bounds.Width;
            int height = Bounds.Height;

            if (_vertical)
            {
                Point size = view.ComputeSize(width, height);
                int posX = Padding + parameters.Margin.Left;

                switch (parameters.Align & Align.Horz)
                {
                    case Align.Center:
                        posX = (width - size.X) / 2;
                        break;

                    case Align.Right:
                        posX = width - Padding - parameters.Margin.Right - size.X;
                        break;

                    case Align.StretchHorz:
                        size.X = width - Padding * 2 - parameters.Margin.Width;
                        break;
                }

                int pos = Padding;

                if (index > 0)
                {
                    pos = _children[index - 1].Bounds.Bottom + _children[index - 1].PositionParameters.Margin.Bottom + Spacing;
                }

                childBounds.X = posX;
                childBounds.Width = size.X;

                childBounds.Y = pos + parameters.Margin.Top;
                childBounds.Height = size.Y;
            }
            else
            {
                Point size = view.ComputeSize(width, height);
                int posY = Padding + parameters.Margin.Top;

                switch (parameters.Align & Align.Vert)
                {
                    case Align.Middle:
                        posY = (height - size.Y)/2;
                        break;

                    case Align.Bottom:
                        posY = height - Padding - parameters.Margin.Bottom - size.Y;
                        break;

                    case Align.StretchVert:
                        size.Y = height - Padding * 2 - parameters.Margin.Height;
                        break;
                }

                int pos = Padding;

                if (index > 0)
                {
                    pos = _children[index - 1].Bounds.Right + _children[index - 1].PositionParameters.Margin.Right + Spacing;
                }

                childBounds.X = pos + parameters.Margin.Left;
                childBounds.Width = size.X;

                childBounds.Y = posY;
                childBounds.Height = size.Y;
            }

            _updateBounds = true;
            return childBounds;
        }

        protected override void OnChildrenModified()
        {
            if (_children.Count == 0)
            {
                _minSizeFromChildren = Point.Zero;
                return;
            }

            int minSizeX = 0;
            int minSizeY = 0;

            if (_vertical)
            {
                for (int idx = 0; idx < _children.Count; ++idx)
                {
                    minSizeX = Math.Max(minSizeX, _children[idx].MinSize.X);
                    minSizeY += _children[idx].MinSize.Y;
                }
            }
            else
            {
                for (int idx = 0; idx < _children.Count; ++idx)
                {
                    minSizeY = Math.Max(minSizeX, _children[idx].MinSize.Y);
                    minSizeX += _children[idx].MinSize.X;
                }
            }

            _minSizeFromChildren = new Point(minSizeX, minSizeY);
        }

        protected override void Init(object controller, object binding, DefinitionFile definition)
        {
            base.Init(controller, binding, definition);

            DefinitionFileWithStyle file = new DefinitionFileWithStyle(definition, typeof(UiSplitterView));

            StackMode = DefinitionResolver.Get<Mode>(Controller, binding, file["Mode"], Mode.Vertical);
            _spacing = DefinitionResolver.Get<int>(Controller, binding, file["Spacing"], 0);
            _padding = DefinitionResolver.Get<int>(Controller, binding, file["Padding"], 0);

            InitChildren(Controller, binding, definition);
        }

    }
}
