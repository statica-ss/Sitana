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

namespace Sitana.Framework.Content
{
    public partial class MusicController
    {
        protected bool GameHasControl
        {
            get
            {
                return MediaPlayer.GameHasControl;
            }
        }

        public void Initialize()
        {
            InitializeCommon();
        }

        public void OnActivate()
        {
            if (!GameHasControl && MediaPlayer.State == MediaState.Playing)
            {
                Stop();
            }
        }
    }
}

