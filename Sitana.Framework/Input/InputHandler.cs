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
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input.Touch;

namespace Ebatianos.Input
{
    /// <summary>
    /// Abstract class for classes handling input.
    /// </summary>
    public abstract class InputHandler
    {
        // Currently set handler.
        public static InputHandler Current { set; get; }

        internal GestureReceiverType GestureReceiverType { get; set; }

        // State of pointers.
        public List<PointerInfo> PointersState { get; protected set; }

        private List<RegisteredGesture> _registeredGestures = new List<RegisteredGesture>();

        protected List<Int32> InvalidPointers = new List<Int32>();

        public event EventHandler<GestureEventArgs> PointerInvalidated;

        private Boolean _escapePressed = false;
        private Boolean _menuPressed = false;

        private Object _lockObject = new Object();

        protected GestureType EnabledGestures = GestureType.None;

        public Boolean DisableButtons { get; set; }

        public Boolean EscapePressed
        { 
            get 
            {
                lock (_lockObject) 
                {
                    Boolean escapePressed = _escapePressed;
                    _escapePressed = false;
                    return escapePressed;
                }
            }

            set 
            {
                lock (_lockObject) 
                {
                    _escapePressed = value;
                }
            }
        }

        public Boolean MenuPressed
        { 
            get 
            {
                lock (_lockObject) 
                {
                    Boolean menuPressed = _menuPressed;
                    _menuPressed = false;
                    return menuPressed;
                }
            }

            set 
            {
                lock (_lockObject) 
                {
                    _menuPressed = value;
                }
            }
        }

        /// <summary>
        /// Updates input state.
        /// </summary>
        public void Update()
        {
            UpdateInternal();

            GestureSample sample;

            for ( Int32 idx = 0; idx < PointersState.Count; ++idx )
            {
                var state = PointersState[idx];

                if (state.State == PointerInfo.PressState.Pressed)
                {
                    sample = new GestureSample(GestureType.None, TimeSpan.Zero, state.Position, state.Position, Vector2.Zero, Vector2.Zero);
                    ProcessGesture(GestureAdditionalType.TouchDown, state.PointerId, ref sample);
                }
                else if (state.State == PointerInfo.PressState.Released || state.State == PointerInfo.PressState.Invalid)
                {
                    sample = new GestureSample(GestureType.None, TimeSpan.Zero, state.Position, state.Position, Vector2.Zero, Vector2.Zero);
                    ProcessGesture(GestureAdditionalType.TouchUp, state.PointerId, ref sample);
                }
                else if (state.State == PointerInfo.PressState.Moved)
                {
                    sample = new GestureSample(GestureType.None, TimeSpan.Zero, state.Position, state.Position, Vector2.Zero, Vector2.Zero);
                    ProcessGesture(GestureAdditionalType.TouchMove, state.PointerId, ref sample);
                }

                if ( InvalidPointers.Contains(state.PointerId) )
                {
                    PointerInfo info = state;
                    info.State = PointerInfo.PressState.Invalid;
                    info.Position = new Vector2(-1000, -1000);
                    PointersState[idx] = info;
                    InvalidPointers.Remove(state.PointerId);

                    if ( PointerInvalidated != null )
                    {
                        GestureEventArgs args = new GestureEventArgs();
                        args.PointerId = state.PointerId;

                        PointerInvalidated(this, args);
                    }
                }
            }

            DisableButtons = false;
        }

        internal void InstallGestureHandler(RegisteredGesture gesture)
        {
            if ( gesture.AdditionalType != GestureAdditionalType.Native )
            {
                if ( gesture.Type != GestureType.None )
                {
                    throw new InvalidOperationException("Native gesture type must be None if additional type is other than Native.");
                }
            }

            _registeredGestures.Add(gesture);

            TouchPanel.EnabledGestures = TouchPanel.EnabledGestures | gesture.Type;
            EnabledGestures = EnabledGestures | gesture.Type;
        }

        internal RegisteredGesture InstallGestureHandler(GestureAdditionalType additionalType, GestureType type, GestureReceiverType receiverType, EventHandler<GestureEventArgs> handler)
        {
            if ( additionalType != GestureAdditionalType.Native )
            {
                if ( type != GestureType.None )
                {
                    throw new InvalidOperationException("Native gesture type must be None if additional type is other than Native.");
                }
            }

            RegisteredGesture gesture = new RegisteredGesture(additionalType, type, receiverType, handler);

            _registeredGestures.Add(gesture);

            TouchPanel.EnabledGestures = TouchPanel.EnabledGestures | type;
            EnabledGestures = EnabledGestures | type;

            return gesture;
        }

        internal void UninstallGestureHandler(RegisteredGesture gesture)
        {
            _registeredGestures.Remove(gesture);

            for ( Int32 idx = 0; idx < _registeredGestures.Count; ++idx )
            {
                if ( _registeredGestures[idx].Type == gesture.Type && gesture.Type != GestureType.None)
                {
                    return;
                }
            }

            TouchPanel.EnabledGestures = TouchPanel.EnabledGestures & (~gesture.Type);
            EnabledGestures = EnabledGestures & (~gesture.Type);
        }

        protected void ProcessGesture(ref GestureSample sample)
        {
            ProcessGesture(GestureAdditionalType.Native, 0, ref sample);
        }

        private void ProcessGesture(GestureAdditionalType additionalType, Int32 pointerId, ref GestureSample sample)
        {
            for ( Int32 idx = _registeredGestures.Count-1; idx >= 0; --idx )
            {
                var registeredGesture = _registeredGestures[idx];

                if (registeredGesture.ReceiverType == GestureReceiverType && registeredGesture.Type == sample.GestureType && registeredGesture.AdditionalType == additionalType)
                {
                    var args = new GestureEventArgs()
                        {
                            AdditionalType = additionalType,
                            Sample = sample,
                            Handled = false,
                            PointerId = pointerId
                        };

                    registeredGesture.Handler(this, args);

                    if ( args.Handled && (sample.GestureType == GestureType.HorizontalDrag || sample.GestureType == GestureType.VerticalDrag ))
                    {
                        for ( idx = 0; idx < PointersState.Count; ++idx )
                        {
                            if ( PointersState[idx].Position == sample.Position )
                            {
                                InvalidPointers.Add(PointersState[idx].PointerId);
                                break;
                            }
                        }
                        break;
                    }

                    if (args.Handled)
                    {
                        break;
                    }
                }
            }
        }

        internal void OffsetPointers(Vector2 offset)
        {
            for (Int32 idx = 0; idx < PointersState.Count; ++idx)
            {
                PointerInfo info = PointersState[idx];
                info.Position = info.Position + offset;
                PointersState[idx] = info;
            }
        }

        protected abstract void UpdateInternal();

        // Screen scale for input.
        public Single Scale { get; set; }

        // Offset for input.
        public Vector2 Offset { get; set; }

        /// <summary>
        /// Creates new InputHandler object.
        /// </summary>
        public InputHandler()
        {
            Scale = 1;
            Offset = Vector2.Zero;
        }

        /// <summary>
        /// Returns state of a key.
        /// </summary>
        /// <param name="keyId">Key id.</param>
        /// <returns>Key state.</returns>
        public abstract KeyInfo.PressState GetKeyState(Keys keyId);

        public virtual void ClearKeys()
        {

        }
    }
}
