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
using Ebatianos.Gui.List;
using System.Collections.Generic;
using Ebatianos.Content;
using System.Text;
using System.Reflection;


namespace Ebatianos.Gui
{
    public class DynamicLabel: Label
    {
        StringBuilder _textSource;
        ColorWrapper _colorSource;

        private String[] _noOverlapped = null;

        private Int32 _noOverlappedMargin = 10;

        public override GuiElement Clone()
        {
            DynamicLabel label = new DynamicLabel();

            label.OnCloned(this);
            return label;
        }

        protected override void OnCloned(GuiElement source)
        {
            var label = source as DynamicLabel;

            _textSource = label._textSource;
            _colorSource = label._colorSource;
            _noOverlapped = label._noOverlapped;

            base.OnCloned(source);
        }

        /// <summary>
        /// Draws label.
        /// </summary>
        /// <param name="spriteBatch">SpriteBatch object used to render textures and texts.</param>
        /// <param name="color">Color to multiply all contents by.</param>
        /// <param name="offset">Offset to move bnutton by.</param>
        public override void Draw(Int32 level, SpriteBatch spriteBatch, Vector2 topLeft, Single transition)
        {
            if (!DrawLevel(level))
            {
                return;
            }

            if (Opacity <= 0)
            {
                return;
            }

            UpdateColor(_colorSource.Value);

            base.Draw(level, spriteBatch, topLeft, transition);
        }

        private Boolean UpdateText()
        {
            Boolean update = false;

            if (_noOverlapped != null)
            {
                MatchScale = 1;

                for (Int32 idx = 0; idx < _noOverlapped.Length; ++idx)
                {
                    var other = Owner.Find<DynamicLabel>(_noOverlapped[idx]);
                    other.MatchScale = 1;
                    other.UpdateSize();
                }
            }

            UpdateText(_textSource);

            Boolean continueTest = _noOverlapped != null;

            while (continueTest)
            {
                continueTest = false;

                Rectangle rect = ElementRectangle;

                rect.X -= _noOverlappedMargin;
                rect.Width += _noOverlappedMargin * 2;

                for (Int32 idx = 0; idx < _noOverlapped.Length; ++idx)
                {
                    var other = Owner.Find<DynamicLabel>(_noOverlapped[idx]);

                    if (other.ElementRectangle.Intersects(rect))
                    {
                        continueTest = true;
                        update = true;
                        
                        MatchScale *= 0.9f;
                        UpdateSize();

                        for (Int32 idx2 = 0; idx2 < _noOverlapped.Length; ++idx2)
                        {
                            var other2 = Owner.Find<DynamicLabel>(_noOverlapped[idx2]);
                            other2.MatchScale = MatchScale;
                            other2.UpdateSize();
                        }
                        break;
                    }
                }
            }

            return update;
        }

        public override bool Update(TimeSpan gameTime, Screen.ScreenState screenState)
        {
            Boolean redraw = base.Update(gameTime, screenState);

            return UpdateText() || redraw;
        }

        /// <summary>
        /// Initializes label from parameters.
        /// </summary>
        /// <param name="parameters">Parameters from xml node.</param>
        /// <param name="contentLoader">Content loader.</param>
        /// <param name="owner">Owner screen.</param>
        /// <returns>True when succeeded.</returns>
        protected override Boolean Initialize(InitializeParams initParams)
        {
            ParametersCollection parameters = initParams.Parameters;
            Vector2 scale = initParams.Scale;

            // First unserialize base parameters.
            if (!base.Initialize(initParams))
            {
                return false;
            }

            String[] noOverlap = parameters.AsString("NoOverlap").Split(',');

            if (noOverlap[0] != String.Empty)
            {
                _noOverlapped = noOverlap;

                _noOverlappedMargin = (Int32)(parameters.AsInt32("NoOverlapMargin") * scale.X);
            }

            String textPropertyName = parameters.AsString("Text");
            String textColorPropertyName = parameters.AsString("Color");

            Object bind = parameters.AsObject<Object>("Binding");

            if (bind == null)
            {
                bind = Owner;
            }

            if (textPropertyName.StartsWith("@"))
            {
                if (textPropertyName.Contains(":"))
                {
                    String[] names = textPropertyName.Split(':');

                    textPropertyName = names[0].Substring(1);
                    Int32 textPropertyIndex = Int32.Parse(names[1]);

                    PropertyInfo textBind = bind.GetType().GetProperty(textPropertyName);
                    _textSource = ((StringBuilder[])textBind.GetValue(bind, null))[textPropertyIndex];
                }
                else
                {
                    textPropertyName = textPropertyName.Substring(1);

                    PropertyInfo textBind = bind.GetType().GetProperty(textPropertyName);
                    _textSource = (StringBuilder)textBind.GetValue(bind, null);
                }
            }
            else
            {
                _textSource = new StringBuilder(textPropertyName);
            }

            if (textColorPropertyName.StartsWith("@"))
            {
                if (textColorPropertyName.Contains(":"))
                {
                    String[] names = textColorPropertyName.Split(':');

                    textColorPropertyName = names[0].Substring(1);
                    Int32 textColorPropertyIndex = Int32.Parse(names[1]);

                    PropertyInfo textBind = bind.GetType().GetProperty(textColorPropertyName);
                    _colorSource = ((ColorWrapper[])textBind.GetValue(bind, null))[textColorPropertyIndex];
                }
                else
                { 
                    textColorPropertyName = textColorPropertyName.Substring(1);

                    PropertyInfo textBind = bind.GetType().GetProperty(textColorPropertyName);
                    _colorSource = (ColorWrapper)textBind.GetValue(bind, null);
                }
            }
            else
            {
                _colorSource = new ColorWrapper( parameters.AsColor("Color") );
            }

            UpdateText();
            return true;
        }
    }
}

