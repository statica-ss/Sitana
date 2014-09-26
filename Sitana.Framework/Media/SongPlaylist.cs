// SITANA - Copyright (C) The Sitana Team.
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System.Collections.Generic;
using Microsoft.Xna.Framework.Media;
using System;

namespace Sitana.Framework.Content
{
    public class SongPlaylist
    {
        private List<Song> _songs = new List<Song>();

        public SongPlaylist()
        {

        }

        public Song this[Int32 index]
        {
            get
            {
                return _songs[index];
            }
        }

        public Int32 Count
        {
            get
            {
                return _songs.Count;
            }
        }

        public void Add(Song song)
        {
            _songs.Add(song);
        }
    }
}
