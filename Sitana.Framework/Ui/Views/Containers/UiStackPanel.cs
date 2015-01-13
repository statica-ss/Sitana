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
        }

        public enum Mode
        {
            Horizontal,
            Vertical
        }

        bool _vertical = false;
        bool _updateBounds = true;
        bool _recalculateLayout = true;

        Length _spacing;
        Length _padding;

        List<UiView> _tempChildren = new List<UiView>();

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

            if (_recalculateLayout)
            {
                _recalculateLayout = false;
                RecalcLayout();
            }

            base.Update(time);
        }

        void UpdateBounds()
        {
            if (Parent != null)
            {
                Parent.RecalcLayout();
            }
        }

        public override void Add(UiView view)
        {
            if (!_children.Contains(view))
            {
                _children.Add(view);
                view.Parent = this;
                view.RegisterView();

                if (_added)
                {
                    view.ViewAdded();
                }

                OnChildrenModified();

                _recalculateLayout = true;
            }
        }

        public override Point ComputeSize(int width, int height)
        {
            Point value = base.ComputeSize(width, height);

            int size = _padding.Compute();

            for (int idx = 0; idx < _children.Count; ++idx)
            {
                var child = _children[idx];

                if (child.DisplayVisibility > 0)
                {
                    if ( size > _padding.Compute())
                    {
                        size += _spacing.Compute();
                    }

                    size += _vertical ? child.Bounds.Height + child.Margin.Height : child.Bounds.Width + child.Margin.Width;
                }
            }

            size += _padding.Compute();

            if (_vertical)
            {
                value.Y = size;
            }
            else
            {
                value.X = size;
            }

            return value;
        }

        protected override Rectangle CalculateChildBounds(UiView view)
        {
            Rectangle childBounds = Bounds;

            int index = _tempChildren.IndexOf(view);

            if ( index < 0)
            {
                return new Rectangle(100000, 10000, 100, 100);
            }

            PositionParameters parameters = _tempChildren[index].PositionParameters;

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
                    pos = _tempChildren[index - 1].Bounds.Bottom + _tempChildren[index - 1].PositionParameters.Margin.Bottom + Spacing;
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
                    pos = _tempChildren[index - 1].Bounds.Right + _tempChildren[index - 1].PositionParameters.Margin.Right + Spacing;
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

        public override void RecalcLayout()
        {
            _tempChildren.Clear();
            for(int idx = 0; idx < _children.Count; ++idx)
            {
                if ( _children[idx].DisplayVisibility > 0 )
                {
                    _tempChildren.Add(_children[idx]);
                }
            }
            base.RecalcLayout();
        }

        protected override void Init(object controller, object binding, DefinitionFile definition)
        {
            base.Init(controller, binding, definition);

            DefinitionFileWithStyle file = new DefinitionFileWithStyle(definition, typeof(UiStackPanel));

            StackMode = DefinitionResolver.Get<Mode>(Controller, Binding, file["Mode"], Mode.Vertical);
            _spacing = DefinitionResolver.Get<Length>(Controller, Binding, file["Spacing"], Length.Zero);
            _padding = DefinitionResolver.Get<Length>(Controller, Binding, file["Padding"], Length.Zero);

            InitChildren(Controller, Binding, definition);

            if ( StackMode == Mode.Vertical )
            {
                PositionParameters.Margin._top = null;
                PositionParameters.Margin._bottom = null;
            }
            else
            {
                PositionParameters.Margin._left = null;
                PositionParameters.Margin._right = null;
            }
        }
    }
}
