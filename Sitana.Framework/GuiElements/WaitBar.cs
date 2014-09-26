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
using Sitana.Framework;
using Sitana.Framework.Content;
using System.Collections.Generic;

namespace Sitana.Framework.Gui
{
    public class WaitBar : GuiElement
    {
        private Texture2D _texture;
        private Double _progress = 0;
        private Vector2 _squareScale;
        private Single _squareSize = 0;
        private Single _squareSpacing = 0;
        private Color _notLightedColor;
        private Color _lightedColor;
        private Int32 _squareCount;
        private Single _speed;

        public override Boolean Update(TimeSpan gameTime, Screen.ScreenState screenState)
        {
            Boolean redraw = base.Update(gameTime, screenState);

            if (screenState == Screen.ScreenState.Visible)
            {
                _progress += gameTime.TotalSeconds * _speed;
                redraw = true;
            }

            return redraw;
        }

        /// <summary>
        /// Draws button.
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

            // Position of button (Button's center).
            Vector2 position = GraphicsHelper.Vector2FromPoint(
               new Point(ElementRectangle.X, ElementRectangle.Y)
               );


            Color lightedColor = ComputeColorWithTransition(transition, _lightedColor);
            Color notLightedColor = ComputeColorWithTransition(transition, _notLightedColor);
            Vector2 offset = ComputeOffsetWithTransition(transition);

            position = position + offset;

            for (Int32 idx = 0; idx < _squareCount; ++idx)
            {
                Color color = (idx == ((Int32)_progress) % _squareCount) ? lightedColor : notLightedColor;

                if (_speed < 0)
                {
                    color = notLightedColor;
                }

                spriteBatch.Draw(_texture, position, null, color, 0, Vector2.Zero, _squareScale, SpriteEffects.None, 0);
                position.X += _squareSize + _squareSpacing;
            }
        }

        /// <summary>
        /// Initializes image from parameters.
        /// </summary>
        /// <param name="parameters">Parameters from xml node.</param>
        /// <param name="contentLoader">Content loader.</param>
        /// <param name="owner">Owner screen.</param>
        /// <returns>True when succeeded.</returns>
        protected override Boolean Initialize(InitializeParams initParams)
        {
            ParametersCollection parameters = initParams.Parameters;
            String directory = initParams.Directory;
            Vector2 scale = initParams.Scale;
            Vector2 areaSize = initParams.AreaSize;
            Vector2 offset = initParams.Offset;

            // First unserialize base parameters.
            if (!base.Initialize(initParams))
            {
                return false;
            }

            String texture = parameters.AsString("Texture");

            Texture2D textureObj = ContentLoader.Current.Load<Texture2D>(texture);

            Align align = parameters.AsAlign("Align", "Valign");

            Point position = FindPosition(parameters, GraphicsHelper.PointFromVector2(areaSize), scale);

            _texture = textureObj;

            _notLightedColor = parameters.AsColor("ColorNotLighted");
            _lightedColor = parameters.AsColor("ColorLighted");

            _squareSize = (Single)parameters.AsInt32("SquareSize") * Scale.X;
            _squareSpacing = (Single)parameters.AsInt32("SquareSpacing") * Scale.X;

            _squareCount = parameters.AsInt32("SquareCount");

            if (_squareCount == 0)
            {
                _squareCount = 3;
            }

            _speed = parameters.AsInt32("Speed");

            if (_speed == 0)
            {
                _speed = 2;
            }

            Single width = _squareCount * _squareSize + (_squareCount - 1) * _squareSpacing;

            Point size = GraphicsHelper.PointFromVector2(new Vector2(width, _squareSize));

            _squareScale = new Vector2(_squareSize / (Single)textureObj.Width, _squareSize / (Single)textureObj.Height);

            ElementRectangle = RectangleFromAlignAndSize(position, size, align, offset);

            return true;
        }
    }
}
