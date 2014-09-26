/* 
 * This file has been created by
 * Sebastian Sejud (ebatiano)
 * http://ebatiano.blogspot.com
 * 
 * This file is published under CC license
 * You may modify, copy, share and use for commercial and non-commercial projects
 * You have to credit this work in your projects
 * When sharing source code, leave this header
 */
using System;
using Microsoft.Xna.Framework.Media;
using MonoTouch.AudioToolbox;
using MonoTouch.AVFoundation;
using MonoTouch.Foundation;

namespace Ebatianos.Content
{
    public class MusicControlerIos : MusicControler
    {
        AVAudioSession _audioSession;

        protected override Boolean GameHasControl
        {
            get
            {
                try
                {
                    return !_audioSession.OtherAudioPlaying;
                }
                catch
                {
                    return true;
                }
            }
        }

        protected override void Initialize()
        {
            _audioSession = AVAudioSession.SharedInstance();

            NSError audioSessionError = new NSError();
            _audioSession.SetCategory(AVAudioSession.CategoryAmbient, out audioSessionError);

            base.Initialize();
        }

        public override void OnActivate()
        {
            base.OnActivate();

            if (_currentlyPlayedSong != null && !GameHasControl && MediaPlayer.State == MediaState.Playing)
            {
                Stop();
            }
        }
    }
}

