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
using Microsoft.Xna.Framework.Audio;

namespace Sitana.Framework.Content
{
    /// <summary>
    /// 
    /// </summary>
    public class SoundControler
    {
        #region Singleton
        private static SoundControler _instance = new SoundControler();

        public static SoundControler Instance
        {
            get
            {
                return _instance;
            }
        }
        #endregion

        /// <summary>
        /// 
        /// </summary>
        public Single MasterVolume
        {
            get
            {
                return MediaHelper.VolumeToLinear(SoundEffect.MasterVolume);
            }

            set
            {
                SoundEffect.MasterVolume = MediaHelper.LinearToVolume(value);
            }
        }
    }
}
