// SITANA - Copyright (C) The Sitana Team.
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using System.Collections.Generic;
using Sitana.Framework.Ui.Views.Parameters;
using Microsoft.Xna.Framework;
using Sitana.Framework.Ui.DefinitionFiles;
using Sitana.Framework.Ui.Controllers;
using Sitana.Framework.Xml;
using Sitana.Framework.Input.TouchPad;

namespace Sitana.Framework.Ui.Views
{
    public abstract class UiContainer: UiView
    {
        public new static void Parse(XNode node, DefinitionFile file)
        {
            UiView.Parse(node, file);

            var parser = new DefinitionParser(node);
            file["ClipChildren"] = parser.ParseBoolean("ClipChildren");

            List<DefinitionFile> list = new List<DefinitionFile>();

            foreach (var cn in node.Nodes)
            {
                if (!cn.Tag.Contains("."))
                {
                    list.Add(DefinitionFile.LoadFile(cn));
                }
            }

            if (list.Count > 0)
            {
                file["Children"] = list;
            }
        }

        protected bool _clipChildren = false;

        protected Point _minSizeFromChildren = Point.Zero;

        protected bool _added = false;

        public bool ClipChildren
        {
            get
            {
                return _clipChildren;
            }
        }

        public override Point MinSize
        {
            get
            {
                return new Point(Math.Max(_minSizeFromChildren.X, _minWidth.Compute()), Math.Max(_minSizeFromChildren.Y, _minHeight.Compute()));
            }
        }

        protected List<UiView> _children = new List<UiView>();

        public bool HasChildren
        {
            get
            {
                return _children.Count > 0;
            }
        }

        public void Remove(UiView view)
        {
            _children.Remove(view);
            view.ViewRemoved();
            OnChildrenModified();
        }

        public virtual void Add(UiView view)
        {
            if (!_children.Contains(view))
            {
                _children.Add(view);
                view.Bounds = CalculateChildBounds(view);
                view.Parent = this;
                view.RegisterView();

                if (_added)
                {
                    view.ViewAdded();
                }

                OnChildrenModified();
            }
        }

        public void RecalculateAll()
        {
            for (Int32 idx = 0; idx < _children.Count; ++idx)
            {
                var child = _children[idx] as UiContainer;

                if (child != null)
                {
                    child.RecalculateAll();
                }
            }

            OnChildrenModified();
            RecalcLayout();
        }

        protected override void OnAdded()
        {
            _added = true;

            for (Int32 idx = 0; idx < _children.Count; ++idx)
            {
                _children[idx].ViewAdded();
            }
        }

        public virtual void RecalcLayout()
        {
            for (Int32 idx = 0; idx < _children.Count; ++idx)
            {
                var child = _children[idx];
                child.Bounds = CalculateChildBounds(child);
            }
        }

        public virtual void RecalcLayout(UiView view)
        {
            view.Bounds = CalculateChildBounds(view);
        }

        public override Rectangle Bounds
        {
            get
            {
                return _bounds;
            }

            set
            {
                bool shouldRecalculate = false;

                if (_bounds.Width != value.Width || _bounds.Height != value.Height)
                {
                    shouldRecalculate = true;
                }

                _bounds = value;
                _enableGestureHandling = false;

                if (shouldRecalculate)
                {
                    RecalcLayout();
                }
            }
        }

        public override void Move(Point offset)
        {
            _bounds = new Rectangle(_bounds.X + offset.X, _bounds.Y + offset.Y, _bounds.Width, _bounds.Height);
        }

        protected virtual Rectangle CalculateChildBounds(UiView view)
        {
            return view.PositionParameters.Margin.ComputeRect(new Rectangle(0,0,Bounds.Width, Bounds.Height));
        }

        protected override void Draw(ref UiViewDrawParameters parameters)
        {
            float opacity = parameters.Opacity;

            if (opacity == 0 )
            {
                return;
            }

            Color backgroundColor = BackgroundColor * opacity;

            if (backgroundColor.A > 0)
            {
                parameters.DrawBatch.DrawRectangle(ScreenBounds, backgroundColor);
            }

            UiViewDrawParameters drawParams = parameters;
            drawParams.Opacity = opacity;

            if (_clipChildren)
            {
                parameters.DrawBatch.PushClip(ScreenBounds);
            }

            for (int idx = 0; idx < _children.Count; ++idx)
            {
                _children[idx].ViewDraw(ref drawParams);   
            }

            if (_clipChildren)
            {
                parameters.DrawBatch.PopClip();
            }
        }

        protected override void OnRemoved()
        {
            for (int idx = 0; idx < _children.Count; ++idx)
            {
                _children[idx].ViewRemoved();
            }
        }

        protected override void Update(float time)
        {
            for (int idx = 0; idx < _children.Count; ++idx)
            {
                _children[idx].ViewUpdate(time);
            }
        }

        protected virtual void OnChildrenModified()
        {
        }

        public UiView ViewFromPoint(Vector2 point)
        {
            return ViewFromPoint(new Point((int)point.X, (int)point.Y));
        }

        public UiView ViewFromPoint(Point point)
        {
            for (Int32 idx = 0; idx < _children.Count; ++idx)
            {
                var child = _children[idx];

                if (child.ScreenBounds.Contains(point))
                {
                    if (child is UiContainer)
                    {
                        return (child as UiContainer).ViewFromPoint(point);
                    }

                    return child;
                }
            }

            return this;
        }

        public UiView ViewFromId(string id)
        {
            for (Int32 idx = 0; idx < _children.Count; ++idx)
            {
                if ( _children[idx].Id == id)
                {
                    return _children[idx];
                }
            }

            return null;
        }

        protected override bool Init(object controller, object binding, DefinitionFile definition)
        {
            if (!base.Init(controller, binding, definition))
            {
                return false;
            }

            var file = new DefinitionFileWithStyle(definition, typeof(UiContainer));

            _clipChildren = DefinitionResolver.Get<bool>(Controller, Binding, file["ClipChildren"], false);
            return true;
        }

        protected void InitChildren(UiController controller, object binding, DefinitionFile definition)
        {
            InitChildren(controller, binding, definition, typeof(PositionParameters));
        }

        protected void InitChildren(UiController controller, object binding, DefinitionFile definition, Type positionParametersType)
        {
            DefinitionFileWithStyle file = new DefinitionFileWithStyle(definition, typeof(UiContainer));

            List<DefinitionFile> children = file["Children"] as List<DefinitionFile>;

            if (children != null)
            {
                for (int idx = 0; idx < children.Count; ++idx)
                {
                    var childFile = children[idx];
                    var child = childFile.CreateInstance(controller, binding) as UiView;

                    if (child != null)
                    {
                        Add(child);
                    }
                }
            }
        }

        internal override void ViewGesture(Gesture gesture)
        {
            if (_enableGestureHandling || gesture.GestureType == GestureType.CapturedByOther)
            {
                for (int idx = _children.Count - 1; idx >= 0; --idx)
                {
                    _children[idx].ViewGesture(gesture);

                    if ((gesture.Handled || gesture.SkipRest) && (gesture.GestureType != GestureType.CapturedByOther))
                    {
                        break;
                    }
                }
            }

            if ((!gesture.Handled && !gesture.SkipRest) || gesture.GestureType == GestureType.CapturedByOther)
            {
                base.ViewGesture(gesture);
            }
        }
    }
}
