/// This file is a part of the EBATIANOS.ESSENTIALS class library.
/// (c)2013-2014 EBATIANO'S a.k.a. Sebastian Sejud. All rights reserved.
///
/// THIS SOURCE FILE IS THE PROPERTY OF EBATIANO'S A.K.A. SEBASTIAN SEJUD 
/// AND IS NOT TO BE RE-DISTRIBUTED BY ANY MEANS WHATSOEVER WITHOUT 
/// THE EXPRESSED WRITTEN CONSENT OF EBATIANO'S A.K.A. SEBASTIAN SEJUD.
///
/// THIS SOURCE CODE CAN ONLY BE USED UNDER THE TERMS AND CONDITIONS OUTLINED
/// IN THE EBATIANOS.ESSENTIALS LICENSE AGREEMENT. 
/// EBATIANO'S A.K.A. SEBASTIAN SEJUD GRANTS TO YOU (ONE SOFTWARE DEVELOPER) 
/// THE LIMITED RIGHT TO USE THIS SOFTWARE ON A SINGLE COMPUTER.
///
/// CONTACT INFORMATION:
/// contact@ebatianos.com
/// www.ebatianos.com/essentials-library
/// 
///---------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using Ebatianos;
using Ebatianos.Gui;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input.Touch;

namespace Ebatianos.Gui.List
{
    internal class ListItem
    {
        private List<ListItemElement> _elements = new List<ListItemElement>();
        private List<ListItemElement> _expandedElements = new List<ListItemElement>();
        private Int32 _height = 0;
        private Int32 _expandedHeight = 0;

        private Single _expandedMultiplier = 0;
        private Boolean _expanded;

        public ListItemData Bind { get; private set; }
        public Boolean IsExpanded
        {
            get
            {
                return _expanded;
            }

            set
            {
                _expanded = value;

                if (Bind != null)
                {
                    Bind.IsExpanded = value;
                }
            }
        }

        public Int32 CollapsedHeight
        {
            get
            {
                return _height;
            }
        }

        public Int32 DrawHeight
        {
            get
            {
                return (Int32)(_height + _expandedHeight * _expandedMultiplier);
            }
        }

        public Int32 Height
        {
            get
            {
                return _expanded ? _height + _expandedHeight : _height;
            }
        }

        public ListItem(XmlFileNode templateNode, Int32 listHeight)
        {
            ParametersCollection parameters = templateNode.Attributes;

            _height = parameters.AsInt32("ItemHeight");

            if ( _height == 0 )
            {
                Int32 visibleElements = parameters.AsInt32("VisibleItems");

                if (visibleElements > 0)
                {
                    _height = listHeight / visibleElements;
                }
            }

            _expandedHeight = parameters.AsInt32("ExtendedHeight");

            for (Int32 idx = 0; idx < templateNode.Nodes.Count; ++idx )
            {
                XmlFileNode node = templateNode.Nodes[idx];

                // Get class name.
                String className = node.Tag;

                ListItemElement element = ExpandableList.Create(className);
                element.Init(node);

                if (node.Attributes.AsBoolean("Extended"))
                {
                    _expandedElements.Add(element);
                }
                else
                {
                    _elements.Add(element);
                }
            }
        }

        private ListItem(ListItem other, ListItemData bind)
        {
            foreach (var element in other._elements)
            {
                var el = element.Clone(bind);

                if (el != null)
                {
                    _elements.Add(el);
                }
            }

            foreach (var element in other._expandedElements)
            {
                var el = element.Clone(bind);

                if (el != null)
                {
                    _expandedElements.Add(el);
                }
            }

            _height = other._height;
            _expandedHeight = other._expandedHeight;

            IsExpanded = bind.IsExpanded;

            if (IsExpanded)
            {
                _expandedMultiplier = 1;
            }

            Bind = bind;
            IsExpanded = bind.IsExpanded;

            UpdateHeights();
        }

        private void UpdateHeights()
        {
            if (_height <= 0)
            {
                _height = -_height;

                foreach (var element in _elements)
                {
                    _height = Math.Max(_height, element.Bottom);
                }
            }

            if (_expandedHeight <= 0)
            {
                _expandedHeight = -_expandedHeight;

                foreach (var element in _expandedElements)
                {
                    _expandedHeight = Math.Max(_expandedHeight, element.Bottom);
                }
            }
        }

        public ListItem Clone(ListItemData bind)
        {
            return new ListItem(this, bind);
        }

        public Boolean OnGesture(GestureType type, Vector2 position, params Object[] parameters)
        {
            for (Int32 idx = _elements.Count-1; idx >= 0; --idx)
            {
                if (_elements[idx].OnGesture(type, position, parameters))
                {
                    return true;
                }
            }

            position -= new Vector2(0, _height);

            for (Int32 idx = _expandedElements.Count-1; idx >= 0; --idx)
            {
                if (_expandedElements[idx].OnGesture(type, position, parameters))
                {
                    return true;
                }
            }

            return false;
        }

        public Boolean UpdateUi(Single time)
        {
            Boolean change = UpdateHeight(time);

            for (Int32 idx = 0; idx < _elements.Count; ++idx)
            {
                change |= _elements[idx].UpdateUi(time);
            }
            
            for (Int32 idx = 0; idx < _expandedElements.Count; ++idx)
            {
                change |= _expandedElements[idx].UpdateUi(time);
            }
            
            return change;
        }

        public Boolean Update()
        {
            Boolean change = false;

            for (Int32 idx = 0; idx < _elements.Count; ++idx)
            {
                change |= _elements[idx].Update();
            }

            if (IsExpanded)
            {
                for (Int32 idx = 0; idx < _expandedElements.Count; ++idx)
                {
                    change |= _expandedElements[idx].Update();
                }
            }

            return change;
        }

        public void Draw(SpriteBatch spriteBatch, Vector2 position, Single scale, Single opacity, Boolean lastValue)
        {
            for (Int32 idx = 0; idx < _elements.Count; ++idx)
            {
                ListItemElement element = _elements[idx];

                if ((!lastValue && Bind.ShowSeparator) || !(element is ListItem_Separator))
                {
                    element.Draw(spriteBatch, position, scale, DrawHeight, 1, opacity);
                }
            }

            if (_expandedMultiplier > 0)
            {
                if (_expandedMultiplier < 1)
                {
                    Vector2 pos = position + new Vector2(0, _height) * scale;

                    Rectangle clip = new Rectangle(0, (Int32)position.Y, spriteBatch.GraphicsDevice.Viewport.Width, (Int32)(_expandedMultiplier * _expandedHeight * scale + _height * scale));

                    Rectangle clip2 = spriteBatch.GraphicsDevice.ScissorRectangle;
                    clip = GraphicsHelper.IntersectRectangle(ref clip2, ref clip);

                    ScreenManager.Current.AdditionalDraw(() =>
                    {
                        for (Int32 idx = 0; idx < _expandedElements.Count; ++idx)
                        {
                            ListItemElement element = _expandedElements[idx];
                            element.Draw(spriteBatch, pos, scale, _expandedHeight, _expandedMultiplier, opacity);
                        }
                    }, clip);
                }
                else
                {
                    position += new Vector2(0, _height) * scale;

                    for (Int32 idx = 0; idx < _expandedElements.Count; ++idx)
                    {
                        ListItemElement element = _expandedElements[idx];
                        element.Draw(spriteBatch, position, scale, _expandedHeight, _expandedMultiplier, opacity);
                    }
                }
            }
        }

        private Boolean UpdateHeight(Single time)
        {
            if (IsExpanded)
            {
                if (_expandedMultiplier < 1)
                {
                    _expandedMultiplier += time * 5;
                    _expandedMultiplier = Math.Min(1, _expandedMultiplier);
                    return true;
                }
            }
            else
            {
                if (_expandedMultiplier > 0)
                {
                    _expandedMultiplier -= time * 5;
                    _expandedMultiplier = Math.Max(0, _expandedMultiplier);
                    return true;
                }
            }

            return false;
        }
    }
}
