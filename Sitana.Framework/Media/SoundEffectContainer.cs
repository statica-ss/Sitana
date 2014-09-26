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
using Microsoft.Xna.Framework.Audio;

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
