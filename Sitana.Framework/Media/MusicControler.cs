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
using Microsoft.Xna.Framework.Media;
using System.Threading;

namespace Sitana.Framework.Content
{
    public class MusicControler
    {
        #region Singleton
        private static MusicControler _instance;

        public static MusicControler Instance
        {
            get
            {
                return _instance;
            }
        }

        public static void Initialize(Type type)
        {
            if (_instance != null)
            {
				_instance.Stop();
				return;
            }

            _instance = Activator.CreateInstance(type) as MusicControler;
            _instance.Initialize();
        }

        #endregion

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
#if WINDOWS_PHONE
                return MediaHelper.LinearToVolume(_masterVolume);
#else
                return MediaHelper.VolumeToLinear(_masterVolume);
#endif
            }

            set
            {
#if WINDOWS_PHONE
                _masterVolume = MediaHelper.VolumeToLinear(value);
#else
                _masterVolume = MediaHelper.LinearToVolume(value);
#endif
            }
        }

        protected virtual Boolean GameHasControl
        {
            get
            {
                return MediaPlayer.GameHasControl;
            }
        }

        protected virtual void Initialize()
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

        public virtual void OnActivate()
        {

            if (_currentlyPlayedSong != null && GameHasControl && MediaPlayer.State != MediaState.Playing)
            {
                Play(_currentlyPlayedSong);
            }
        }

        public virtual void OnDeactivate()
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

