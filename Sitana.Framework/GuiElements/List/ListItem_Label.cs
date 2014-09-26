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
using System.Text;

namespace Ebatianos.Gui.List
{
    public class ListItem_Label : ListItemElement
    {
        private IFontPresenter _fontPresenter;

        private String _textPropertyName;
        private Align _align;
        private Point _position;
        
        private String _textColorPropertyName;
        private String _boldPropertyName;

        private ColorWrapper _color;

        private Color[] _colors = null;
        private Color[] _tempColors = null;

        private StringBuilder _stringBuilder;
        private Int32? _textPropertyIndex;
        private Int32? _textColorPropertyIndex;

        private Boolean[] _bold = new Boolean[1]{false};

        public ListItem_Label()
        {
            _color = new ColorWrapper(Color.White);
        }

        public override void Init(XmlFileNode node)
        {
            ParametersCollection parameters = node.Attributes;

            String fontPath = parameters.AsString("Font");

            Int32 x = parameters.AsInt32("X");
            Int32 y = parameters.AsInt32("Y");

            _fontPresenter = FontLoader.Load(fontPath, String.Empty);

            _position = new Point(x, y);

            _align = parameters.AsAlign("Align", "Valign");

            String textPropertyName = parameters.AsString("Text");
            String textColorPropertyName = parameters.AsString("TextColor");
            String boldPropertyName = parameters.AsString("Bold");

            if (textPropertyName.StartsWith("@"))
            {
                if (textPropertyName.Contains(":"))
                {
                    String[] names = textPropertyName.Split(':');

                    _textPropertyName = names[0].Substring(1);
                    _textPropertyIndex = Int32.Parse(names[1]);
                }
                else
                {
                    _textPropertyName = textPropertyName.Substring(1);
                }
            }
            else
            {
                _textPropertyName = "";
                _stringBuilder = new StringBuilder(textPropertyName);
            }

            if (textColorPropertyName.StartsWith("@"))
            {
                if (textColorPropertyName.Contains(":"))
                {
                    String[] names = textColorPropertyName.Split(':');

                    _textColorPropertyName = names[0].Substring(1);
                    _textColorPropertyIndex = Int32.Parse(names[1]);
                }
                else
                { 
                    _textColorPropertyName = parameters.AsString("TextColor").Substring(1);
                }
            }
            else
            {
                _textColorPropertyName = "";
                _color = new ColorWrapper( parameters.AsColor("TextColor") );
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
            ListItem_Label label = new ListItem_Label();
            label._align = _align;
            label._position = _position;
            label._fontPresenter = _fontPresenter.Clone();

            label.Bind = bind;

            PropertyInfo textBind = bind.GetType().GetProperty(_textPropertyName);
            PropertyInfo colorBind = bind.GetType().GetProperty(_textColorPropertyName);
            PropertyInfo boldBind = bind.GetType().GetProperty(_boldPropertyName);

            Object color = colorBind != null ? colorBind.GetValue(bind, null) : _color;
            Object text = textBind != null ? textBind.GetValue(bind, null) : _stringBuilder;
            Object bold = boldBind != null ? boldBind.GetValue(bind, null) : _bold;

            label._bold = (Boolean[])bold;

            if ( _textColorPropertyIndex.HasValue )
            {
                label._color = ((ColorWrapper[])color)[_textColorPropertyIndex.Value];
            }
            else
            {
                if (color is Array)
                {
                    label._colors = (Color[])color;
                }
                else
                {
                    label._color = (ColorWrapper)color;
                }
            }
            
            if ( _textPropertyIndex.HasValue )
            {
                label._stringBuilder = ((StringBuilder[])text)[_textPropertyIndex.Value];
            }
            else
            {
                label._stringBuilder = (StringBuilder)text;
            }

            label.Bottom += (Int32)GuiElement.OffsetByAlign(_align, new Vector2(0, _fontPresenter.Height)).Y;

            return label;
        }

        public override Boolean Update()
        {
            return true;
        }

        public override void Draw(SpriteBatch spriteBatch, Vector2 offset, Single scale, Single itemHeight, Single expanding, Single opacity)
        {
            _fontPresenter.PrepareRender(_stringBuilder);

            Vector2 position = GraphicsHelper.Vector2FromPoint(_position) * scale;

            position += offset;

            if (_colors != null)
            {
                if (_tempColors == null || _tempColors.Length != _colors.Length)
                {
                    _tempColors = new Color[_colors.Length];
                }

                for (Int32 idx = 0; idx < _tempColors.Length; ++idx)
                {
                    _tempColors[idx] = _colors[idx] * opacity;
                }

                _fontPresenter.DrawText(spriteBatch, position, _tempColors, scale, _align, _bold[0]);
            }
            else if (_color != null)
            {
                if (_color.Value.A > 0)
                {
                    _fontPresenter.DrawText(spriteBatch, position, _color.Value * opacity, scale, _align, _bold[0]);
                }
            }
        }
    }
}
