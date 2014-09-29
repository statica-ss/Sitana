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
        protected Song _currentlyPlayed;
        protected Song _currentlyPlayedSong;

        private Song _nextPlayedSong;
        private Boolean _playNextSong = false;

        private Single _masterVolume = 1;

        private Single _volumeModifier = 0;

        private Object lockObj = new Object();

        public Double FadeTime { get; set; }

        private Single mediaPlayerVolume = 0;

        public Single MasterVolume
        {
            get
            {
                return MediaHelper.VolumeToLinear(_masterVolume);
            }

            set
            {
                _masterVolume = MediaHelper.LinearToVolume(value);
            }
        }

        private void InitializeCommon()
        {
            FadeTime = 0.5;
            _masterVolume = MediaPlayer.Volume;

            MediaPlayer.Volume = 0;
            mediaPlayerVolume = 0;

            // Start the volume update thread.
            var thread = new Thread(Update);
            thread.Name = "MusicContoler.Update";
            thread.IsBackground = true;
            thread.Start();
        }

        

        public void OnDeactivate()
        {

        }

        public void Play(String path, Boolean startOver)
        {

            try
            {
                Song song = ContentLoader.Current.Load<Song>(path);

                if (startOver || song != _currentlyPlayed)
                {
                    _currentlyPlayed = song;

                    if (GameHasControl)
                    {
                        _nextPlayedSong = _currentlyPlayed;
                        _playNextSong = true;
                    }
                }
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
                _currentlyPlayed = null;
                _nextPlayedSong = null;
                _playNextSong = true;
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
                MediaPlayer.IsRepeating = true;
                MediaPlayer.Play(song);
            }
        }

        private void Stop(Object dummy)
        {
            if (GameHasControl)
            {
                MediaPlayer.Stop();
            }
        }

        private void Update()
        {
            const Double updateInterval = 0.10;

            var timer = new AutoResetEvent(false);

            while (true)
            {
                lock (lockObj)
                {
                    if (_playNextSong)
                    {
                        _volumeModifier -= (Single)(updateInterval / FadeTime);

                        if (_volumeModifier <= 0)
                        {
                            _volumeModifier = 0;

                            if (_currentlyPlayedSong != null)
                            {
                                try
                                {
                                    Stop(null);
                                    _currentlyPlayedSong = null;
                                }
                                catch (Exception)
                                {

                                }
                            }

                            if (_nextPlayedSong != null)
                            {
                                MediaPlayer.Volume = 0;

                                try
                                {
                                    Play(_nextPlayedSong);

                                    _currentlyPlayedSong = _nextPlayedSong;
                                    _nextPlayedSong = null;
                                    _playNextSong = false;
                                }
                                catch (Exception ex)
                                {
                                    System.Diagnostics.Debug.WriteLine(ex.ToString());
                                }
                            }
                        }
                    }
                    else if (_currentlyPlayedSong != null)
                    {
                        if (_volumeModifier < 1)
                        {
                            _volumeModifier += (Single)(updateInterval / FadeTime);

                            if (_volumeModifier > 1)
                            {
                                _volumeModifier = 1;
                            }
                        }
                    }

                    Single newVolume = MediaHelper.LinearToVolume(_volumeModifier) * _masterVolume;

                    if (mediaPlayerVolume != newVolume)
                    {
                        MediaPlayer.Volume = newVolume;
                        mediaPlayerVolume = newVolume;
                    }
                }

                // Wait before trying to set it again as each
                // set can be very very expensive.
                timer.WaitOne((Int32)(updateInterval * 1000));
            }
        }
    }
}

