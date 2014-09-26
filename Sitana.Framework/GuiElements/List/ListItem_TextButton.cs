using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ebatianos.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Reflection;

namespace Ebatianos.Gui.List
{
    public class ListItem_TextButton : ListItem_ButtonBase
    {
        private IFontPresenter _fontPresenter;

        private Align _align;
        private Point _position;

        private Color _normalColor;
        private Color _pushedColor;

        ParametersCollection _actionParam;

        private Int32 _bottomMargin;

        private String _visiblityPropertyName;

        public override void Init(XmlFileNode node)
        {
            ParametersCollection parameters = node.Attributes;

            _actionParam = new ParametersCollection(parameters.ValueSource, false);

            String action = parameters.GetAt("OnClick");
            _actionParam.Add("Action", action);

            String fontPath = parameters.AsString("Font");

            Int32 x = parameters.AsInt32("X");
            Int32 y = parameters.AsInt32("Y");

            _bottomMargin = parameters.AsInt32("BottomMargin");

            _fontPresenter = FontLoader.Load(fontPath, String.Empty);

            _position = new Point(x, y);

            _align = parameters.AsAlign("Align", "Valign");

            _normalColor = parameters.AsColor("Color");
            _pushedColor = parameters.AsColor("ColorPushed");

            _visiblityPropertyName = parameters.AsString("Visible");

            if (_visiblityPropertyName.StartsWith("@"))
            {
                _visiblityPropertyName = _visiblityPropertyName.Substring(1);
            }

            String text = parameters.AsString("Text");
            _fontPresenter.PrepareRender(text);
        }

        public override Boolean Update()
        {
            return true;
        }

        public override ListItemElement Clone(Object bind)
        {
            ListItem_TextButton item = new ListItem_TextButton();

            item._fontPresenter = _fontPresenter;
            item._align = _align;
            item._position = _position;
            item._normalColor = _normalColor;
            item._pushedColor = _pushedColor;

            Rectangle rect = GuiElement.RectangleFromAlignAndSize(_position, GraphicsHelper.PointFromVector2(_fontPresenter.Size), _align, Vector2.Zero);

            rect.Y -= rect.Height / 2;
            rect.Height *= 2;

            rect.X -= rect.Height / 2;
            rect.Width += rect.Height;

            if (!String.IsNullOrEmpty(_visiblityPropertyName))
            {
                PropertyInfo visiblityBind = bind.GetType().GetProperty(_visiblityPropertyName);

                if (visiblityBind != null)
                {
                    Boolean isVisible = (Boolean)visiblityBind.GetValue(bind, null);

                    if (!isVisible)
                    {
                        return null;
                    }
                }
            }

            Action action = _actionParam.AsAction("Action", bind);
            item.InitButton(rect, action);

            item.Bottom = _position.Y;
            item.Bottom += (Int32)GuiElement.OffsetByAlign(_align, new Vector2(0, _fontPresenter.Height)).Y;
            item.Bottom += _bottomMargin;

            return item;
        }

        public override void Draw(SpriteBatch spriteBatch, Vector2 offset, Single scale, Single itemHeight, Single expanding, Single opacity)
        {
            Vector2 position = GraphicsHelper.Vector2FromPoint(_position) * scale;
            position += offset;

            _fontPresenter.DrawText(spriteBatch, position, IsPushed ? _pushedColor : _normalColor, scale, _align);
        }

    }
}
