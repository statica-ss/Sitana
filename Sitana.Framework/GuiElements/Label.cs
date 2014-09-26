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
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Ebatianos.Content;
using Ebatianos;
using System.Collections.Generic;
using System.Text;

namespace Ebatianos.Gui
{
    public class Label : GuiElement
    {
        private IFontPresenter _presenter;
        protected ColorWrapper _color;
        private Align _align;
        private Point _position;
        protected String _text;
        public Single MatchScale { get; set; }
        private Vector2 _offset;

        private Single _maxWidth;

        public Label()
        {
            MatchScale = 1;
        }

        public override GuiElement Clone()
        {
            Label label = new Label();

            label.OnCloned(this);

            return label;
        }

        protected override void OnCloned(GuiElement source)
        {
            var label = source as Label;

            _presenter = label._presenter.Clone();
            _align = label._align;
            _position = label._position;
            _color = label._color;

            UpdateText(label._text);

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

            if (SecondInstance != null)
            {
                if (this == FirstInstance)
                {
                    transition = 1;
                }
                else
                {
                    return;
                }
            }

            // Position of button (Button's center).
            Vector2 position = GraphicsHelper.Vector2FromPoint(
                                   new Point(ElementRectangle.X, ElementRectangle.Y)
                               );

            Color color = ComputeColorWithTransition(transition, _color.Value);

            Vector2 offset = ComputeOffsetWithTransition(transition) + topLeft;

            _presenter.DrawText(spriteBatch, position + offset, color * Opacity, Scale * MatchScale, Align.Top | Align.Left);
        }

        /// <summary>
        /// Initializes label from parameters.
        /// </summary>
        /// <param name="parameters">Parameters from xml node.</param>
        /// <param name="contentLoader">Content loader.</param>
        /// <param name="owner">Owner screen.</param>
        /// <returns>True when succeeded.</returns>
        /// <summary>
        /// Initializes accordion from parameters.
        /// </summary>
        /// <param name="node">XML node entity.</param>
        /// <param name="parameters">Parameters from xml node.</param>
        /// <param name="scale">Current screen scale.</param>
        /// <param name="areaSize">Size of the area.</param>
        /// <param name="owner">Owner screen.</param>
        /// <returns>True when succeeded.</returns>
        protected override Boolean Initialize(InitializeParams initParams)
        {
            ParametersCollection parameters = initParams.Parameters;
            String directory = initParams.Directory;
            Vector2 scale = initParams.Scale;
            Vector2 areaSize = initParams.AreaSize;
            Vector2 offset = initParams.Offset;
            Screen owner = initParams.Owner;

            // First unserialize base parameters.
            if (!base.Initialize(initParams))
            {
                return false;
            }

            String text = parameters.AsString("Text");
            String font = parameters.AsString("Font");

            if (text.StartsWith(":"))
            {
                text = owner.ScreenManager.StringsManager[text];
            }

            _color = parameters.AsObject<ColorWrapper>("Color");

            if (_color == null)
            {
                _color = new ColorWrapper(parameters.AsColor("Color"));
            }

            _maxWidth = parameters.AsSingle("MaxWidth") * scale.X;

            _align = parameters.AsAlign("Align", "Valign");

            _position = FindPosition(parameters, GraphicsHelper.PointFromVector2(areaSize), scale);

            _presenter = FontLoader.Load(font, directory);
            _presenter.PrepareRender(text);

            Point size = CalculateSizeToMatchWidth();

            ElementRectangle = RectangleFromAlignAndSize(_position, size, _align, offset);

            _offset = offset;

            _text = text;

            return true;
        }

        private Point CalculateSizeToMatchWidth()
        {
            Point size = GraphicsHelper.PointFromVector2(_presenter.Size * Scale * MatchScale);

            if (_maxWidth > 0)
            {
                if (size.X > _maxWidth)
                {
                    MatchScale = _maxWidth / size.X;
                    size = GraphicsHelper.PointFromVector2(_presenter.Size * Scale * MatchScale);
                }
                else
                {
                    MatchScale = 1;
                }
            }

            return size;
        }

        public void UpdateText(String text)
        {
            _text = text;
            _presenter.PrepareRender(text);

            Point size = CalculateSizeToMatchWidth();
            ElementRectangle = RectangleFromAlignAndSize(_position, size, _align, _offset);
        }

        public void UpdateText(StringBuilder text)
        {
            _presenter.PrepareRender(text);
            Point size = CalculateSizeToMatchWidth();
            ElementRectangle = RectangleFromAlignAndSize(_position, size, _align, _offset);
        }

        public void UpdateSize()
        {
            Point size = CalculateSizeToMatchWidth();
            ElementRectangle = RectangleFromAlignAndSize(_position, size, _align, _offset);
        }

        public void UpdateColor(Color color)
        {
            _color.Value = color;
        }
    }
}
