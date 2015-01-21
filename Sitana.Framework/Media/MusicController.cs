// SITANA - Copyright (C) The Sitana Team.
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using Microsoft.Xna.Framework.Media;
using System;
using System.Threading;
using Sitana.Framework.Cs;

namespace Sitana.Framework.Content
{
    public partial class MusicController: Singleton<MusicController>
    {
        private Single _masterVolume = 1;

        public Single MasterVolume
        {
            get
            {
                return MediaHelper.VolumeToLinear(_masterVolume);
            }

            set
            {
                _masterVolume = MediaHelper.LinearToVolume(value);
				Microsoft.Xna.Framework.Media.MediaPlayer.Volume = _masterVolume;
            }
        }

        private void InitializeCommon()
        {
            //_masterVolume = MediaPlayer.Volume;
        }

        public void OnDeactivate()
        {

        }

        public void Play(String path, Boolean startOver)
        {

            try
            {
                Song song = ContentLoader.Current.Load<Song>(path);
                Play(song);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void Stop()
        {
            try
            {
                if (GameHasControl)
                {
					Microsoft.Xna.Framework.Media.MediaPlayer.Stop();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void Play(Song song)
        {
            if (GameHasControl)
            {
				Microsoft.Xna.Framework.Media.MediaPlayer.IsRepeating = true;
				Microsoft.Xna.Framework.Media.MediaPlayer.Play(song);
            }
        }
    }
}

