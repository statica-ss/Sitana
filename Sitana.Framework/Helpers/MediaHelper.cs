// SITANA - Copyright (C) The Sitana Team.
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
namespace Sitana.Framework.Content
{
    public static class MediaHelper
    {
        private const float _defaultExponent = 2;

        /// <summary>
        /// Converts volume value to linear value.
        /// </summary>
        /// <param name="volume">Volume value from 0 to 1.</param>
        /// <returns></returns>
        public static float VolumeToLinear(float volume)
        {
            return (float)(Math.Pow(volume, 1 / _defaultExponent) + 0.001);
        }

        /// <summary>
        /// Converts linear value to volume value.
        /// </summary>
        /// <param name="linear">Linear value from 0 to 1.</param>
        /// <returns></returns>
        public static float LinearToVolume(float linear)
        {
            return (float)(Math.Pow(linear, _defaultExponent));
        }
    }
}
