/// This file is a part of the EBATIANOS.ESSENTIALS class library.
/// (C)2013-2014 Sebastian Sejud. All rights reserved.
///
/// THIS SOURCE FILE IS THE PROPERTY OF SEBASTIAN SEJUD AND IS NOT TO BE 
/// RE-DISTRIBUTED BY ANY MEANS WHATSOEVER WITHOUT THE EXPRESSED WRITTEN 
/// CONSENT OF SEBASTIAN SEJUD.
/// 
/// THIS SOURCE CODE CAN ONLY BE USED UNDER THE TERMS AND CONDITIONS OUTLINED
/// IN THE EBATIANOS.ESSENTIALS LICENSE AGREEMENT. SEBASTIAN SEJUD GRANTS
/// TO YOU (ONE SOFTWARE DEVELOPER) THE LIMITED RIGHT TO USE THIS SOFTWARE 
/// ON A SINGLE COMPUTER.
///
/// CONTACT INFORMATION:
/// essentials@sejud.com
/// sejud.com/essentials-library
/// 
///---------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using Sitana.Framework.Ui.Views.Parameters;
using Microsoft.Xna.Framework;
using Sitana.Framework.Graphics;
using Sitana.Framework.Ui.DefinitionFiles;

namespace Sitana.Framework.Ui.Views
{
    public abstract class UiContainer: UiView
    {
        public new static void Parse(XNode node, DefinitionFile file)
        {
            UiView.Parse(node, file);
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

        private Point _minSizeSet = Point.Zero;

        protected Point _minSizeFromChildren = Point.Zero;

        public override Point MinSize
        {
            get
            {
                return new Point(Math.Max(_minSizeFromChildren.X, _minSizeSet.X), Math.Max(_minSizeFromChildren.Y, _minSizeSet.Y));
            }

            protected set
            {
                _minSizeSet = value;
            }
        }

        protected List<UiView> _children = new List<UiView>();
        Rectangle _bounds = new Rectangle();

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
            return view.PositionParameters.ComputePosition(new Rectangle(0,0,Bounds.Width, Bounds.Height));
        }

        protected override void Draw(ref UiViewDrawParameters parameters)
        {
            if (DisplayOpacity == 0)
            {
                return;
            }

            base.Draw(ref parameters);

            UiViewDrawParameters drawParams = parameters;

            parameters.DrawBatch.PushClip(ScreenBounds);

            for (int idx = 0; idx < _children.Count; ++idx)
            {
                _children[idx].ViewDraw(ref drawParams);
            }

            parameters.DrawBatch.PopClip();
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

        protected override void Init(ref object context, DefinitionFile file)
        {
            base.Init(ref context, file);
        }
    }
}
