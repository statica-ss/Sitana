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
using Sitana.Framework.Content;
using Sitana.Framework;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Sitana.Framework.Gui.List
{
    public class ListItem_Separator : ListItemElement
    {
        private Texture2D _texture;
        private Point _position;
        private Color _color;
        private Int32 _width;

        public ListItem_Separator()
        {

        }

        public override void Init(XmlFileNode node)
        {
            ParametersCollection parameters = node.Attributes;

            String texturePath = parameters.AsString("Image");

            Int32 x = parameters.AsInt32("X");
            Int32 y = parameters.AsInt32("Y");

            _width = parameters.AsInt32("Width");

            _color = parameters.AsColor("Color");

            _position = new Point(x, y);

            _texture = ContentLoader.Current.Load<Texture2D>(texturePath);
        }

        public override ListItemElement Clone(Object bind)
        {
            ListItem_Separator separator = new ListItem_Separator();
            separator._position = _position;
            separator._texture = _texture;
            separator._color = _color;
            separator._width = _width;

            return separator;
        }

        public override void Draw(SpriteBatch spriteBatch, Vector2 offset, Single scale, Single itemHeight, Single expanding, Single opacity)
        {
            Vector2 position = (GraphicsHelper.Vector2FromPoint(_position) + new Vector2(0, itemHeight)) * scale;

            position += offset;

            Vector2 rescale = new Vector2(scale);

            if (_width != 0)
            {
                rescale.X = scale * (Single)_width / (Single)_texture.Width;
            }

            spriteBatch.Draw(_texture, position, null, _color * opacity, 0, Vector2.Zero, rescale, SpriteEffects.None, 0);
        }
    }
}
