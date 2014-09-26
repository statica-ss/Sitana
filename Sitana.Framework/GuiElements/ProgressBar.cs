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
using Ebatianos;
using Ebatianos.Content;
using System.Collections.Generic;

namespace Ebatianos.Gui
{
    public class ProgressBar : GuiElement
    {
        private Single _progress = 0;
        private Color _bgColor;
        private Color _color;

        public override Boolean Update(TimeSpan gameTime, Screen.ScreenState screenState)
        {
            Boolean redraw = base.Update(gameTime, screenState);

            if (screenState == Screen.ScreenState.Visible)
            {
                Single progress = (Owner as ScreenPreloader).Progress;
                redraw = progress != _progress;

                _progress = progress;
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


            Color lightedColor = ComputeColorWithTransition(transition, _color);
            Color notLightedColor = ComputeColorWithTransition(transition, _bgColor);
            Vector2 offset = ComputeOffsetWithTransition(transition);

            position = position + offset;

            Vector2 scale1 = new Vector2(ElementRectangle.Width, ElementRectangle.Height);
            Vector2 scale2 = new Vector2((Int32)(ElementRectangle.Width * _progress), ElementRectangle.Height);

            spriteBatch.Draw(ContentLoader.Current.OnePixelWhiteTexture, position, null, notLightedColor, 0, Vector2.Zero, scale1, SpriteEffects.None, 0);
            spriteBatch.Draw(ContentLoader.Current.OnePixelWhiteTexture, position, null, lightedColor, 0, Vector2.Zero, scale2, SpriteEffects.None, 0);
        }

        /// <summary>
        /// Initializes image from parameters.
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
            Vector2 scale = initParams.Scale;
            Vector2 areaSize = initParams.AreaSize;
            Vector2 offset = initParams.Offset;

            // First unserialize base parameters.
            if (!base.Initialize(initParams))
            {
                return false;
            }

            Align align = parameters.AsAlign("Align", "Valign");

            Point position = FindPosition(parameters, GraphicsHelper.PointFromVector2(areaSize), scale);

            Int32 width = (Int32)(parameters.AsInt32("Width") * scale.X);
            Int32 height = (Int32)(parameters.AsInt32("Height") * scale.Y);

            _bgColor = parameters.AsColor("BgColor");
            _color = parameters.AsColor("Color");

            Point size = new Point(width, height);

            ElementRectangle = RectangleFromAlignAndSize(position, size, align, offset);

            return true;
        }
    }
}
