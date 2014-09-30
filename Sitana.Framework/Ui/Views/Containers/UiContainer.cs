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
using Sitana.Framework.Essentials.Ui.DefinitionFiles;

namespace Sitana.Framework.Ui.Views
{
    public abstract class UiContainer: UiView
    {
        public new static void Parse(XNode node, DefinitionFile file)
        {
            UiView.Parse(node, file);

            var parser = new DefinitionParser(node);
            file["ClipChildren"] = parser.ParseBoolean("ClipChildren");
        }

        protected static void ParseChildren(XNode node, DefinitionFile file)
        {
            List<DefinitionFile> list = new List<DefinitionFile>();

            for ( int idx = 0; idx < node.Nodes.Count; ++idx )
            {
                var childNode = node.Nodes[idx];
                list.Add(DefinitionFile.LoadFile(childNode));
            }

            file["Children"] = list;
        }

        private bool _clipChildren = false;

        protected Point _minSizeFromChildren = Point.Zero;

        public override Point MinSize
        {
            get
            {
                return new Point(Math.Max(_minSizeFromChildren.X, _minWidth.Compute()), Math.Max(_minSizeFromChildren.Y, _minHeight.Compute()));
            }
        }

        protected List<UiView> _children = new List<UiView>();
        protected Rectangle _bounds = new Rectangle();

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

                view.ViewAdded();
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

        public void RecalcLayout()
        {
            for (Int32 idx = 0; idx < _children.Count; ++idx)
            {
                var child = _children[idx];
                child.Bounds = CalculateChildBounds(child);
            }
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
            }
        }

        protected virtual Rectangle CalculateChildBounds(UiView view)
        {
            return view.PositionParameters.Margin.ComputeRect(new Rectangle(0,0,Bounds.Width, Bounds.Height));
        }

        protected override void Draw(ref UiViewDrawParameters parameters)
        {
            if (DisplayOpacity == 0)
            {
                return;
            }

            base.Draw(ref parameters);

            UiViewDrawParameters drawParams = parameters;

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

        public virtual void UpdateChildBounds(UiView view, Rectangle bounds)
        {
        }

        protected override void Init(object controller, object binding, DefinitionFile definition)
        {
            base.Init(controller, binding, definition);

            var file = new DefinitionFileWithStyle(definition, typeof(UiContainer));

            _clipChildren = DefinitionResolver.Get<bool>(Controller, binding, file["ClipChildren"], false);
        }

        protected void InitChildren(UiController controller, object binding, DefinitionFile definition)
        {
            InitChildren(controller, binding, definition, typeof(PositionParameters));
        }

        protected void InitChildren(UiController controller, object binding, DefinitionFile definition, Type positionParametersType)
        {
            DefinitionFileWithStyle file = new DefinitionFileWithStyle(definition, typeof(UiSplitterView));

            List<DefinitionFile> children = file["Children"] as List<DefinitionFile>;

            if (children != null)
            {
                for (int idx = 0; idx < children.Count; ++idx)
                {
                    var childFile = children[idx];
                    var child = childFile.CreateInstance(controller, binding) as UiView;
                    child.CreatePositionParameters(controller, binding, childFile, positionParametersType);

                    if (child != null)
                    {
                        Add(child);
                    }
                }
            }
        }
    }
}
