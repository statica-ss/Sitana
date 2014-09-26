// SITANA - Copyright (C) The Sitana Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using Microsoft.Xna.Framework.Audio;
using System;

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
