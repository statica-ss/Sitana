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
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;

namespace Ebatianos.Input
{
    public class MouseInputHandler : InputHandler
    {
        private KeyboardState _keyboardState;
        private PointerInfo.PressState _pressState = PointerInfo.PressState.Invalid;
        private Int32 _pointerId = 1;

        private Vector2 _lastPosition = Vector2.Zero;

        private GestureType _currentGesture = GestureType.None;

        private const Single _maxMoveToTap = 16;

        private DateTime _downTime;

        private KeyboardState _oldKeyState;
        private KeyboardState _newKeyState = new KeyboardState();

        /// <summary>
        /// Constructs input handler object.
        /// </summary>
        public MouseInputHandler()
        {
            PointersState = new List<PointerInfo>();
        }

        /// <summary>
        /// Updates input state.
        /// </summary>
        protected override void UpdateInternal()
        {
            _oldKeyState = _newKeyState;   

            _keyboardState = Keyboard.GetState();
            _newKeyState = _keyboardState;

            MouseState mouseState = Mouse.GetState();

            PointerInfo.PressState newPressState = PointerInfo.PressState.Invalid;
         
            if (mouseState.LeftButton == ButtonState.Pressed)
            {
                if (_pressState == PointerInfo.PressState.Pressed || _pressState == PointerInfo.PressState.Moved)
                {
                    newPressState = PointerInfo.PressState.Moved;
                }
                else
                {
                    newPressState = PointerInfo.PressState.Pressed;
                }
            }
            else
            {
                if (_pressState == PointerInfo.PressState.Pressed || _pressState == PointerInfo.PressState.Moved)
                {
                    newPressState = PointerInfo.PressState.Released;
                }
                else
                {
                    newPressState = PointerInfo.PressState.Invalid;
                }
            }

            PointersState.Clear();
            PointersState.Add(new PointerInfo(_pointerId, (new Vector2(mouseState.X, mouseState.Y) - Offset) * Scale, newPressState));

            _pressState = newPressState;

            switch(_pressState)
            {
                case PointerInfo.PressState.Pressed:
                    _currentGesture = GestureType.Tap;
                    _lastPosition = PointersState[0].Position;
                    _downTime = DateTime.Now;
                    break;

                case PointerInfo.PressState.Moved:
                    AnalyzeGesture(PointersState[0].Position, false);
                    break;

                case PointerInfo.PressState.Released:
                    AnalyzeGesture(PointersState[0].Position, true);
                    break;

                case PointerInfo.PressState.Invalid:
                    _currentGesture = GestureType.None;
                    break;
            }
        }

        private void AnalyzeGesture(Vector2 position, Boolean released)
        {
            Vector2 move = position - _lastPosition;

            if ( _currentGesture == GestureType.Tap && GestureEnabled(GestureType.Tap))
            {
                if ( Math.Abs(move.X) > _maxMoveToTap || Math.Abs(move.Y) > _maxMoveToTap )
                {
                    _currentGesture = GestureType.FreeDrag;
                }
                else if ( released )
                {
                    if ((DateTime.Now - _downTime).TotalSeconds > 0.75)
                    {
                        _currentGesture = GestureType.None;
                        return;
                    }

                    GestureSample sample = new GestureSample(GestureType.Tap, TimeSpan.FromTicks(0), _lastPosition, Vector2.Zero, Vector2.Zero, Vector2.Zero);
                    ProcessGesture(ref sample);
                }
            }

            if ( _currentGesture == GestureType.FreeDrag )
            {
                if ( GestureEnabled(GestureType.HorizontalDrag) )
                {
                    if ( Math.Abs(move.X) > Math.Abs(move.Y) && Math.Abs(move.X) > _maxMoveToTap)
                    {
                        _currentGesture = GestureType.HorizontalDrag;
                    }
                }
            }

            if ( _currentGesture == GestureType.FreeDrag )
            {
                if ( GestureEnabled(GestureType.VerticalDrag) )
                {
                    if ( Math.Abs(move.Y) >= Math.Abs(move.X) && Math.Abs(move.Y) > _maxMoveToTap)
                    {
                        _currentGesture = GestureType.VerticalDrag;
                    }
                }
            }

            if ( _currentGesture == GestureType.HorizontalDrag )
            {
                if (move.X != 0)
                {
                    GestureSample sample = new GestureSample(released ? GestureType.Flick : GestureType.HorizontalDrag, TimeSpan.FromTicks(0), position, Vector2.Zero, new Vector2(released ? move.X * 10 : move.X, 0), Vector2.Zero);
                    ProcessGesture(ref sample);
                    _lastPosition = position;
                }
            }

            if ( _currentGesture == GestureType.VerticalDrag )
            {
                if (move.Y != 0)
                {
                    GestureSample sample = new GestureSample(released ? GestureType.Flick : GestureType.VerticalDrag, TimeSpan.FromTicks(0), position, Vector2.Zero, new Vector2(0, released ? move.Y * 10 : move.Y), Vector2.Zero);
                    ProcessGesture(ref sample);
                    _lastPosition = position;
                }
            }

            if ( _currentGesture == GestureType.FreeDrag )
            {
                if (move != Vector2.Zero)
                {
                    GestureSample sample = new GestureSample(released ? GestureType.Flick : GestureType.FreeDrag, TimeSpan.FromTicks(0), position, Vector2.Zero,released ? move * 10 : move, Vector2.Zero);
                    ProcessGesture(ref sample);
                    _lastPosition = position;
                }
            }

            if ( released )
            {
                _currentGesture = GestureType.None;
            }
        }

        private Boolean GestureEnabled(GestureType type)
        {
            return (EnabledGestures & type) != GestureType.None;
        }

        /// <summary>
        /// Returns state of a key.
        /// </summary>
        /// <param name="keyId">Key id.</param>
        /// <returns>Key state.</returns>
        public override KeyInfo.PressState GetKeyState(Keys keyId)
        {
            Boolean pressed = _keyboardState.IsKeyDown(keyId);
         
            if ( pressed )
            {
                if ( _oldKeyState.IsKeyDown(keyId) )
                {
                    return KeyInfo.PressState.Hold;
                }
                else
                {
                    return KeyInfo.PressState.Pressed;
                }
            }

            return KeyInfo.PressState.None;
        }

        public override void ClearKeys()
        {
            _keyboardState = new KeyboardState();
        }
    }
}
