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
using Sitana.Framework.Input;
using System.Collections.Generic;

namespace Sitana.Framework.Gui
{
    public class Slider : GuiElement
    {
        enum Sound : int
        {
            Pushed = 0,
            Change = 1,
            Count
        }

        private Color _thumbColor;
        private Color _trackColor;

        private Texture2D _trackTexture;
        private Texture2D _thumbTexture;

        private Vector2 _trackPosition;
        private Vector2 _thumbPosition;

        private Vector2 _trackScale;
        private Vector2 _thumbScale;

        private Single _value = 0.0f;

        private Point _range;

        private Int32 _pointerId = PointerInfo.InvalidPointerId;
        private Single _pointerLastPosition = 0;

        private Single _touchMargin = 0;

        private Action _onChangedAction = null;

        private SoundEffectContainer<Sound> _sounds;

        public Int32 Value
        {
            get
            {
                return (Int32)(_value * (_range.Y - _range.X) + _range.X + 0.5);
            }

            set
            {
                _value = Math.Max(0, (Single)(value - _range.X) / (_range.Y - _range.X));
            }
        }

        private Single ValueForDisplay
        {
            get
            {
                return (Single)(Value - _range.X) / (_range.Y - _range.X);
            }
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

            Vector2 thumbOffset = Vector2.Zero;

            thumbOffset.X = (ElementRectangle.Width - (Int32)(_thumbScale.X * _thumbTexture.Width)) * ValueForDisplay;

            Color thumbColor = ComputeColorWithTransition(transition, _thumbColor);
            Color trackColor = ComputeColorWithTransition(transition, _trackColor);

            Vector2 offset = ComputeOffsetWithTransition(transition);

            spriteBatch.Draw(_trackTexture, _trackPosition + offset, null, trackColor, 0, Vector2.Zero, _trackScale, SpriteEffects.None, 0);
            spriteBatch.Draw(_thumbTexture, _thumbPosition + offset + thumbOffset, null, thumbColor, 0, Vector2.Zero, _thumbScale, SpriteEffects.None, 0);
        }

        /// <summary>
        /// Updates element state.
        /// </summary>
        /// <param name="gameTime">Game time.</param>
        /// <param name="screenState">Screen state.</param>
        public override Boolean Update(TimeSpan gameTime, Screen.ScreenState screenState)
        {
            if (screenState != Screen.ScreenState.Visible)
            {
                return false;
            }

            Vector2 thumbSize = new Vector2((Single)_thumbTexture.Width * _thumbScale.X, (Single)_thumbTexture.Height * _thumbScale.Y);

            Vector2 thumbOffset = Vector2.Zero;

            thumbOffset.X = (ElementRectangle.Width - (Int32)(_thumbScale.X * _thumbTexture.Width)) * ValueForDisplay;

            Vector2 thumbPosition = thumbOffset + _thumbPosition;

            if (_pointerId == PointerInfo.InvalidPointerId)
            {
                return AnalyzeNoPushed(thumbPosition, thumbSize);
            }
            else
            {
                AnalyzePushed(thumbPosition, thumbSize);
                return true;
            }
        }

        private Boolean AnalyzeNoPushed(Vector2 thumbPosition, Vector2 thumbSize)
        {
            Vector2 margin = new Vector2(_touchMargin);

            Vector2 thumbTopLeft = thumbPosition - margin;
            Vector2 thumbBottomRight = thumbPosition + thumbSize + margin;

            for (Int32 idx = 0; idx < InputHandler.Current.PointersState.Count; ++idx)
            {
                var ptr = InputHandler.Current.PointersState[idx];

                if (ptr.State == PointerInfo.PressState.Pressed)
                {
                    if (ptr.Position.X > thumbTopLeft.X && ptr.Position.X < thumbBottomRight.X &&
                         ptr.Position.Y > thumbTopLeft.Y && ptr.Position.Y < thumbBottomRight.Y)
                    {
                        _pointerId = ptr.PointerId;
                        _pointerLastPosition = ptr.Position.X;

                        InputHandler.Current.PointersState.Remove(ptr);
                        _sounds.Play(Sound.Pushed);
                        return true;
                    }
                }
            }

            return false;
        }

        private void AnalyzePushed(Vector2 thumbPosition, Vector2 thumbSize)
        {
            for (Int32 idx = 0; idx < InputHandler.Current.PointersState.Count; ++idx)
            {
                var ptr = InputHandler.Current.PointersState[idx];

                if (ptr.PointerId == _pointerId)
                {
                    if (ptr.State == PointerInfo.PressState.Moved)
                    {
                        Single move = ptr.Position.X - _pointerLastPosition;

                        Single range = (Single)ElementRectangle.Width - thumbSize.X;

                        move = move / range;

                        Int32 lastValue = Value;

                        _value = Math.Max(0, Math.Min(1, _value + move));
                        _pointerLastPosition = ptr.Position.X;

                        if (lastValue != Value)
                        {
                            if (_onChangedAction != null)
                            {
                                _onChangedAction.Invoke();
                            }

                            _sounds.Play(Sound.Change);
                        }

                        InputHandler.Current.PointersState.Remove(ptr);                        return;
                    }
                }
            }

            _pointerId = PointerInfo.InvalidPointerId;
            Value = Value;
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
            String directory = initParams.Directory;
            Vector2 scale = initParams.Scale;
            Vector2 areaSize = initParams.AreaSize;
            Vector2 offset = initParams.Offset;

            // First unserialize base parameters.
            if (!base.Initialize(initParams))
            {
                return false;
            }

            Point position1 = ParsePosition(parameters, "X1", "Y", GraphicsHelper.PointFromVector2(areaSize), scale);
            Point position2 = ParsePosition(parameters, "X2", "Y", GraphicsHelper.PointFromVector2(areaSize), scale);

            Point range = parameters.ParsePoint("Range");

            if (range.X == range.Y)
            {
                throw new Exception("Slider cannot have zero length range.");
            }

            _range = range;

            Align valign = parameters.AsAlign("", "Valign");

            _trackTexture = ContentLoader.Current.Load<Texture2D>(parameters.AsString("Track"));
            _thumbTexture = ContentLoader.Current.Load<Texture2D>(parameters.AsString("Thumb"));

            _thumbColor = parameters.AsColor("ThumbColor");
            _trackColor = parameters.AsColor("TrackColor");

            Single thumbHeight = (Single)parameters.AsInt32("ThumbHeight");
            Single thumbWidth = (Single)parameters.AsInt32("ThumbWidth");
            Single trackHeight = (Single)parameters.AsInt32("TrackSize");

            Point thumbSize = GraphicsHelper.PointFromVector2(new Vector2(thumbWidth, thumbHeight) * scale);
            Point trackSize = GraphicsHelper.PointFromVector2(new Vector2(position2.X - position1.X, trackHeight * scale.Y));

            _thumbScale = GraphicsHelper.CalculateScale(_thumbTexture, thumbSize);
            _trackScale = GraphicsHelper.CalculateScale(_trackTexture, trackSize);

            Single trackOffset = (thumbSize.Y - trackSize.Y) / 2;

            Point size = new Point(position2.X - position1.X, thumbSize.Y);

            ElementRectangle = RectangleFromAlignAndSize(position1, size, valign, offset);

            _trackPosition = new Vector2(ElementRectangle.X, ElementRectangle.Y + trackOffset);
            _thumbPosition = new Vector2(ElementRectangle.X, ElementRectangle.Y);

            _touchMargin = (Single)parameters.AsInt32("TouchMargin") * Scale.X;

            Value = parameters.AsInt32("Value");

            _onChangedAction = parameters.AsAction("OnChanged", this);

            _sounds = new SoundEffectContainer<Sound>();

            String soundPath = parameters.AsString("SoundPushed");

            if (!String.IsNullOrWhiteSpace(soundPath))
            {
                _sounds.Add(Sound.Pushed, soundPath);
            }

            soundPath = parameters.AsString("SoundChange");

            if (!String.IsNullOrWhiteSpace(soundPath))
            {
                _sounds.Add(Sound.Change, soundPath);
            }

            return true;
        }
    }
}