// /// This file is a part of the EBATIANOS.ESSENTIALS class library.
// /// (c)2013-2014 EBATIANO'S a.k.a. Sebastian Sejud. All rights reserved.
// ///
// /// THIS SOURCE FILE IS THE PROPERTY OF EBATIANO'S A.K.A. SEBASTIAN SEJUD 
// /// AND IS NOT TO BE RE-DISTRIBUTED BY ANY MEANS WHATSOEVER WITHOUT 
// /// THE EXPRESSED WRITTEN CONSENT OF EBATIANO'S A.K.A. SEBASTIAN SEJUD.
// ///
// /// THIS SOURCE CODE CAN ONLY BE USED UNDER THE TERMS AND CONDITIONS OUTLINED
// /// IN THE EBATIANOS.ESSENTIALS LICENSE AGREEMENT. 
// /// EBATIANO'S A.K.A. SEBASTIAN SEJUD GRANTS TO YOU (ONE SOFTWARE DEVELOPER) 
// /// THE LIMITED RIGHT TO USE THIS SOFTWARE ON A SINGLE COMPUTER.
// ///
// /// CONTACT INFORMATION:
// /// contact@ebatianos.com
// /// www.ebatianos.com/essentials-library
// /// 
// ///---------------------------------------------------------------------------
//
using System;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System.Reflection;
using Sitana.Framework.Content;
using Microsoft.Xna.Framework.Input.Touch;
using Sitana.Framework.Input;
using System.Collections.Generic;

namespace Sitana.Framework.Gui.List
{
    public class ListItem_Toolbar : ListItemElement
    {
        private class ToolbarButton
        {
            public Int32 X;
            public Texture2D Texture;
            public ParametersCollection Command;
        }

        private Single _width;
        private Single _size;
        private Color _backgroundColor;
        private Color _pushedColor;

        private Texture2D _background;

        private String _visiblePropertyName;

        private Boolean _visible = false;

        PropertyInfo _visibleProperty;

        private Single _visiblity = 0;

        private Single _offsetY;

        private Int32 _pushedButton = -1;

        private List<ToolbarButton> _buttons = new List<ToolbarButton>();

        private Action _onHideAction = null;

        public ListItem_Toolbar()
        {
        }

        public override void Init(XmlFileNode node)
        {
            ParametersCollection parameters = node.Attributes;

            _width = parameters.AsSingle("Width");
            _backgroundColor = parameters.AsColor("BackgroundColor");
            _visiblePropertyName = parameters.AsString("Visible").Substring(1);
            _offsetY = parameters.AsSingle("OffsetY");
            _pushedColor = parameters.AsColor("PushedColor");

            _background = ContentLoader.Current.Load<Texture2D>(parameters.AsString("Background"));

            _size = _background.Width;

            foreach (var nd in node.Nodes)
            {
                Texture2D icon = ContentLoader.Current.Load<Texture2D>(nd.Attributes.AsString("Icon"));

                String commandName = nd.Attributes.AsString("OnClick");

                ParametersCollection command = new ParametersCollection(nd.Attributes.ValueSource, true);
                command.Add("Command", commandName);

                Int32 x = nd.Attributes.AsInt32("X") - icon.Width / 2;

                _buttons.Add(new ToolbarButton()
                {
                    X = x,
                    Texture = icon,
                    Command = command
                });
            }
        }

        public override ListItemElement Clone(Object bind)
        {
            ListItem_Toolbar toolbar = new ListItem_Toolbar();

            toolbar._width = _width;
            toolbar._size = _size;
            toolbar._backgroundColor = _backgroundColor;
            toolbar._background = _background;
            toolbar._visibleProperty = bind.GetType().GetProperty(_visiblePropertyName);
            toolbar._offsetY = _offsetY;
            toolbar._pushedColor = _pushedColor;
            toolbar._buttons = _buttons;

            toolbar.Bind = bind;

            return toolbar;
        }

        public override void Draw(SpriteBatch spriteBatch, Vector2 offset, Single scale, Single itemHeight, Single expanding, Single opacity)
        {
            if (_visiblity == 0)
            {
                return;
            }

            offset.X += scale * (_width - _visiblity * _size);
            offset.Y += _offsetY * scale;

            Vector2 origin = new Vector2(0, _background.Height / 2);

            spriteBatch.Draw(_background, offset, null, _backgroundColor * opacity, 0, origin, scale, SpriteEffects.None, 0);

            for (Int32 idx = 0; idx < _buttons.Count; ++idx)
            {
                var btn = _buttons[idx];
                Vector2 middle = new Vector2(btn.X * scale, 0) + offset;
                origin = new Vector2(btn.Texture.Width / 2, btn.Texture.Height / 2);

                spriteBatch.Draw(btn.Texture, middle, null, (_pushedButton == idx ? _pushedColor : Color.White), 0, origin, scale, SpriteEffects.None, 0);
            }
        }

        public override Boolean UpdateUi(Single time)
        {
            if (_visible && _visiblity < 1)
            {
                _visiblity += time * 8;
                _visiblity = Math.Min(1, _visiblity);
                return true;
            }

            if (!_visible && _visiblity > 0)
            {
                _visiblity -= time * 8;
                _visiblity = Math.Max(0, _visiblity);

                if (_visiblity <= 0)
                {
                    if (_onHideAction != null)
                    {
                        _onHideAction.Invoke();
                        _onHideAction = null;
                    }
                }
                return true;
            }

            return false;
        }

        public override Boolean Update()
        {
            Boolean visible = _visible;

            if (!_visible)
            {
                _pushedButton = -1;
            }

            _visible = (Boolean)_visibleProperty.GetValue(Bind, null);
            return _visible != visible;
        }

        public override Boolean OnGesture(GestureType type, Vector2 position, params Object[] parameters)
        {
            if (!_visible)
            {
                return false;
            }

            if (type == GestureType.Tap)
            {
                if (ButtonIndexFromPosition(position))
                {
                    if (_pushedButton >= 0)
                    {
                        _onHideAction = _buttons[_pushedButton].Command.AsAction("Command", Bind);
                        Hide();
                        _pushedButton = -1;
                    }
                    return true;
                }
                _pushedButton = -1;
                return false;
            }
            else if (type == GestureType.None)
            {
                switch ((GestureAdditionalType)parameters[0])
                {
                    case GestureAdditionalType.Cancel:
                        _pushedButton = -1;
                        (Bind as ListItemData).Update();
                        return true;

                    case GestureAdditionalType.TouchUp:
                        return true;

                    case GestureAdditionalType.TouchDown:

                        if (ButtonIndexFromPosition(position))
                        {
                            InputHandler.Current.PointerInvalidated += PointerInvalidated;
                        }

                        return true;
                }
            }

            return false;
        }

        void Hide()
        {
            UiTask.BeginInvoke( ()=>
                {
                    _pushedButton = -1;
                    _visibleProperty.SetValue(Bind, false, null);
                    _visible = false;
                }
            );
        }

        void PointerInvalidated(object sender, GestureEventArgs e)
        {
            InputHandler.Current.PointerInvalidated -= PointerInvalidated;
            _pushedButton = -1;

            (Bind as ListItemData).Update();
        }

        private Boolean ButtonIndexFromPosition(Vector2 position)
        {
            _pushedButton = -1;
            if (position.X > _width - _size)
            {
                if (position.Y < _background.Height / 2 + _offsetY &&
                position.Y > _offsetY - _background.Height / 2)
                {
                    Single distance = Single.MaxValue;

                    for (Int32 idx = 0; idx < _buttons.Count; ++idx)
                    {
                        Single dist = Math.Abs(_buttons[idx].X + _width - _size - position.X);

                        if (dist < distance)
                        {
                            _pushedButton = idx;
                            distance = dist;
                            (Bind as ListItemData).Update();
                        }
                    }

                    return true;
                }
            }
            return false;
        }
    }
}

