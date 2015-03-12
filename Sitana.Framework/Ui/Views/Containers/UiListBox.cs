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
using Sitana.Framework.Ui.Interfaces;

namespace Sitana.Framework.Ui.Views
{
    public class UiListBox : UiContainer, IItemsConsumer, IScrolledElement
    {
        public new static void Parse(XNode node, DefinitionFile file)
        {
            UiContainer.Parse(node, file);

            DefinitionParser parser = new DefinitionParser(node);
            
            file["Items"] = parser.ParseDelegate("Items");
            file["Mode"] = parser.ParseEnum<Mode>("Mode");
            file["Reverse"] = parser.ParseBoolean("Reverse");
            file["ExceedRule"] = parser.ParseEnum<ScrollingService.ExceedRule>("ExceedRule");
			file["WheelScrollSpeed"] = parser.ParseDouble("WheelScrollSpeed");

            Dictionary<Type, DefinitionFile> additionalTemplates = new Dictionary<Type, DefinitionFile>();

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

                        if(string.IsNullOrEmpty(cn.Attribute("DataType")))
                        { 
                            if (file["Template"] != null)
                            {
                                string error = node.NodeError("UiListBox default template already defined.");

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
                        else
                        {
                            Type type = Type.GetType(cn.Attribute("DataType"));

                            if(type == null)
                            {
                                string error = node.NodeError("Cannot find type: {0}", cn.Attribute("DataType"));

                                if (DefinitionParser.EnableCheckMode)
                                {
                                    ConsoleEx.WriteLine(error);
                                }
                                else
                                {
                                    throw new Exception(error);
                                }
                            }

                            additionalTemplates.Add(type, DefinitionFile.LoadFile(cn.Nodes[0]));
                        }
                    }
                    break;
                }
            }

            if (additionalTemplates.Count > 0)
            {
                file["AdditionalTemplates"] = additionalTemplates;
            }
        }

        enum Mode
        {
            Horizontal,
            Vertical
        }

        DefinitionFile _template;
        Dictionary<Type, DefinitionFile> _additionalTemplates;

        IItemsProvider _items = null;
        bool _recalculate = true;
        bool _vertical = false;

        object _childrenLock = new object();

        Dictionary<object, UiView> _bindingToElement = new Dictionary<object, UiView>();

        Point _updateScrollPosition = Point.Zero;

        object _recalcLock = new object();

        Scroller _scroller = null;
        ScrollingService _scrollingService;

        Point _maxScroll = Point.Zero;
        ScrollingService.ExceedRule _rule = ScrollingService.ExceedRule.Allow;

        bool _reverse = false;
		float _wheelSpeed = 0;

        int _maxAddOneTime = 32;
        bool _clearChildren = false;

        public ScrollingService ScrollingService
        {
            get
            {
                return _scrollingService;
            }
        }

        protected override void OnAdded()
        {
			Scroller.Mode mode = (_vertical ? Scroller.Mode.VerticalDrag | Scroller.Mode.VerticalWheel : Scroller.Mode.HorizontalDrag | Scroller.Mode.HorizontalWheel);

            _scrollingService = new ScrollingService(this, _rule);
			_scroller = new Scroller(this, mode, _scrollingService, _wheelSpeed );

            base.OnAdded();
        }

        protected override void OnRemoved()
        {
            base.OnRemoved();
            
            _items.Unsubscribe(this);
            _scrollingService.Remove();
        }

        protected override Rectangle CalculateChildBounds(UiView view)
        {
            lock (_recalcLock)
            {
                _recalculate = true;
            }

            return view.Bounds;
        }

        protected override bool Init(object controller, object binding, DefinitionFile definition)
        {
            if (!base.Init(controller, binding, definition))
            {
                return false;
            }

            DefinitionFileWithStyle file = new DefinitionFileWithStyle(definition, typeof(UiListBox));

            _vertical = DefinitionResolver.Get<Mode>(Controller, Binding, file["Mode"], Mode.Vertical) == Mode.Vertical;

            _template = (DefinitionFile)file["Template"];

            _items = (IItemsProvider)DefinitionResolver.GetValueFromMethodOrField(Controller, Binding, file["Items"]);
            _items.Subscribe(this);

            _rule = DefinitionResolver.Get<ScrollingService.ExceedRule>(Controller, Binding, file["ExceedRule"], ScrollingService.ExceedRule.Allow);

            _reverse = DefinitionResolver.Get<bool>(Controller, Binding, file["Reverse"], false);

			_wheelSpeed = (float)DefinitionResolver.Get<double>(Controller, Binding, file["WheelScrollSpeed"], 0);

            _additionalTemplates = file["AdditionalTemplates"] as Dictionary<Type, DefinitionFile>;

            return true;
        }

        void Recalculate()
        {
            lock (_items)
            {
                int count = _items.Count;

                int added = 0;

                _updateScrollPosition = new Point((int)_scrollingService.ScrollPositionX, (int)_scrollingService.ScrollPositionY);
                Point position = new Point(-_updateScrollPosition.X, -_updateScrollPosition.Y);

                for (int idx = 0; idx < count; ++idx)
                {
                    object bind = _items.ElementAt(_reverse ? count - idx - 1 : idx);

                    UiView view;
                    _bindingToElement.TryGetValue(bind, out view);

                    if (view == null)
                    {
                        if (added < _maxAddOneTime)
                        {
                            DefinitionFile template;

                            if (_additionalTemplates == null || !_additionalTemplates.TryGetValue(bind.GetType(), out template))
                            {
                                template = _template;
                            }

                            view = (UiView)template.CreateInstance(Controller, bind);

                            lock (_childrenLock)
                            {
                                _bindingToElement.Add(bind, view);
                                _children.Add(view);
                            }

                            view.Parent = this;
                            view.RegisterView();
                            view.ViewAdded();
                            added++;
                        }
                        else
                        {
                            continue;
                        }
                    }

                    Rectangle bounds = CalculateItemBounds(view);
                    Point size = view.ComputeSize(Bounds.Width, Bounds.Height);

                    if (_vertical)
                    {
                        bounds.Height = size.Y;
                        bounds.Y = position.Y + view.PositionParameters.Margin.Top;

                        position.Y = bounds.Bottom + view.PositionParameters.Margin.Bottom;
                    }
                    else
                    {
                        bounds.Width = size.X;
                        bounds.X = position.X + view.PositionParameters.Margin.Left;

                        position.X = bounds.Right + view.PositionParameters.Margin.Right;
                    }

                    view.Bounds = bounds;
                }

                _maxScroll = new Point(_updateScrollPosition.X + position.X, _updateScrollPosition.Y + position.Y);
            }
        }

        protected override void Update(float time)
        {
            Rectangle listBounds = new Rectangle(0, 0, Bounds.Width, Bounds.Height);

            for (int idx = 0; idx < _children.Count; ++idx)
            {
                var child = _children[idx];

                if (listBounds.Intersects(child.Bounds))
                {
                    child.ViewUpdate(time);
                }
            }

            bool recalculate = false;
            bool clearChildren = false;

            lock (_recalcLock)
            {
                recalculate = _recalculate;
                _recalculate = false;

                clearChildren = _clearChildren;
                _clearChildren = false;
            }

            if (clearChildren)
            {
                _children.Clear();
            }

            lock (_items)
            {
                if (_children.Count != _items.Count)
                {
                    recalculate = true;
                }
            }

            if (recalculate)
            {
                Recalculate();
            }
            else
            {
                Point scrollPosition = new Point((int)_scrollingService.ScrollPositionX, (int)_scrollingService.ScrollPositionY);

                if (scrollPosition != _updateScrollPosition)
                {
                    Point diff = new Point(_updateScrollPosition.X - scrollPosition.X, _updateScrollPosition.Y - scrollPosition.Y);
                    _updateScrollPosition = scrollPosition;

                    for (int idx = 0; idx < _children.Count; ++idx)
                    {
                        var child = _children[idx];
                        child.Move(diff);
                    }
                }
            }
        }

        protected override void OnGesture(Gesture gesture)
        {
            _scroller.OnGesture(gesture);
        }

        protected override void Draw(ref UiViewDrawParameters parameters)
        {
            float opacity = parameters.Opacity;

            if (opacity == 0)
            {
                return;
            }

            DrawBackground(ref parameters);

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

        public override void RecalcLayout()
        {
            base.RecalcLayout();

            lock (_recalcLock)
            {
                _recalculate = true;
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
                switch (pos.HorizontalAlignment)
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

                pos.Margin.RepairRect(ref childRect, Bounds.Width, int.MaxValue);
            }
            else
            {
                switch (pos.VerticalAlignment)
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

                pos.Margin.RepairRect(ref childRect, int.MaxValue, Bounds.Height);
            }

            return childRect;
        }

        public void Show(object item)
        {
            UiView view;
            if(_bindingToElement.TryGetValue(item, out view))
            {
                if(_vertical)
                {
                    if(view.Bounds.Bottom > Bounds.Height)
                    {
                        _scrollingService.ScrollPositionY += view.Bounds.Bottom - Bounds.Height;
                    }

                    if (view.Bounds.Top < 0)
                    {
                        _scrollingService.ScrollPositionY += view.Bounds.Top;
                    }
                    
                    _scrollingService.Process();
                }
                else
                {
                    if (view.Bounds.Right > Bounds.Width)
                    {
                        _scrollingService.ScrollPositionX += view.Bounds.Right - Bounds.Width;
                    }

                    if (view.Bounds.Left < 0)
                    {
                        _scrollingService.ScrollPositionX += view.Bounds.Left;
                    }

                    _scrollingService.Process();
                }
            }
        }

        public Rectangle BoundsOf(object item)
        {
            UiView view;
            if(_bindingToElement.TryGetValue(item, out view))
            {
                return view.ScreenBounds;
            }

            throw new Exception("List doesn't contain requested element!");
        }

        public T FirstVisible<T>() where T: class
        {
            Rectangle bounds = new Rectangle(0, 0, Bounds.Width, Bounds.Height);

            for(int idx = 0; idx < _children.Count; ++idx)
            {
                if(_children[idx].Bounds.Intersects(bounds))
                {
                    return _children[idx].Binding as T;
                }
            }

            return null;
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

        void IItemsConsumer.RemovedAll()
        {
            lock(_childrenLock)
            {
                _bindingToElement.Clear();
            }

            lock (_recalcLock)
            {
                _clearChildren = true;
                _recalculate = true;
            }
        }

        void IItemsConsumer.Removed(object item)
        {
            lock (_recalcLock)
            {
                _recalculate = true;
            }

            UiView view = null;
            lock(_childrenLock)
            {
                if (_bindingToElement.TryGetValue(item, out view))
                {
                    _bindingToElement.Remove(item);
                }
            }

            if(view != null)
            {
                UiTask.BeginInvoke(() => _children.Remove(view));
            }
        }

        Rectangle IScrolledElement.ScreenBounds { get{ return ScreenBounds;} }

        int IScrolledElement.MaxScrollX { get{return _maxScroll.X;} }
        int IScrolledElement.MaxScrollY { get{return _maxScroll.Y;} }

        ScrollingService IScrolledElement.ScrollingService {get{ return _scrollingService;}}
    }
}
