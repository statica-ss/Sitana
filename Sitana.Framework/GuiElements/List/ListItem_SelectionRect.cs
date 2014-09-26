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
using System.Reflection;
using Ebatianos.Content;
using Ebatianos;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Ebatianos.Gui.List
{
    public class ListItem_SelectionRect : ListItemElement
    {
        private Texture2D _texture;
        private Single _width;
        private ColorWrapper _color;
        private Single _offsetX;
        private Single _marginY;

        private String _selectionColorPropertyName;

        public ListItem_SelectionRect()
        {

        }

        public override void Init(XmlFileNode node)
        {
            ParametersCollection parameters = node.Attributes;

            String texturePath = parameters.AsString("Image");

            _width = parameters.AsInt32("Width");
            _offsetX = parameters.AsInt32("X");
            _marginY = parameters.AsInt32("Y");

            _texture = ContentLoader.Current.Load<Texture2D>(texturePath);

            var colorPropertyName= parameters.AsString("Color");

            if(colorPropertyName.StartsWith("@"))
            {
                _selectionColorPropertyName = parameters.AsString("Color").Substring(1);
            }
            else
            {
                _color = new ColorWrapper(parameters.AsColor("Color"));
            }
        }

        public override ListItemElement Clone(Object bind)
        {
            ListItem_SelectionRect rect = new ListItem_SelectionRect();
            rect._texture = _texture;
            rect._width = _width;
            rect._offsetX = _offsetX;
            rect._marginY = _marginY;

            rect.Bind = bind;

            if(_selectionColorPropertyName != null)
            {
                PropertyInfo colorBind = bind.GetType().GetProperty(_selectionColorPropertyName);
                rect._color = colorBind != null ? (ColorWrapper)colorBind.GetValue(bind, null) : new ColorWrapper();
            }
            else
            {
                rect._color = _color;
            }

            return rect;
        }

        public override void Draw(SpriteBatch spriteBatch, Vector2 offset, Single scale, Single itemHeight, Single expanding, Single opacity)
        {
            Vector2 rescale = new Vector2(_width * scale, (itemHeight + _marginY) * scale);

            if (_color.Value.A > 0)
            { 
                spriteBatch.Draw(_texture, offset + new Vector2(_offsetX * scale, 0), null, _color.Value * opacity, 0, Vector2.Zero, rescale, SpriteEffects.None, 0);
            }
        }
    }
}
