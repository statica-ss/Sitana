// SITANA - Copyright (C) The Sitana Team.
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System.IO;
using System.Xml;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Sitana.Framework;
using System;

namespace Sitana.Framework.Content
{
    /// <summary>
    /// Sprite information container.
    /// It contains sprite sheet texture
    /// and animation sequences
    /// </summary>
    public class Sprite : ContentLoader.AdditionalType
    {
        /// <summary>
        /// Class defining animation sequence
        /// </summary>
        public class Sequence
        {
            public Sequence(Int32 start, Int32 length, Boolean loop, Double speed, String name)
            {
                Start = start;
                Length = length;
                Loop = loop;
                Speed = speed;
                Name = name;
            }

            public Int32 Start { get; private set; }     // First frame of the sequence
            public Int32 Length { get; private set; }    // Number of frames in sequence
            public Boolean Loop { get; private set; }    // Should loop the animation
            public Double Speed { get; private set; }    // Speed (fps) of sequence
            public String Name { get; private set; }     // Name of sequence
        }

        // Array of sequences sprite contains
        public Sequence[] Sequences { get; private set; }

        // Size of one frame in sprite sheet
        public Point FrameSize { get; private set; }

        // Texture with sprite sheet
        public Texture2D[] SpriteSheet { get; private set; }

        public List<Point[]> OptimizedFrames { get; set; }

        // Number of columns in sprite sheet
        public Int32 Columns { get; private set; }

        /// <summary>
        /// Registers type in ContentLoader
        /// </summary>
        public static void Register()
        {
            RegisterType(typeof(Sprite), Load, true);
        }

        internal static Sprite FromData(Point frameSize, List<Point[]> optimizedFrames, Sequence[] sequences, Texture2D[] sheet)
        {
            return new Sprite()
            {
                FrameSize = frameSize,
                OptimizedFrames = optimizedFrames,
                Sequences = sequences,
                SpriteSheet = sheet
            };
        }

        /// <summary>
        /// Loads content object
        /// </summary>
        /// <param name="name">name of resource</param>
        /// <param name="contentLoader">content loader to load additional resources and files</param>
        /// <returns></returns>
        public static Object Load(String name)
        {
            // Open sprite definition file
            Stream stream = null;

            try
            {
                stream = ContentLoader.Current.Open(name + ".xml");
            }
            catch (Exception)
            {
                return LoadSprite(name);
            }

            using (stream)
            {
                // Create xmlReader.
                XmlReader reader = XmlReader.Create(stream);

                // Get sprite file directory name
                String directory = Path.GetDirectoryName(name);

                // Load sprite
                return LoadSprite(reader, directory);
            }
        }

        private static Object LoadSprite(String name)
        {
            String texturePath = name;

            // List of animation sequences
            List<Sequence> sequences = new List<Sequence>();
            sequences.Add(new Sequence(0, 1, false, 0, ""));

            // Create list of textures.
            List<Texture2D> spriteSheet = new List<Texture2D>();
            List<Rectangle> textureArea = new List<Rectangle>();

            // Exception to throw if no texture is loaded.
            Exception fileNotFoundException = null;

            // Try load texture using texture name.
            try
            {
                spriteSheet.Add(ContentLoader.Current.Load<Texture2D>(texturePath));
                textureArea.Add(new Rectangle(0, 0, spriteSheet[spriteSheet.Count - 1].Width, spriteSheet[spriteSheet.Count - 1].Height));
            }
            catch (System.Exception fileNotFound)
            {
                // Get exception to throw later if no textures are loaded.
                fileNotFoundException = fileNotFound;

                // If no texture found, try load textures' sequence.
                for (Int32 idx = 1; true; ++idx)
                {
                    try
                    {
                        // Load sprite sheet texture
                        spriteSheet.Add(ContentLoader.Current.Load<Texture2D>(texturePath + String.Format(".{0}", idx)));
                        textureArea.Add(new Rectangle(0, 0, spriteSheet[spriteSheet.Count - 1].Width, spriteSheet[spriteSheet.Count - 1].Height));

                        // Check if textures match each other.
                        if (spriteSheet[spriteSheet.Count - 1].Width != spriteSheet[0].Width || spriteSheet[spriteSheet.Count - 1].Height != spriteSheet[0].Height)
                        {
                            fileNotFoundException = new Exception("Multiple texture sprite must contain textures of same size.");
                            spriteSheet.Clear();
                            break;
                        }
                    }
                    catch (System.Exception)
                    {
                        break;
                    }
                }
            }

            // If no texture is loaded, throw file not found exception.
            if (spriteSheet.Count == 0)
            {
                throw fileNotFoundException;
            }

            // Compute size of one frame
            Point frameSize = new Point(spriteSheet[0].Width, spriteSheet[0].Height);

            // Create new Sprite object with loaded data
            return new Sprite()
            {
                SpriteSheet = spriteSheet.ToArray(),
                FrameSize = frameSize,
                Columns = 1,
                Sequences = sequences.ToArray()
            };
        }

        /// <summary>
        /// Loads sprite from xml file
        /// </summary>
        /// <param name="xmlReader">xml reader object with already loaded file</param>
        /// <param name="contentLoader">content loader for loading additional stuff</param>
        /// <returns></returns>
        private static Object LoadSprite(XmlReader xmlReader, String directory)
        {
            // Path for sprite sheet texture
            String texturePath = "";

            // Number of frames horizontal and vertical
            Point frames = new Point();

            // List of animation sequences
            List<Sequence> sequences = new List<Sequence>();

            // Read main element.
            xmlReader.ReadStartElement("SpriteSheet");

            try
            {
                // Find Texture tag and read texture path
                xmlReader.ReadToFollowing("Texture");
                texturePath = xmlReader.GetAttribute("Path");

                // Find frames definition tag and read number of frames
                xmlReader.ReadToFollowing("Frames");
                frames.X = Int32.Parse(xmlReader.GetAttribute("X"));
                frames.Y = Int32.Parse(xmlReader.GetAttribute("Y"));

                // Go to tag with animation sequences
                xmlReader.ReadToFollowing("Sequences");

                // Into the tag...
                xmlReader.ReadStartElement();

                // Read the sequences
                while (xmlReader.ReadToFollowing("Sequence"))
                {
                    String name = xmlReader.GetAttribute("Name");
                    Int32 start = Int32.Parse(xmlReader.GetAttribute("Start"));
                    Int32 length = Int32.Parse(xmlReader.GetAttribute("Length"));
                    Boolean loop = (xmlReader.GetAttribute("Loop") == "Yes");
                    Double speed = 1;

                    // Try to read optional Speed attribute for the sequence
                    try
                    {
                        speed = Double.Parse(xmlReader.GetAttribute("Speed"));
                    }
                    catch (Exception)
                    {

                    }

                    // Add new sequence to list
                    sequences.Add(
                       new Sequence(start, length, loop, speed, name)
                       );
                }

                xmlReader.ReadEndElement();
            }
            catch (System.Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.ToString());
            }

            // Create list of textures.
            List<Texture2D> spriteSheet = new List<Texture2D>();
            List<Rectangle> textureArea = new List<Rectangle>();

            // Exception to throw if no texture is loaded.
            Exception fileNotFoundException = null;

            // Try load texture using texture name.
            try
            {
                spriteSheet.Add(ContentLoader.Current.Load<Texture2D>(texturePath));
                textureArea.Add(new Rectangle(0, 0, spriteSheet[spriteSheet.Count - 1].Width, spriteSheet[spriteSheet.Count - 1].Height));
            }
            catch (System.Exception fileNotFound)
            {
                // Get exception to throw later if no textures are loaded.
                fileNotFoundException = fileNotFound;

                // If no texture found, try load textures' sequence.
                for (Int32 idx = 1; true; ++idx)
                {
                    try
                    {
                        // Load sprite sheet texture
                        spriteSheet.Add(ContentLoader.Current.Load<Texture2D>(texturePath + String.Format(".{0}", idx)));
                        textureArea.Add(new Rectangle(0, 0, spriteSheet[spriteSheet.Count - 1].Width, spriteSheet[spriteSheet.Count - 1].Height));

                        // Check if textures match each other.
                        if (spriteSheet[spriteSheet.Count - 1].Width != spriteSheet[0].Width || spriteSheet[spriteSheet.Count - 1].Height != spriteSheet[0].Height)
                        {
                            fileNotFoundException = new Exception("Multiple texture sprite must contain textures of same size.");
                            spriteSheet.Clear();
                            break;
                        }
                    }
                    catch (System.Exception)
                    {
                        break;
                    }
                }
            }

            // If no texture is loaded, throw file not found exception.
            if (spriteSheet.Count == 0)
            {
                throw fileNotFoundException;
            }

            // Compute size of one frame
            Point frameSize = new Point(spriteSheet[0].Width / frames.X, spriteSheet[0].Height / frames.Y);

            // Create new Sprite object with loaded data
            return new Sprite()
            {
                SpriteSheet = spriteSheet.ToArray(),
                FrameSize = frameSize,
                Columns = frames.X,
                Sequences = sequences.ToArray()
            };
        }
    }
}
