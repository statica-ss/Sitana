// SITANA - Copyright (C) The Sitana Team.
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using Microsoft.Xna.Framework.Media;
using System;
using System.Threading;
using Sitana.Framework.Cs;
using Sitana.Framework.Content;

namespace Sitana.Framework.Media
{
    public partial class MusicController: Singleton<MusicController>
    {
        private Single _masterVolume = 1;
        private bool _enabled = true;

        private Song _song;

        public Single MasterVolume
        {
            get
            {
                return _masterVolume;//MediaHelper.VolumeToLinear(_masterVolume);
            }

            set
            {
                _masterVolume = value;//MediaHelper.LinearToVolume(value);
                Microsoft.Xna.Framework.Media.MediaPlayer.Volume = _masterVolume;
            }
        }

        public bool Enabled
        {
            get
            {
                return _enabled;
            }

            set
            {
                _enabled = value;
                if ( _song != null )
                {
                    if (GameHasControl)
                    {
                        if (_enabled)
                        {
                            if (Microsoft.Xna.Framework.Media.MediaPlayer.State == MediaState.Stopped)
                            {
                                Microsoft.Xna.Framework.Media.MediaPlayer.Play(_song);
                            }
                            else if (Microsoft.Xna.Framework.Media.MediaPlayer.State == MediaState.Paused)
                            {
                                Microsoft.Xna.Framework.Media.MediaPlayer.Resume();
                            }
                        }
                        else
                        {
                            Microsoft.Xna.Framework.Media.MediaPlayer.Pause();
                        }
                    }
                }
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

                _song = song;
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
            Microsoft.Xna.Framework.Media.MediaPlayer.IsRepeating = true;

            if (GameHasControl && Enabled)
            {
				Microsoft.Xna.Framework.Media.MediaPlayer.Play(song);
            }
        }
    }
}

