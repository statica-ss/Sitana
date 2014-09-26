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
    /// <summary>
    /// Handles input from touch panel.
    /// </summary>
    public class TouchInputHandler : InputHandler
    {
        private Boolean _escape = false;
		private Boolean _menu = false;

        /// <summary>
        /// Constructs input handler object.
        /// </summary>
        public TouchInputHandler()
        {
            PointersState = new List<PointerInfo>();
            PointersState.Capacity = 8;
        }

        /// <summary>
        /// Updates input state.
        /// </summary>
        protected override void UpdateInternal()
        {
			_escape = EscapePressed;
			_menu = MenuPressed;

            // Remove all states for keys and pointers.
            PointersState.Clear();

            // Collect touch points.
            TouchCollection touch = TouchPanel.GetState();

            // Iterate through touch points.
            for (Int32 idx = 0; idx < touch.Count; ++idx)
            {
                var point = touch[idx];

                PointerInfo.PressState pressState = PointerInfo.PressState.Invalid;  // State of pointer.

                // Convert touch point state into pointer state.
                switch (point.State)
                {
                    case TouchLocationState.Pressed:
                        pressState = PointerInfo.PressState.Pressed;
                        break;

                    case TouchLocationState.Moved:
                        pressState = PointerInfo.PressState.Moved;
                        break;

                    case TouchLocationState.Released:
                        pressState = PointerInfo.PressState.Released;
                        break;
                }

                // Add new pointer info for every touch point.
                PointersState.Add(new PointerInfo(point.Id, (point.Position - Offset) * Scale, pressState));
            }

            #if WINDOWS_PHONE
                // Get state of phone's back button and convert it to state of Escape key.
                switch (GamePad.GetState(PlayerIndex.One).Buttons.Back)
                {
                    case ButtonState.Pressed:
                        _escape = true;
                        break;
                }
            #endif

            while(TouchPanel.IsGestureAvailable)
            {
                GestureSample sample = TouchPanel.ReadGesture();

                if ( Offset != Vector2.Zero || Scale != 1 )
                {
                    sample = new GestureSample(sample.GestureType, sample.Timestamp, (sample.Position - Offset) * Scale, (sample.Position2 - Offset) * Scale, sample.Delta * Scale, sample.Delta2 * Scale);
                }
                ProcessGesture(ref sample);
            }
        }

        /// <summary>
        /// Returns state of a key.
        /// </summary>
        /// <param name="keyId">Key id.</param>
        /// <returns>Key state.</returns>
        public override KeyInfo.PressState GetKeyState(Keys keyId)
        {
            if (keyId == Keys.Escape && _escape)
            {
                return KeyInfo.PressState.Pressed;
            }

			if (keyId == Keys.LeftAlt && _menu)
			{
				return KeyInfo.PressState.Pressed;
			}

            return KeyInfo.PressState.None;
        }

        public override void ClearKeys()
        {
            _escape = false;
            _menu = false;
        }
    }
}
