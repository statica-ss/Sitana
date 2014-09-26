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
using Microsoft.Xna.Framework.Media;
using Sitana.Framework;
using Sitana.Framework.Content;
using System.Collections.Generic;

namespace Sitana.Framework.Gui
{
    public class MusicPlayer : GuiElement
    {
        private static MusicPlayer _musicOwner;

        private Boolean _musicPlayed = false;
        private Boolean _startOver = false;

        private String _musicPath;

        private Screen _owner;

        private Double _waitToTryAgain = 0;

        public override Boolean Update(TimeSpan gameTime, Screen.ScreenState screenState)
        {
            if (_waitToTryAgain > 0)
            {
                _waitToTryAgain -= gameTime.TotalSeconds;
                if (_waitToTryAgain <= 0)
                {
                    _musicPlayed = false;
                }
            }
            else if (!_musicPlayed && (screenState == Screen.ScreenState.Visible || screenState == Screen.ScreenState.TransitionIn) && _owner.Transition > 0.25)
            {
                if (ContentLoader.Current.IsContentLoaded<Song>(_musicPath))
                {
                    _musicPlayed = true;
                    _musicOwner = this;
                    MusicControler.Instance.Play(_musicPath, _startOver);
                }
                else
                {
                    _waitToTryAgain = 0.25;
                }
            }

            return false;
        }

        /// <summary>
        /// Called when element is removed either by screen or whole owner screen is removed.
        /// </summary>
        public override void OnRemoved()
        {
            if (_musicOwner == this)
            {
                MusicControler.Instance.Stop();
            }
        }

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
            Screen owner = initParams.Owner;

            // First unserialize base parameters.
            if (!base.Initialize(initParams))
            {
                return false;
            }            

            if (MusicControler.Instance == null)
            {
                return false;
            }

            _musicPath = parameters.AsString("Path");
            _startOver = parameters.AsBoolean("StartOver");
            _owner = owner;

            return true;
        }
    }
}

