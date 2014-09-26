// SITANA - Copyright (C) The Sitana Team.
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System.Collections.Generic;
using Microsoft.Xna.Framework.Audio;
using System;

namespace Sitana.Framework.Content
{
    public class SoundEffectContainer<T>
    {
        private Dictionary<T, SoundEffect> _soundEffects = new Dictionary<T, SoundEffect>();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="contentLoader"></param>
        public SoundEffectContainer()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="path"></param>
        public void Add(T key, String path)
        {
            _soundEffects.Add(key, ContentLoader.Current.Load<SoundEffect>(path));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public SoundEffect At(T key)
        {
            SoundEffect sound;

            if (_soundEffects.TryGetValue(key, out sound))
            {
                return sound;
            }

            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public Boolean Play(T key)
        {
            SoundEffect sound = At(key);

            if (sound != null)
            {
                sound.Play(1, 0, 0);
                return true;
            }

            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public SoundEffectInstance CreateInstance(T key)
        {
            SoundEffect sound = At(key);

            if (sound != null)
            {
                return sound.CreateInstance();
            }

            return null;
        }
    }
}
