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
using Microsoft.Xna.Framework;
using Sitana.Framework.Content;
using Sitana.Framework;
using Sitana.Framework.Gui;
using System.Collections.Generic;

namespace Sitana.Framework.Content.Generators
{
    public class SongPlaylistGenerator : ContentGenerator
    {
        private String _directory;

        public override void Generate()
        {
            SongPlaylist playlist = new SongPlaylist();

            for (Int32 index = 1;; ++index)
            {
                String path = _parameters.AsString(String.Format("Song{0}", index));

                if (String.IsNullOrWhiteSpace(path))
                {
                    break;
                }

                playlist.Add(ContentLoader.Current.Load<Song>(path));
            }

            OnGenerated(typeof(SongPlaylist), playlist);
        }

        protected override Boolean Initialize(InitializeParams initParams)
        {
            String directory = initParams.Directory;
            
            _directory = directory;

            return base.Initialize(initParams);
        }
    }
}

