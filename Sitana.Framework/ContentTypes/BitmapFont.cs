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
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Globalization;
using Sitana.Framework.Cs;

namespace Sitana.Framework.Content
{
    /// <summary>
    /// Font information container.
    /// It contains texture with characters
    /// and map of regions for each letter
    /// </summary>
    public class BitmapFont : ContentLoader.AdditionalType
    {
        /// <summary>
        /// Info for each character.
        /// Area - texture region with character.
        /// Spacing - distance between the begining of the character and begining of next character.
        /// </summary>
        public class CharacterInfo
        {
            public Rectangle Area;
            public Int32 Spacing;

            public Dictionary<Char, Single> Kerning = new Dictionary<Char, Single>();

            public Int32 CalculateSpacing(Char nextChar)
            {
                Single kerning = 0;

                if (Kerning.TryGetValue(nextChar, out kerning))
                {
                    return (Int32)Math.Ceiling(kerning * Area.Height) + (nextChar=='\0' ? 1 : 0);
                }

                return 0;
            }
        }

        // Texture with character.
        public Texture2D FontTexture { get; private set; }

        // Map of characters info
        public Dictionary<Char, CharacterInfo> CharactersMap { get; private set; }

        public Single Height { get; private set; }

        private static Byte[] _readBuffer = new Byte[1024*1024];

        public Single LineHeight
        {
            get
            {
                return BaseLine - CapLine;
            }
        }

        public Single BaseLine {get; private set;}
        public Single CapLine {get; private set;}

        /// <summary>
        /// Registers additional type in ContentLoader
        /// </summary>
        public static void Register()
        {
            RegisterType(typeof(BitmapFont), Load, true);
        }

        // <summary>
        /// Loads content object
        /// </summary>
        /// <param name="name">name of resource</param>
        /// <param name="contentLoader">content loader to load additional resources and files</param>
        /// <returns></returns>
        public static Object Load(String name)
        {
            return new BitmapFont(name);
        }

        /// <summary>
        /// Initializes new BitmapFont object from font files.
        /// </summary>
        /// <param name="path">Path of font file.</param>
        /// <param name="contentLoader">Content loader used to load assets.</param>
        private BitmapFont(String path)
        {
            // Create new map for characters.
            CharactersMap = new Dictionary<Char, CharacterInfo>();

            // Load texture with characters
            FontTexture = ContentLoader.Current.Load<Texture2D>(path);

            // Open font definition file and load info.
            using (Stream stream = ContentLoader.Current.Open(path + ".ff0"))
            {
                LoadInfo(stream);
            }
        }

        /// <summary>
        /// Initializes new BitmapFont object with only one character using given texture.
        /// </summary>
        /// <param name="texture">Texture with character.</param>
        /// <param name="character">Character which is represented by this texture.</param>
        public BitmapFont(Texture2D texture, Char character)
        {
            // Set texture.
            FontTexture = texture;

            // Create new map and add only one character to it with region of entire texture.
            CharactersMap = new Dictionary<Char, CharacterInfo>();

            CharactersMap.Add(character,
                  new CharacterInfo()
                  {
                      Area = new Rectangle(0, 0, texture.Width, texture.Height),
                      Spacing = 0
                  }
               );
        }

        #if WINDOWS || WINDOWSMG
        public BitmapFont(GraphicsDevice device, Stream textureStream, Stream infoStream)
        {
            // Create new map for characters.
            CharactersMap = new Dictionary<Char, CharacterInfo>();

            // Load texture.
            FontTexture = LoadTexture2DFromPng.FromStream(device, textureStream);

            // Load info.
            LoadInfo(infoStream);
        }
        #endif

        /// <summary>
        /// Loads characters info from font definition file.
        /// </summary>
        /// <param name="stream">Stream with opened font definition.</param>
        private void LoadInfo(Stream stream)
        {
            Int32 length = stream.Read(_readBuffer, 0, _readBuffer.Length); 

            using (MemoryStream memoryStream = new MemoryStream(_readBuffer, 0, length))
            {
                // Create binary reader to read data.
                BinaryReader reader = new BinaryReader(memoryStream);

                // Get number of characters.
                Int32 count = reader.ReadInt32();

                Height = 0;

                CharactersMap.Add('\0', new CharacterInfo() {
                    Area = Rectangle.Empty,
                    Spacing = 0
                });

                // Read each character data.
                for (Int32 idx = 0; idx < count; ++idx)
                {
                    CharacterInfo info = new CharacterInfo();

                    // Character id.
                    Char character = reader.ReadChar();

                    // Read texture region where character is on the texture.
                    info.Area.X = reader.ReadInt32();
                    info.Area.Y = reader.ReadInt32();
                    info.Area.Width = reader.ReadInt32();
                    info.Area.Height = reader.ReadInt32();

                    // Read spacing of character.
                    info.Spacing = reader.ReadInt32();

                    Height = Math.Max(Height, info.Area.Height);

                    // Add character to map.
                    CharactersMap.Add(character, info);
                }

                try
                {
                    CapLine = reader.ReadInt32();
                    BaseLine = reader.ReadInt32();

                    Int32 kerningCount = reader.ReadInt32();

                    for (Int32 idx = 0; idx < kerningCount; ++idx)
                    {
                        Char context = reader.ReadChar();
                        Int32 numberOfChars = reader.ReadInt32();

                        var info = CharactersMap[context];

                        for (Int32 charIdx = 0; charIdx < numberOfChars; ++charIdx)
                        {
                            Char nextChar = reader.ReadChar();
                            Single offset = reader.ReadSingle();

                            info.Kerning.Add(nextChar, offset);
                        }
                    }
                } catch(Exception)
                {
                    CapLine = 0;
                    BaseLine = Height;
                }

                CharactersMap['\0'].Area = new Rectangle(0, 0, 0, (Int32)Height);
            }
        }
    }
}
