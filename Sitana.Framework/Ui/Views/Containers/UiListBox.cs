using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sitana.Framework.Xml;
using Sitana.Framework.Ui.DefinitionFiles;
using Sitana.Framework.Diagnostics;
using Microsoft.Xna.Framework;
using Sitana.Framework.Ui.Binding;
using Sitana.Framework.Ui.Views.Parameters;
using Sitana.Framework.Input.TouchPad;
using Sitana.Framework.Ui.Core;

namespace Sitana.Framework.Ui.Views
{
    public class UiListBox: UiContainer, IItemsConsumer
    {
        public new static void Parse(XNode node, DefinitionFile file)
        {
            UiContainer.Parse(node, file);

            DefinitionParser parser = new DefinitionParser(node);

            file["Items"] = parser.ParseDelegate("Items");
            file["Mode"] = parser.ParseEnum<Mode>("Mode");

            foreach (var cn in node.Nodes)
            {
                switch (cn.Tag)
                {
                    case "UiListBox.ItemTemplate":
                    {
                        if (cn.Nodes.Count != 1)
                        {
                            string error = node.NodeError("UiListBox.ItemTemplate must have exactly 1 child.");

                            if (DefinitionParser.EnableCheckMode)
                            {
                                ConsoleEx.WriteLine(error);
                            }
                            else
                            {
                                throw new Exception(error);
                            }
                        }

                        if (file["Template"] != null)
                        {
                            string error = node.NodeError("UiListBox.ItemTemplate already defined.");

                            if (DefinitionParser.EnableCheckMode)
                            {
                                ConsoleEx.WriteLine(error);
                            }
                            else
                            {
                                throw new Exception(error);
                            }
                        }

                        file["Template"] = DefinitionFile.LoadFile(cn.Nodes[0]);
                    }
                    break;
                }
            }
        }

        enum Mode
        {
            Horizontal,
            Vertical
        }

        DefinitionFile _template;
        IItemsProvider _items = null;
        bool _recalculate = true;
        bool _vertical = false;

        Dictionary<object, UiView> _bindingToElement = new Dictionary<object, UiView>();

        int _updateScrollPosition = 0;

        object _recalcLock = new object();

        Scroller _scroller = null;

        protected override void OnAdded()
        {
            base.OnAdded();
            _scroller = new Scroller(this, _vertical);
        }

        protected override void OnRemoved()
        {
            base.OnRemoved();
            
            _items.Unsubscribe(this);
            _scroller.Remove();
        }

        protected override Rectangle CalculateChildBounds(UiView view)
        {
            lock (_recalcLock)
            {
                _recalculate = true;
            }

            return Rectangle.Empty;
        }

        protected override void Init(object controller, object binding, DefinitionFile definition)
        {
            base.Init(controller, binding, definition);

            DefinitionFileWithStyle file = new DefinitionFileWithStyle(definition, typeof(UiListBox));

            _vertical = DefinitionResolver.Get<Mode>(Controller, Binding, file["Mode"], Mode.Vertical) == Mode.Vertical;

            _template = (DefinitionFile)file["Template"];

            _items = (IItemsProvider)DefinitionResolver.GetValueFromMethodOrField(Controller, Binding, file["Items"]);
            _items.Subscribe(this);
        }

        protected override void Update(float time)
        {
            base.Update(time);

            bool recalculate = false;

            lock (_recalcLock)
            {
                recalculate = _recalculate;
                _recalculate = false;
            }

            if (recalculate)
            {
                lock (_items)
                {
                    int count = _items.Count;

                    _updateScrollPosition = _scroller.ScrollPosition;
                    int position = -_updateScrollPosition;

                    for (int idx = 0; idx < count; ++idx)
                    {
                        object bind = _items.ElementAt(idx);

                        UiView view;
                        _bindingToElement.TryGetValue(bind, out view);

                        if (view == null)
                        {
                            view = (UiView)_template.CreateInstance(Controller, bind);
                            _bindingToElement.Add(bind, view);
                            _children.Add(view);

                            view.Parent = this;
                            view.ViewAdded();
                        }

                        Rectangle bounds = CalculateItemBounds(view);
                        Point size = view.ComputeSize(Bounds.Width, Bounds.Height);

                        if (_vertical)
                        {
                            bounds.Height = size.Y;
                            bounds.Y = position + view.PositionParameters.Margin.Top;

                            position = bounds.Bottom + view.PositionParameters.Margin.Bottom;
                        }
                        else
                        {
                            bounds.Width = size.X;
                            bounds.X = position + view.PositionParameters.Margin.Left;

                            position = bounds.Right + view.PositionParameters.Margin.Right;
                        }

                        view.Bounds = bounds;
                    }

                    _scroller.MaxScroll = position + _updateScrollPosition;
                }
            }
            else
            {
                int scrollPosition = _scroller.ScrollPosition;

                if (scrollPosition != _updateScrollPosition)
                {
                    int diff = _updateScrollPosition - scrollPosition;
                    _updateScrollPosition = scrollPosition;

                    Point offset = new Point(diff, diff);

                    if (_vertical)
                    {
                        offset.X = 0;
                    }
                    else
                    {
                        offset.Y = 0;
                    }

                    for (int idx = 0; idx < _children.Count; ++idx)
                    {
                        var child = _children[idx];
                        child.Move(offset);
                    }
                }
            }

            _scroller.Update(time, Bounds);
        }

        protected override void OnGesture(Gesture gesture)
        {
            _scroller.OnGesture(gesture, ScreenBounds);
        }

        protected override void Draw(ref UiViewDrawParameters parameters)
        {
            float opacity = DisplayOpacity * parameters.Opacity;

            if (opacity == 0)
            {
                return;
            }

            base.Draw(ref parameters);

            UiViewDrawParameters drawParams = parameters;
            drawParams.Opacity = opacity;

            if (_clipChildren)
            {
                parameters.DrawBatch.PushClip(ScreenBounds);
            }

            Rectangle bounds = new Rectangle(0, 0, Bounds.Width, Bounds.Height);

            for (int idx = 0; idx < _children.Count; ++idx)
            {
                var child = _children[idx];

                if (bounds.Intersects(child.Bounds))
                {
                    child.ViewDraw(ref drawParams);
                }
            }

            if (_clipChildren)
            {
                parameters.DrawBatch.PopClip();
            }
        }

        Rectangle CalculateItemBounds(UiView view)
        {
            Point size = view.ComputeSize(Bounds.Width, Bounds.Height);
            PositionParameters pos = view.PositionParameters;

            Rectangle childRect = new Rectangle(0, 0, size.X, size.Y);

            int posX = pos.X.Compute(Bounds.Width);
            int posY = pos.Y.Compute(Bounds.Height);

            if (_vertical)
            {
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

                pos.Margin.RepairRect(ref childRect, Bounds.Width, int.MaxValue);
            }
            else
            {
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
                        childRect.Height = Bounds.Height;
                        break;
                }

                pos.Margin.RepairRect(ref childRect, int.MaxValue, Bounds.Height);
            }

            return childRect;
        }

        void IItemsConsumer.Recalculate()
        {
            lock (_recalcLock)
            {
                _recalculate = true;
            }
        }

        void IItemsConsumer.Added(object item, int index)
        {
            lock (_recalcLock)
            {
                _recalculate = true;
            }
        }

        void IItemsConsumer.Removed(object item)
        {
            lock (_recalcLock)
            {
                _recalculate = true;
            }

            UiTask.BeginInvoke(() =>
                {
                    UiView view;
                    if (_bindingToElement.TryGetValue(item, out view))
                    {
                        _children.Remove(view);
                    }
                });
        }
    }
}
