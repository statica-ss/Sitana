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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Ebatianos;
using Ebatianos.Content;

namespace Ebatianos.Gui
{
    public class Background : GuiElement
    {
        static Dictionary<String, Vector2> _savedStates = new Dictionary<String, Vector2>();

        private Texture2D _texture;
        private Color _color = Color.White;

        private Vector2 _size;
        private Vector2 _position;
        private Vector2 _direction;

        private String _restoreState = null;

        public Background()
        {

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
            Vector2 areaSize = initParams.AreaSize;

            if (!parameters.HasKey("Order"))
            {
                parameters.Add("Order", "Background");
            }

            // First unserialize base parameters.
            if (!base.Initialize(initParams))
            {
                return false;
            }

            String texture = parameters.AsString("Texture");
            Int32 horzSpeed = parameters.AsInt32("HorizontalSpeed");
            Int32 vertSpeed = parameters.AsInt32("VerticalSpeed");

            _color = parameters.AsColor("Color");

            String restoreState = parameters.AsString("RestoreState");

            _texture = ContentLoader.Current.Load<Texture2D>(texture);

            _size = GraphicsHelper.Vector2FromPoint(
               GraphicsHelper.TextureSize(_texture)
               );

            Point screenSize = GraphicsHelper.PointFromVector2(areaSize);

            _position = new Vector2(0, 0);
            _direction = new Vector2((Single)horzSpeed, (Single)vertSpeed);

            if (horzSpeed != 0)
            {
                Scale = new Vector2((Single)screenSize.Y / (Single)_texture.Height, (Single)screenSize.Y / (Single)_texture.Height);
            }
            else if (vertSpeed != 0)
            {
                Scale = new Vector2((Single)screenSize.X / (Single)_texture.Width, (Single)screenSize.X / (Single)_texture.Width);
            }
            else
            {
                Scale = new Vector2((Single)screenSize.X / (Single)_texture.Width, (Single)screenSize.Y / (Single)_texture.Height);
            }

            if (restoreState != null)
            {
                _savedStates.TryGetValue(restoreState, out _position);
            }

            return true;
        }

        /// <summary>
        /// Draws GuiElement.
        /// </summary>
        /// <param name="spriteBatch">SpriteBatch object used to render textures and texts.</param>
        /// <param name="color">Color to multiply all contents by.</param>
        /// <param name="offset">Offset to move element by.</param>
        public override void Draw(Int32 level, SpriteBatch spriteBatch, Vector2 topLeft, Single transition)
        {
            if (!DrawLevel(level))
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

            if (_color.A == 0)
            {
                return;
            }

            Vector2 rescale = Scale;

            if (_direction.X > 0)
            {
                spriteBatch.Draw(_texture, new Vector2(-_position.X, 0), null, _color * transition, 0, Vector2.Zero, rescale, SpriteEffects.None, 0);
                spriteBatch.Draw(_texture, new Vector2(_texture.Width * rescale.X - _position.X, 0), null, _color * transition, 0, Vector2.Zero, rescale, SpriteEffects.None, 0);
            }
            else if (_direction.Y > 0)
            {
                spriteBatch.Draw(_texture, new Vector2(0, -_position.Y), null, _color * transition, 0, Vector2.Zero, rescale, SpriteEffects.None, 0);
                spriteBatch.Draw(_texture, new Vector2(0, _texture.Height * rescale.Y - _position.Y), null, _color * transition, 0, Vector2.Zero, rescale, SpriteEffects.None, 0);
            }
            else
            {
                spriteBatch.Draw(_texture, topLeft, null, _color * transition, 0, Vector2.Zero, rescale, SpriteEffects.None, 0);
            }
        }

        /// <summary>
        /// Updates element state.
        /// </summary>
        public override Boolean Update(TimeSpan gameTime, Screen.ScreenState screenState)
        {
            Boolean redraw = false;

            if (_direction != Vector2.Zero)
            {
                Vector2 rescale = Scale;

                _position += _direction * (Single)gameTime.TotalSeconds;

                if (_position.X >= _size.X * rescale.X)
                {
                    _position.X -= _size.X * rescale.X;
                }

                if (_position.Y >= _size.Y * rescale.Y)
                {
                    _position.Y -= _size.Y * rescale.Y;
                }

                if (_position.X < 0)
                {
                    _position.X += _size.X * rescale.X;
                }

                if (_position.Y < 0)
                {
                    _position.Y += _size.Y * rescale.Y;
                }

                if (_restoreState != null)
                {
                    _savedStates.Remove(_restoreState);
                    _savedStates.Add(_restoreState, _position);
                }

                redraw = true;
            }

            return redraw;
        }
    }
}
