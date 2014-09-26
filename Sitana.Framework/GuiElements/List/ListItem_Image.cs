using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Sitana.Framework.Content;
using System.Reflection;

namespace Sitana.Framework.Gui.List
{
    public class ListItem_Image : ListItemElement
    {
        private Texture2D _texture;

        private String _texturePropertyName;
        private Point _position;
        private ColorWrapper _color;
        private String _colorPropertyName;
        private Single _rotation = 0;
        private Single _rotationSpeed = 0;
        private Int32? _colorPropertyIndex;
        private Vector2 _origin;
        private Vector2 _scale;
        private Align _align;

        private Int32 _width;
        private Int32 _height;

        public override void Init(XmlFileNode node)
        {
            ParametersCollection parameters = node.Attributes;

            String texturePath = parameters.AsString("Texture");

            Int32 x = parameters.AsInt32("X");
            Int32 y = parameters.AsInt32("Y");

            _position = new Point(x, y);
            
            _rotationSpeed = parameters.AsSingle("RotationSpeed");

            if (texturePath.StartsWith("@"))
            {
                _texturePropertyName = texturePath.Substring(1);
            }
            else
            {
                _texture = ContentLoader.Current.Load<Texture2D>(texturePath);
            }

            String colorPropertyName = parameters.AsString("Color");

            if (colorPropertyName.StartsWith("@"))
            {
                if (colorPropertyName.Contains(":"))
                {
                    String[] names = colorPropertyName.Split(':');

                    _colorPropertyName = names[0].Substring(1);
                    _colorPropertyIndex = Int32.Parse(names[1]);
                }
                else
                {
                    _colorPropertyName = colorPropertyName.Substring(1);
                }
            }
            else
            {
                _colorPropertyName = "";
                _color = new ColorWrapper(parameters.AsColor("Color"));
            }

            _align = parameters.AsAlign("Align", "Valign", Align.Center | Align.Middle);


            _width = parameters.AsInt32("Width");
            _height = parameters.AsInt32("Height");
        }

        private void ComputeOrigin()
        {
            _origin = Vector2.Zero;
            _scale = Vector2.One;

            if ( GuiElement.IsAlign(_align, Align.Right) )
            {
                _origin.X = _texture.Width;
            }
            else if ( GuiElement.IsAlign(_align, Align.Center) )
            {
                _origin.X = (Single)_texture.Width / 2;
            }

            if ( GuiElement.IsAlign(_align, Align.Bottom) )
            {
                _origin.Y = _texture.Height;
            }
            else if ( GuiElement.IsAlign(_align, Align.Middle) )
            {
                _origin.Y = (Single)_texture.Height / 2;
            }

            if (_width != 0)
            {
                _scale.X = (Single)_width / (Single)_texture.Width;
            }

            if (_height != 0)
            {
                _scale.Y = (Single)_height / (Single)_texture.Height;
            }
        }

        public override ListItemElement Clone(Object bind)
        {
            ListItem_Image image = new ListItem_Image();
            image._position = _position;
            image._texture = _texture;
            image._color = _color;
            image._rotationSpeed = _rotationSpeed;
            image.Bind = bind;
            image._origin = _origin;
            image._scale = _scale;
            image._width = _width;
            image._height = _height;
            image._align = _align;

            if (!String.IsNullOrEmpty(_texturePropertyName))
            {
                PropertyInfo textureBind = bind.GetType().GetProperty(_texturePropertyName);
                String path = textureBind != null ? (String)textureBind.GetValue(bind, null) : String.Empty;
                image._texture = ContentLoader.Current.Load<Texture2D>(path);
            }

            PropertyInfo colorBind = bind.GetType().GetProperty(_colorPropertyName);

            Object color = colorBind != null ? colorBind.GetValue(bind, null) : _color;

            if (_colorPropertyIndex.HasValue)
            {
                image._color = ((ColorWrapper[])color)[_colorPropertyIndex.Value];
            }
            else
            {
                image._color = (ColorWrapper)color;
            }

            image.ComputeOrigin();

            image.Bottom = _position.Y;
            image.Bottom += (Int32)GuiElement.OffsetByAlign(_align, new Vector2(0, image._texture.Height)).Y;

            return image;
        }

        public override Boolean UpdateUi(Single time)
        {
            if (_color.Value.A > 0 && _rotationSpeed > 0)
            { 
                _rotation += _rotationSpeed * time * (Single)Math.PI * 2;
                return true;
            }

            return false;
        }

        public override void Draw(SpriteBatch spriteBatch, Vector2 offset, Single scale, Single itemHeight, Single expanding, Single opacity)
        {
            Vector2 position = GraphicsHelper.Vector2FromPoint(_position) * scale;

            Single bottom = position.Y + (_texture.Height - _origin.Y) * scale * _scale.Y;

            Int32 elementBottom = (Int32)((expanding * itemHeight) * scale);

            if (expanding < 0)
            {
                elementBottom -= (Int32)(scale * 2);
            }

            Int32 diff = (Int32)Math.Max(0, (bottom - elementBottom) / scale);

            position += offset;
            position = new Vector2((Int32)position.X, (Int32)position.Y);

            if (_color.Value.A > 0)
            {
                Vector2 size = scale * _scale * new Vector2(_texture.Width, _texture.Height);

                if (size.X < 1)
                {
                    _scale.X = 1.0f / scale / (Single)_texture.Width;
                }

                if (size.Y < 1)
                {
                    _scale.Y = 1.0f / scale / (Single)_texture.Height;
                }

                Rectangle rect = new Rectangle(0, 0, _texture.Width, Math.Min(_texture.Height, _texture.Height - diff + 1) );
                spriteBatch.Draw(_texture, position, rect, _color.Value * opacity, _rotation, _origin, scale * _scale, SpriteEffects.None, 0);
            }
        }
    }
}
