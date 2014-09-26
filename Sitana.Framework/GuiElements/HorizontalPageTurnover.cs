// /// This file is a part of the EBATIANOS.ESSENTIALS class library.
// /// (c)2013-2014 EBATIANO'S a.k.a. Sebastian Sejud. All rights reserved.
// ///
// /// THIS SOURCE FILE IS THE PROPERTY OF EBATIANO'S A.K.A. SEBASTIAN SEJUD 
// /// AND IS NOT TO BE RE-DISTRIBUTED BY ANY MEANS WHATSOEVER WITHOUT 
// /// THE EXPRESSED WRITTEN CONSENT OF EBATIANO'S A.K.A. SEBASTIAN SEJUD.
// ///
// /// THIS SOURCE CODE CAN ONLY BE USED UNDER THE TERMS AND CONDITIONS OUTLINED
// /// IN THE EBATIANOS.ESSENTIALS LICENSE AGREEMENT. 
// /// EBATIANO'S A.K.A. SEBASTIAN SEJUD GRANTS TO YOU (ONE SOFTWARE DEVELOPER) 
// /// THE LIMITED RIGHT TO USE THIS SOFTWARE ON A SINGLE COMPUTER.
// ///
// /// CONTACT INFORMATION:
// /// contact@ebatianos.com
// /// www.ebatianos.com/essentials-library
// /// 
// ///---------------------------------------------------------------------------
//
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Sitana.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;

namespace Sitana.Framework.Gui
{
    public class HorizontalPageTurnover: GuiElement
    {
        private Single _offset = 0;
        private Single _moveToTurn;

        private Action _onNextPage;
        private Action _onPrevPage;

        private Double _time = 0;

        /// <summary>
        /// Initializes label from parameters.
        /// </summary>
        /// <param name="parameters">Parameters from xml node.</param>
        /// <param name="contentLoader">Content loader.</param>
        /// <param name="owner">Owner screen.</param>
        /// <returns>True when succeeded.</returns>
        /// <summary>1
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

            // First unserialize base parameters.
            if (!base.Initialize(initParams))
            {
                return false;
            }

            _moveToTurn = parameters.AsSingle("TurnGestureSize") * scale.X;

            _onNextPage = parameters.AsAction("OnNextPage", this);
            _onPrevPage = parameters.AsAction("OnPrevPage", this);

            InstallGestureHandler(GestureAdditionalType.TouchDown, GestureType.None, OnTouchDown);
            InstallGestureHandler(GestureAdditionalType.TouchUp, GestureType.None, OnTouchUp);
            InstallGestureHandler(GestureAdditionalType.Native, GestureType.Flick, OnTouchUp);

            InstallGestureHandler(GestureAdditionalType.Native, GestureType.HorizontalDrag, OnHorizontalDrag);

            return true;
        }

        private void OnTouchDown(Object sender, GestureEventArgs args)
        {
            _offset = 0;
            InputHandler.Current.PointerInvalidated += PointerInvalidated;
        }

        private void PointerInvalidated(Object sender, EventArgs args)
        {
            _offset = 0;
            InputHandler.Current.PointerInvalidated -= PointerInvalidated;
        }

        private void OnTouchUp(Object sender, GestureEventArgs args)
        {
            if ( _time>0 && Math.Abs(_offset) > _moveToTurn )
            {
                if ( _offset > 0 )
                {
                    _onPrevPage.Invoke();
                }
                else
                {
                    _onNextPage.Invoke();
                }
            }

            _offset = 0;
        }

        private void OnHorizontalDrag(Object sender, GestureEventArgs args)
        {
            _offset += args.Sample.Delta.X;
            args.Handled = true;

            _time = 0.1;
        }

        public override bool Update(TimeSpan gameTime, Screen.ScreenState screenState)
        {
            if (Math.Abs(_offset) > _moveToTurn)
            {
                _time -= gameTime.TotalSeconds;
            }

            return base.Update(gameTime, screenState);
        }
    }
}

