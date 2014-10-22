using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
            file["Spacing"] = parser.ParseLength("Spacing", false);
            file["Padding"] = parser.ParseLength("Padding", false);

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

        Length _spacing;
        Length _padding;

        public int Spacing
        {
            get
            {
                return _spacing.Compute(0);
            }
        }

        public int Padding
        {
            get
            {
                return _padding.Compute();
            }

            set
            {
                _padding = new Length(value);
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

                switch (parameters.HorizontalAlignment)
                {
                case HorizontalAlignment.Center:
                        posX = (width - size.X) / 2;
                        break;

                case HorizontalAlignment.Right:
                        posX = width - Padding - parameters.Margin.Right - size.X;
                        break;

                case HorizontalAlignment.Stretch:
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

                switch (parameters.VerticalAlignment)
                {
                case VerticalAlignment.Center:
                        posY = (height - size.Y)/2;
                        break;

                case VerticalAlignment.Bottom:
                        posY = height - Padding - parameters.Margin.Bottom - size.Y;
                        break;

                case VerticalAlignment.Stretch:
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

            DefinitionFileWithStyle file = new DefinitionFileWithStyle(definition, typeof(UiStackPanel));

            StackMode = DefinitionResolver.Get<Mode>(Controller, Binding, file["Mode"], Mode.Vertical);
            _spacing = DefinitionResolver.Get<Length>(Controller, Binding, file["Spacing"], Length.Zero);
            _padding = DefinitionResolver.Get<Length>(Controller, Binding, file["Padding"], Length.Zero);

            InitChildren(Controller, binding, definition);
        }

    }
}
