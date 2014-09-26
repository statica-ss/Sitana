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
    public class ListItem_MultilineLabel: ListItemElement
    {
        private String _textPropertyName;
        private String _textColorPropertyName;
        private ColorWrapper _color;
        private IFontPresenter[] _texts;
        private Int32 _width;
        private Point _position;
        private IFontPresenter _presenter;
        private Single _interline;
        private Int32 _margin = 0;
        private String _boldPropertyName;
        private Boolean[] _bold = new Boolean[1] { false };
        private Boolean _justify = false;
        private Int32 _indent = 0;

        public ListItem_MultilineLabel()
        {

        }

        public override void Init(XmlFileNode node)
        {
            ParametersCollection parameters = node.Attributes;

            String fontPath = parameters.AsString("Font");

            Int32 x = parameters.AsInt32("X");
            Int32 y = parameters.AsInt32("Y");

            _position = new Point(x, y);

            _width = parameters.AsInt32("Width");
            _margin = parameters.AsInt32("Margin");

            _justify = parameters.AsBoolean("Justify");
            _indent = parameters.AsInt32("Indent");

            String textPropertyName = parameters.AsString("Text");
            String textColorPropertyName = parameters.AsString("TextColor");
            String boldPropertyName = parameters.AsString("Bold");

            _interline = parameters.AsSingle("Interline");

            _presenter = FontLoader.Load(fontPath, String.Empty);

            if (textPropertyName.StartsWith("@"))
            {
                _textPropertyName = textPropertyName.Substring(1);
            }
            else
            {
                _textPropertyName = "";
                _texts = _presenter.PrepareMultilineText(new StringBuilder(textPropertyName), _width).ToArray();
            }

            if (textColorPropertyName.StartsWith("@"))
            {
                _textColorPropertyName = parameters.AsString("TextColor").Substring(1);
            }
            else
            {
                _textColorPropertyName = "";
                _color = new ColorWrapper(parameters.AsColor("TextColor"));
            }

            if (boldPropertyName.StartsWith("@"))
            {
                _boldPropertyName = parameters.AsString("Bold").Substring(1);
            }
            else
            {
                _boldPropertyName = "";
                _bold[0] = parameters.AsBoolean("Bold");
            }
        }

        public override ListItemElement Clone(Object bind)
        {
            ListItem_MultilineLabel item = new ListItem_MultilineLabel();

            item._position = _position;
            item._width = _width;
            item._texts = _texts;
            item._interline = _interline;
            item._margin = _margin;
            item._indent = _indent;

            PropertyInfo textBind = bind.GetType().GetProperty(_textPropertyName);
            PropertyInfo colorBind = bind.GetType().GetProperty(_textColorPropertyName);
            PropertyInfo boldBind = bind.GetType().GetProperty(_boldPropertyName);

            Object color = colorBind != null ? colorBind.GetValue(bind, null) : _color;
            Object text = textBind != null ? textBind.GetValue(bind, null) : null;
            Object bold = boldBind != null ? boldBind.GetValue(bind, null) : _bold;

            item._color = (ColorWrapper)color;
            item._bold = (Boolean[])bold;

            StringBuilder builder = text as StringBuilder;

            if (builder != null)
            {
                item._texts = (_presenter as BitmapFontPresenter).PrepareMultilineText(builder, _width, _justify, _indent).ToArray();
            }

            if (item._texts != null && item._texts.Length > 0)
            {
                item.Bottom = _position.Y + _margin;

                foreach (var txt in item._texts)
                {
                    item.Bottom += (Int32)(txt.LineHeight * _interline);
                }
            }
            else
            {
                item.Bottom = 0;
            }

            return item;
        }

        public override Boolean Update()
        {
            return true;
        }

        public override void Draw(SpriteBatch spriteBatch, Vector2 offset, Single scale, Single itemHeight, Single expanding, Single opacity)
        {
            Vector2 position = GraphicsHelper.Vector2FromPoint(_position) * scale;
            position += offset;

            if (_texts != null)
            {
                Int32 indent = (Int32)(scale * _indent);
                foreach (var text in _texts)
                {
                    text.DrawText(spriteBatch, position + new Vector2(indent, 0), _color.Value, scale, Align.Top | Align.Left, _bold[0]);
                    position.Y += (Int32)(text.LineHeight * _interline * scale);
                    indent = 0;
                }
            }
        }
    }
}
