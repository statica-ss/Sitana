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
using System.Collections.ObjectModel;

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

        protected bool ProcessGestureBeforeChildren = false;
        protected bool _shouldRecalcLayout = false;

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

        public IReadOnlyCollection<UiView> Children { get; private set; }

        public bool HasChildren
        {
            get
            {
                return _children.Count > 0;
            }
        }

        public UiContainer()
        {
            Children = new ReadOnlyCollection<UiView>(_children);
        }

        public virtual void Remove(UiView view)
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
                view.Parent = this;
                view.RegisterView();
                view.Bounds = CalculateChildBounds(view);
                
                if (_added)
                {
                    view.ViewAdded();
                }

                OnChildrenModified();
            }
        }

        public virtual void Insert(int index, UiView view)
        {
            if (!_children.Contains(view))
            {
                _children.Insert(index, view);
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
            for (int idx = 0; idx < _children.Count; ++idx)
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

            for (int idx = 0; idx < _children.Count; ++idx)
            {
                _children[idx].ViewAdded();
            }
        }

        public virtual void RecalcLayout()
        {
            for (int idx = 0; idx < _children.Count; ++idx)
            {
                var child = _children[idx];
                child.Bounds = CalculateChildBounds(child);
            }

            if(_shouldRecalcLayout)
            {
                _shouldRecalcLayout = false;
                RecalcLayout();
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
                _bounds = value;

                RecalcLayout();
                InvalidateScreenBounds();
            }
        }

        public override void Move(Point offset)
        {
            Bounds = new Rectangle(_bounds.X + offset.X, _bounds.Y + offset.Y, _bounds.Width, _bounds.Height);
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

            DrawBackground(ref parameters);

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

        internal override void ResetViewDisplayed()
        {
            base.ResetViewDisplayed();

            for (int idx = 0; idx < _children.Count; ++idx)
            {
                _children[idx].ResetViewDisplayed();
            }
        }

        internal override void ProcessAfterDraw()
        {
            base.ProcessAfterDraw();

            for (int idx = 0; idx < _children.Count; ++idx)
            {
                _children[idx].ProcessAfterDraw();
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
            if (_shouldRecalcLayout)
            {
                _shouldRecalcLayout = false;
                RecalcLayout();
            }

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
            for (int idx = 0; idx < _children.Count; ++idx)
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
            for (int idx = 0; idx < _children.Count; ++idx)
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

            for (int idx = 0; idx < _children.Count; ++idx)
            {
                _children[idx].OnNeighboursInited();
            }
        }

        internal UiView FindChild(string id)
        {
            UiView found = _children.Find((v) => v.Id == id);

            if(found == null)
            {
                foreach(var ch in _children)
                {
                    if(ch is UiContainer)
                    {
                        found = (ch as UiContainer).FindChild(id);

                        if(found != null)
                        {
                            break;
                        }
                    }
                }
            }

            return found;
        }

        public void ShouldRecalcLayout()
        {
            _shouldRecalcLayout = true;
        }

        internal override void ViewGesture(Gesture gesture)
        {
            if (ProcessGestureBeforeChildren)
            {
                base.ViewGesture(gesture);
            }

            if ((_isViewDisplayed && !gesture.Handled && !gesture.SkipRest) || gesture.GestureType == GestureType.CapturedByOther)
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

            if (!ProcessGestureBeforeChildren)
            {
                if ((!gesture.Handled && !gesture.SkipRest) || gesture.GestureType == GestureType.CapturedByOther)
                {
                    base.ViewGesture(gesture);
                }
            }
        }
    }
}
