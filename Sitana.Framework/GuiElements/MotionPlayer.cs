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
using Ebatianos;
using Ebatianos.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Ebatianos.Gui
{
    public class MotionPlayer : GuiElement
    {
        private MotionPicture _motionPicture;
        private Action _finished;
        private Vector2 _areaSize;
        private Texture2D _onePixelWhite;
        private Screen _owner;
        private Boolean _paintBgColor;

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

            if (_paintBgColor)
            {
                spriteBatch.Draw(_onePixelWhite, Vector2.Zero, null, _motionPicture.BgColor * transition, 0, Vector2.Zero, _areaSize, SpriteEffects.None, 0);
            }

            if (transition < 1 && _owner.State == Screen.ScreenState.TransitionIn)
            {
                _paintBgColor = true;
            }
            else
            {
                _paintBgColor = false;
            }

            _motionPicture.Draw(spriteBatch, ElementRectangle, transition);
        }

        public override Boolean Update(TimeSpan gameTime, Screen.ScreenState screenState)
        {
            _owner.BgColor = _motionPicture.BgColor;

            if (screenState == Screen.ScreenState.Visible)
            {
                if (_motionPicture.Playback((Single)gameTime.TotalSeconds))
                {
                    if (_finished != null)
                    {
                        _finished.Invoke();
                    }
                }
            }

            return true;
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
            Vector2 areaSize = initParams.AreaSize;
            Screen owner = initParams.Owner;

            // First unserialize base parameters.
            if (!base.Initialize(initParams))
            {
                return false;
            }

            _motionPicture = ContentLoader.Current.Load<MotionPicture>(parameters.AsString("Path"));

            _motionPicture.InitPlayback();

            _finished = parameters.AsAction("Finished", this);
            _areaSize = areaSize;

            owner.BgColor = _motionPicture.BgColor;

            _onePixelWhite = ContentLoader.Current.Load<Texture2D>(parameters.AsString("BackgroundBlankTexture"));

            ElementRectangle = new Rectangle(0, 0, (Int32)areaSize.X, (Int32)areaSize.Y);

            _owner = owner;
            _paintBgColor = true;
            return true;
        }
    }
}
