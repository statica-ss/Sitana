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
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Ebatianos;
using Ebatianos.Content;
using Ebatianos.Input;
using System.Collections.Generic;

namespace Ebatianos.Gui
{
    public class CheckBox : GuiElement
    {
        protected Texture2D _checkedTexture;
        protected Texture2D _uncheckedTexture;
        protected Color _color;
        protected Color _markColor;
        protected Action _changedAction;
        protected Single _touchMargin;

        protected Single _checkedAlpha = 0;

        private SoundEffectContainer<Boolean> _sounds;

        public Boolean Checked { get; set; }

        public override Boolean Update(TimeSpan gameTime, Screen.ScreenState screenState)
        {
            Boolean redraw = base.Update(gameTime, screenState);
            Boolean forceUpdate = false;

            if (SecondInstance != null)
            {
                if (this == FirstInstance)
                {
                    forceUpdate = true;
                }
            }

            if ( !forceUpdate && screenState != Screen.ScreenState.Visible)
            {
                return false;
            }

            Vector2 topLeft = new Vector2(ElementRectangle.X, ElementRectangle.Y) - new Vector2(_touchMargin);
            Vector2 bottomRight = new Vector2(ElementRectangle.Right, ElementRectangle.Bottom) + new Vector2(_touchMargin);

            if (Enabled)
            {
                for (Int32 idx = 0; idx < InputHandler.Current.PointersState.Count; ++idx)
                {
                    var ptr = InputHandler.Current.PointersState[idx];

                    if (ptr.State == PointerInfo.PressState.Pressed)
                    {
                        if (ptr.Position.X > topLeft.X && ptr.Position.X < bottomRight.X &&
                             ptr.Position.Y > topLeft.Y && ptr.Position.Y < bottomRight.Y)
                        {
                            Checked = !Checked;

                            if (_changedAction != null)
                            {
                                _changedAction.Invoke();
                            }

                            _sounds.Play(Checked);

                            InputHandler.Current.PointersState.Remove(ptr);

                            return true;
                        }
                    }
                }
            }

            Single desiredAlpha = Checked ? 1 : 0;
            Single time = (Single)gameTime.TotalSeconds * 6;

            Single oldAlpha = _checkedAlpha;
            _checkedAlpha = (1 - time) * _checkedAlpha + time * desiredAlpha;

            return redraw || oldAlpha != _checkedAlpha;
        }

        public override void Draw(Int32 level, SpriteBatch spriteBatch, Vector2 topLeft, Single transition)
        {
            if (!DrawLevel(level))
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

            Vector2 offset = ComputeOffsetWithTransition(transition);
            Color color = ComputeColorWithTransition(transition, _color) * Opacity;
            Color markColor = ComputeColorWithTransition(transition, _markColor) * Opacity;

			Vector2 position = new Vector2(ElementRectangle.X, ElementRectangle.Y) + offset + topLeft;

            spriteBatch.Draw(_uncheckedTexture, position, null, color, 0, Vector2.Zero, Scale, SpriteEffects.None, 0);
            spriteBatch.Draw(_checkedTexture, position, null, markColor * _checkedAlpha, 0, Vector2.Zero, Scale, SpriteEffects.None, 0);
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

            // First unserialize base parameters.
            if (!base.Initialize(initParams))
            {
                return false;
            }

            Align align = parameters.AsAlign("Align", "Valign");
            Point position = FindPosition(parameters, GraphicsHelper.PointFromVector2(areaSize), scale);

            _checkedTexture = ContentLoader.Current.Load<Texture2D>(parameters.AsString("CheckedImage"));

            _uncheckedTexture = ContentLoader.Current.Load<Texture2D>(parameters.AsString("UncheckedImage"));

            if (_checkedTexture.Width != _uncheckedTexture.Width || _checkedTexture.Height != _uncheckedTexture.Height)
            {
                throw new InvalidOperationException("Checked and unchecked textures must be equal size.");
            }

            _color = parameters.AsColor("Color");
            _markColor = parameters.AsColor("MarkColor");

            Single width = (Single)_checkedTexture.Width * scale.X;
            Single height = (Single)_checkedTexture.Height * scale.Y;

            ElementRectangle = RectangleFromAlignAndSize(position, new Point((Int32)width, (Int32)height), align, offset);

            _changedAction = parameters.AsAction("OnChanged", this);

            _touchMargin = parameters.AsInt32("TouchMargin") * Scale.X;

            Checked = parameters.AsBoolean("Checked");

            if (Checked)
            {
                _checkedAlpha = 1;
            }

            _sounds = new SoundEffectContainer<Boolean>();

            String soundPath = parameters.AsString("SoundChecked");

            if (!String.IsNullOrWhiteSpace(soundPath))
            {
                _sounds.Add(true, soundPath);
            }

            soundPath = parameters.AsString("SoundUnchecked");

            if (!String.IsNullOrWhiteSpace(soundPath))
            {
                _sounds.Add(false, soundPath);
            }

            return true;
        }
    }
}

