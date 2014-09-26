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

namespace Ebatianos.Content
{
    public static class MediaHelper
    {
        private const float _defaultExponent = 2;

        /// <summary>
        /// Converts volume value to linear value.
        /// </summary>
        /// <param name="volume">Volume value from 0 to 1.</param>
        /// <returns></returns>
        public static Single VolumeToLinear(Single volume)
        {
            return (Single)(Math.Pow(volume, 1 / _defaultExponent) + 0.001);
        }

        /// <summary>
        /// Converts linear value to volume value.
        /// </summary>
        /// <param name="linear">Linear value from 0 to 1.</param>
        /// <returns></returns>
        public static Single LinearToVolume(Single linear)
        {
            return (Single)(Math.Pow(linear, _defaultExponent));
        }
    }
}
