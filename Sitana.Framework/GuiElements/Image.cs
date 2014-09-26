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
    public class Image : GuiElement
    {
        private Texture2D _texture = null;
        private ColorWrapper _color;
        private Single _rotation = 0;
        private Single _rotationSpeed = 0;

        private Rectangle _sourceRect;

        SpriteEffects _spriteEffects;

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

            if (SecondInstance != null)
            {
                if (this == SecondInstance)
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

            Vector2 origin = Vector2.Zero;

            if (_rotationSpeed != 0)
            {
                origin = new Vector2 ( _sourceRect.X + _sourceRect.Width / 2, _sourceRect.Y + _sourceRect.Height / 2);
                position = position + origin * Scale;
            }

            spriteBatch.Draw(_texture, position + offset, _sourceRect, color * Opacity, _rotation, origin, Scale, _spriteEffects, 0);
        }


        public override Boolean Update (TimeSpan gameTime, Screen.ScreenState screenState)
        {
            Boolean redraw = base.Update (gameTime, screenState);

            if (_rotationSpeed != 0)
            {
                redraw = true;
                _rotation += (Single)gameTime.TotalSeconds * _rotationSpeed;
            }

            return redraw;
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
            Boolean scaling = parameters.AsBoolean("Scaling", true);

            Single width = parameters.AsSingle("Width");
            Single height = parameters.AsSingle("Height");

            Texture2D textureObj = ContentLoader.Current.Load<Texture2D>(texture);

            Align align = parameters.AsAlign("Align", "Valign");

            _rotationSpeed = parameters.AsSingle("RotationSpeed");

            _spriteEffects = parameters.AsEnum<SpriteEffects>("Effect", SpriteEffects.None);

            Point position = FindPosition(parameters, GraphicsHelper.PointFromVector2(areaSize), scale);

            if (!scaling)
            {
                Scale = Vector2.One;
            }

            if (IsAlign(align, Align.StretchVert))
            {
                Rectangle rect = FindElementRectangle(parameters, GraphicsHelper.PointFromVector2(areaSize), scale, offset);

                align &= Align.Horz;
                align |= Align.Top;

                position.Y = rect.Top;
                height = (Single)Math.Ceiling((Single)rect.Height / scale.Y);
            }

            if (IsAlign(align, Align.StretchHorz))
            {
                Rectangle rect = FindElementRectangle(parameters, GraphicsHelper.PointFromVector2(areaSize), scale, offset);

                align &= Align.Vert;
                align |= Align.Left;

                position.X = rect.Left;
                width = (Single)Math.Ceiling((Single)rect.Width / scale.X);
            }

            _sourceRect = new Rectangle (0, 0, textureObj.Width, textureObj.Height);

            if (parameters.HasKey ("ClipHeight"))
            {
                Point clipHeight = parameters.ParsePoint ("ClipHeight");

                _sourceRect.Y = clipHeight.X;
                _sourceRect.Height = clipHeight.Y;

                if (height == 0)
                {
                    height = _sourceRect.Height;
                }
            }

            if (IsAlign(align, Align.CutVert))
            {
                Rectangle rect = FindElementRectangle(parameters, GraphicsHelper.PointFromVector2(areaSize), scale, offset);

                align &= Align.Horz;
                align |= Align.Top;

                _sourceRect.Height = (Int32)Math.Ceiling((Single)rect.Height / scale.Y);

                position.Y = rect.Top;

                if (height == 0)
                {
                    height = _sourceRect.Height;
                }
            }

            if (parameters.HasKey("ClipWidth"))
            {
                Point clipWidth = parameters.ParsePoint("ClipWidth");

                _sourceRect.X = clipWidth.X;
                _sourceRect.Width = clipWidth.Y;

                if (width == 0)
                {
                    width = _sourceRect.Width;
                }
            }

            if (IsAlign(align, Align.CutHorz))
            {
                Rectangle rect = FindElementRectangle(parameters, GraphicsHelper.PointFromVector2(areaSize), scale, offset);

                align &= Align.Vert;
                align |= Align.Left;

                _sourceRect.Width = (Int32)Math.Ceiling((Single)rect.Width / scale.X);

                position.X = rect.Left;

                if (width == 0)
                {
                    width = _sourceRect.Width;
                }
            }

            if (width == 0)
            {
                width = textureObj.Width;
            }

            if (height == 0)
            {
                height = textureObj.Height;
            }

            _texture = textureObj;
            _color = parameters.FindColorWrapper("Color");

            Scale = new Vector2(width / (Single)_sourceRect.Width, height / (Single)_sourceRect.Height) * Scale;

            Point size = GraphicsHelper.PointFromVector2(new Vector2(_sourceRect.Width, _sourceRect.Height) * Scale);

            if (size.X == 0)
            {
                size.X = 1;
                Scale.X = 1 / (Single)_sourceRect.Width;
            }

            if (size.Y == 0)
            {
                size.Y = 1;
                Scale.Y = 1 / (Single)_sourceRect.Height;
            }

            ElementRectangle = RectangleFromAlignAndSize(position, size, align, offset);

            return true;
        }
    }
}
