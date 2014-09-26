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
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Sitana.Framework;
using System.Xml;
using Sitana.Framework.Content;
using System.IO;
using Microsoft.Xna.Framework.Input.Touch;
using Sitana.Framework.Input;
using System.Reflection;

namespace Sitana.Framework.Gui.List
{
    public class ListItem_SlideContainer: ListItemElement
    {
        private Int32   _currentSelection = 0;
        private Int32   _nextSelection = 0;
        private Single  _slide = 0;
        private Single  _dragSize = 0;
        private Int32   _width;

        private Int32   _buttonsMargin;
        private Int32   _buttonPos;

        private Texture2D   _nextButton;
        private Texture2D   _prevButton;

        private Rectangle  _nextButtonRect;
        private Rectangle  _prevButtonRect;

        private Color      _buttonColor;
        private Color      _buttonColorPushed;

        private Int32      _buttonSelected = 0;

        private String      _positionPropertyName;
        private PropertyInfo _positionProperty;

        private List<List<ListItemElement>> _elements = new List<List<ListItemElement>>();

        private String _showPanelsPropertyName;

        private Boolean[] _showPanels;

        public override void Init(XmlFileNode node)
        {
            ParametersCollection parameters = node.Attributes;

            _buttonsMargin = parameters.AsInt32("ButtonsMargin");
            _buttonPos = parameters.AsInt32("ButtonsPosition");

            if ( parameters.HasKey("NextIcon"))
            {
                _nextButton = ContentLoader.Current.Load<Texture2D>( parameters.AsString("NextIcon"));
            }

            if (parameters.HasKey("PrevIcon"))
            {
                _prevButton = ContentLoader.Current.Load<Texture2D>(parameters.AsString("PrevIcon"));
            }

            _buttonColor = parameters.AsColor("ButtonColor");
            _buttonColorPushed = parameters.AsColor("ButtonColorPushed");

            _positionPropertyName = parameters.AsString("Position").Substring(1);

            _showPanelsPropertyName = parameters.AsString("ShowPanelsArray").Substring(1);

            for ( Int32 idx = 0; idx < node.Nodes.Count; ++idx )
            {
                XmlFileNode tempNode = node.Nodes[idx];

                if ( tempNode.Tag == "ElementTemplate")
                {
                    ReadElements(tempNode);
                    return;
                }
            }

            String path = parameters.AsString("Template");
            
            XmlFileNode templateNode = ContentLoader.Current.Load<XmlFile>(path);
                    
            if ( templateNode.Tag != "Template" )
            {
                throw new Exception("Invalid xml node. Expected: Template");
            }

            templateNode.ValueSource = node.Attributes.ValueSource;
            templateNode.ColorsManager = node.Attributes.ColorsManager;

            ReadElements(templateNode.Nodes[0]);
        }

        private void ReadElements(XmlFileNode elementsNode)
        {
            if ( elementsNode.Tag != "ElementTemplate")
            {
                throw new Exception("Invalid xml node. Expected: ElementTemplate");
            }

            ParametersCollection parameters = elementsNode.Attributes;
            _width = parameters.AsInt32("Width");

            // Read GuiElements.
            for (Int32 idx = 0; idx < elementsNode.Nodes.Count; ++idx )
            {
                XmlFileNode node = elementsNode.Nodes[idx];

                // Get class name.
                String className = node.Tag;

                ListItemElement element = ExpandableList.Create(className);
                element.Init(node);

                Int32 column = node.Attributes.AsInt32("Column");

                while (_elements.Count <= column)
                {
                    _elements.Add(new List<ListItemElement>());
                }

                _elements[column].Add(element);
            }
        }

        public override ListItemElement Clone(Object bind)
        {
            ListItem_SlideContainer container = new ListItem_SlideContainer();

            for (Int32 idx = 0; idx < _elements.Count; ++idx)
            {
                var elements = _elements[idx];
                var elements2 = new List<ListItemElement>();

                container._elements.Add(elements2);

                for (Int32 idx2 = 0; idx2 < elements.Count; ++idx2)
                { 
                    elements2.Add(elements[idx2].Clone(bind));
                }
            }

            container._width = _width;

            container._buttonColor = _buttonColor;
            container._buttonColorPushed = _buttonColorPushed;
            container._nextButton = _nextButton;
            container._prevButton = _prevButton;
            container._buttonsMargin = _buttonsMargin;
            container._buttonPos = _buttonPos;

            PropertyInfo positionBind = bind.GetType().GetProperty(_positionPropertyName);

            container._positionProperty = positionBind;
            container._currentSelection = (positionBind == null) ? 0 : (Int32)positionBind.GetValue(bind, null);

            PropertyInfo panelsVisiblityBind = bind.GetType().GetProperty(_showPanelsPropertyName);

            container._showPanels = panelsVisiblityBind == null ? null : (Boolean[])panelsVisiblityBind.GetValue(bind, null);

            container.Bind = bind;
            return container;
        }

        public void ShowNext()
        {
            _slide = 0.0001f;

            Boolean move = true;

            Int32 column = _currentSelection;

            while(move)
            {
                column = (column + 1) % _elements.Count;

                if ( _showPanels != null )
                {
                    if (!_showPanels[column])
                    {
                        continue;
                    }
                }

                _nextSelection = column;

                move = false;
                var elements = _elements[column];

                for (Int32 idx = 0; idx < elements.Count; ++idx)
                {
                    elements[idx].Update();
                }
            }
        }

        public void ShowPrevious()
        {
            _slide = -0.0001f;

            Boolean move = true;

            Int32 column = _currentSelection;

            while (move)
            {
                column = (column - 1 + _elements.Count) % _elements.Count;

                if (_showPanels != null)
                {
                    if (!_showPanels[column])
                    {
                        continue;
                    }
                }

                _nextSelection = column;

                move = false;
                var elements = _elements[column];

                for (Int32 idx = 0; idx < elements.Count; ++idx)
                {
                    elements[idx].Update();
                }
            }
        }

        public override Boolean UpdateUi(Single time)
        {
            if (_slide == 0)
            { 
                Boolean update = false;

                var elements = _elements[_currentSelection];

                for (Int32 idx = 0; idx < elements.Count; ++idx)
                {
                    update |= elements[idx].UpdateUi(time);
                }

                return update;
            }

            Int32 columns = _elements.Count;

            if (_slide < 0)
            {
                _slide -= time * 2;
                if (_slide <= -1)
                {
                    _currentSelection = _nextSelection;

                    if (_positionProperty != null)
                    {
                        _positionProperty.SetValue(Bind, _currentSelection, null);
                    }

                    _slide = 0;
                }
            }
            else
            {
                _slide += time * 2;
                if (_slide >= 1)
                {
                    _currentSelection = _nextSelection;

                    if (_positionProperty != null)
                    {
                        _positionProperty.SetValue(Bind, _currentSelection, null);
                    }

                    _slide = 0;
                }
            }

            return true;
        }

        public override Boolean Update()
        {
            Boolean change = false;

            var elements = _elements[_currentSelection];

            for (Int32 idx = 0; idx < elements.Count; ++idx)
            {
                change |= elements[idx].Update();
            }

            return change;
        }

        public override void Draw(SpriteBatch spriteBatch, Vector2 offset, Single scale, Single itemHeight, Single expanding, Single opacity)
        {
            if (_nextButton != null && _prevButton != null)
            {
                Vector2 posPrev = new Vector2(_buttonsMargin, _buttonPos) * scale + offset;
                Vector2 posNext = new Vector2(_width - _buttonsMargin, _buttonPos) * scale + offset;

                Vector2 originPrev = new Vector2(0, _prevButton.Height / 2);
                Vector2 originNext = new Vector2(_nextButton.Width, _nextButton.Height / 2);

                _prevButtonRect = new Rectangle((Int32)0, (Int32)0, (Int32)((_buttonsMargin * 2 + _prevButton.Width) ), (Int32)itemHeight);
                _nextButtonRect = new Rectangle((Int32)0, (Int32)0, (Int32)((_buttonsMargin * 2 + _nextButton.Width) ), (Int32)itemHeight);
                _nextButtonRect.X = _width - _nextButtonRect.Width;

                Int32 elementBottom = (Int32)((expanding * itemHeight) * scale + offset.Y);

                if (expanding < 0)
                {
                    elementBottom -= (Int32)(scale * 10);
                }

                Int32 buttonBottom = (Int32)(posPrev + new Vector2(0, _prevButton.Height / 2) * scale).Y;

                Int32 diff = (Int32)Math.Max(0, (buttonBottom - elementBottom) / scale);

                Rectangle rectPrev = new Rectangle(0, 0, _prevButton.Width, _prevButton.Height - diff);
                Rectangle rectNext = new Rectangle(0, 0, _nextButton.Width, _nextButton.Height - diff);

                spriteBatch.Draw(_nextButton, posNext, rectNext, (_buttonSelected > 0 ? _buttonColorPushed : _buttonColor) * opacity, 0, originNext, scale, SpriteEffects.None, 0);
                spriteBatch.Draw(_prevButton, posPrev, rectPrev, (_buttonSelected < 0 ? _buttonColorPushed : _buttonColor) * opacity, 0, originPrev, scale, SpriteEffects.None, 0);
            }

            if (_slide != 0 && _nextButton != null && _prevButton != null)
            {
                Rectangle clip = new Rectangle((Int32)(_prevButtonRect.Right*scale), (Int32)offset.Y, (Int32)((_nextButtonRect.Left-_prevButtonRect.Right)*scale), (Int32)(itemHeight*scale));

                Rectangle current = spriteBatch.GraphicsDevice.ScissorRectangle;
                clip = GraphicsHelper.IntersectRectangle(ref clip, ref current);

                ScreenManager.Current.AdditionalDraw(() =>
                {
                    var elements = _elements[_currentSelection];

                    for (Int32 idx = 0; idx < elements.Count; ++idx)
                    {
                        elements[idx].Draw(spriteBatch, offset - new Vector2(_width * _slide, 0) * scale, scale, itemHeight, expanding, opacity);
                    }

                    Int32 columns = _elements.Count;
                    Int32 currentSelection = _currentSelection;
                    Single additional = 0;

                    currentSelection = _nextSelection;

                    if (_slide < 0)
                    {    
                        additional = _width;
                    }
                    else
                    {
                        additional = -_width;
                    }

                    elements = _elements[currentSelection];

                    for (Int32 idx = 0; idx < elements.Count; ++idx)
                    {
                        elements[idx].Draw(spriteBatch, offset - new Vector2(_width * _slide + additional, 0) * scale, scale, itemHeight, expanding, opacity);
                    }
                }
                , clip);
            }
            else
            { 
                var elements = _elements[_currentSelection];

                for (Int32 idx = 0; idx < elements.Count; ++idx)
                {
                    elements[idx].Draw(spriteBatch, offset - new Vector2(_width*_slide, 0) * scale, scale, itemHeight, expanding, opacity);
                }
            }
        }

        public override Boolean OnGesture(GestureType type, Vector2 position, params Object[] parameters)
        {
            switch(type)
            {
            case GestureType.HorizontalDrag:
                return HandleDragGesture(position, parameters);

            case GestureType.Tap:

                if (HandleTouchUp(position))
                {
                    return true;
                }

                break;

            case GestureType.None:

                if ((GestureAdditionalType)parameters[0] == GestureAdditionalType.TouchDown)
                {
                    if (HandleTouchDown(position))
                    {
                        return true;
                    }
                }
                else if ((GestureAdditionalType)parameters[0] == GestureAdditionalType.TouchUp)
                {
                    _buttonSelected = 0;
                }
                break;
            }

            var elements = _elements[_currentSelection];

            for (Int32 idx = 0; idx < elements.Count; ++idx)
            {
                if (elements[idx].OnGesture(type, position, parameters))
                {
                    return true;
                }
            }

            return false;
        }

        private Boolean HandleTouchDown(Vector2 position)
        {
            Point pos = GraphicsHelper.PointFromVector2(position);

            _buttonSelected = 0;

            if (_prevButtonRect.Contains(pos))
            {
                _buttonSelected = -1;
            }

            if (_nextButtonRect.Contains(pos))
            {
                _buttonSelected = 1;
            }

            InputHandler.Current.PointerInvalidated += PointerInvalidated;

            return _buttonSelected != 0;
        }

        void PointerInvalidated(object sender, GestureEventArgs e)
        {
            InputHandler.Current.PointerInvalidated -= PointerInvalidated;
            _buttonSelected = 0;
        }

        private Boolean HandleTouchUp(Vector2 position)
        {
            Point pos = GraphicsHelper.PointFromVector2(position);

            if (_prevButtonRect.Contains(pos) && _buttonSelected < 0)
            {
                ShowPrevious();
                _buttonSelected = 0;
                return true;
            }

            if (_nextButtonRect.Contains(pos) && _buttonSelected > 0)
            {
                ShowNext();
                _buttonSelected = 0;
                return true;
            }

            return false;
        }

        private Boolean HandleDragGesture(Vector2 position, Object[] parameters)
        {
            if (_buttonSelected != 0)
            {
                _buttonSelected = 0;
                return false;
            }

            if (position.Y < 0)
            {
                return false;
            }

            if ( _slide != 0 )
            {
                return false;
            }

            _dragSize -= ((Vector2)parameters[0]).X;

            if (Math.Abs(_dragSize) > 64)
            {
                if (_dragSize > 0)
                {
                    ShowNext();
                }
                else
                {
                    ShowPrevious();
                }

                _dragSize = 0;
            }

            if ((Boolean)parameters[1])
            {
                _dragSize = 0;
            }

            return true;
        }
    }
}
