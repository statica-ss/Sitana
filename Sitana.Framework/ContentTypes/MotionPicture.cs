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
using Sitana.Framework.Content.MotionPictureCore;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Globalization;

namespace Sitana.Framework.Content
{
    public class MotionPicture : ContentLoader.AdditionalType
    {
        /// <summary>
        /// Registers type in ContentLoader
        /// </summary>
        public static void Register()
        {
            RegisterType(typeof(MotionPicture), Load, true);
        }

        /// <summary>
        /// Loads content object
        /// </summary>
        /// <param name="name">name of resource</param>
        /// <param name="contentLoader">content loader to load additional resources and files</param>
        /// <returns></returns>
        public static Object Load(String name)
        {
            using (Stream stream = ContentLoader.Current.Open(name + ".eas"))
            {
                // Create xmlReader.
                StreamReader reader = new StreamReader(stream);
                String content = reader.ReadToEnd();
                String[] lines = content.Split('\n', '\r');

                return new MotionPicture(lines);
            }
        }

        private MotionPicturePlayback _playback;
        private List<MotionPictureObject> _objects = new List<MotionPictureObject>();
        public Single Length { get; private set; }
        private Single _time = 0;

        public MotionPicture(String[] lines)
        {
            _playback = new MotionPicturePlayback(this);
            _objects.Add(_playback);

            List<String> parameters = new List<String>();

            Single currentTime = 0;

            foreach (var line in lines)
            {
                if (line.StartsWith("#"))
                {
                    continue;
                }

                String[] parseComments = line.Split('#');

                if (parseComments[0].Length == 0)
                {
                    continue;
                }

                String[] elements = parseComments[0].Split('\t', ' ');

                parameters.Clear();

                foreach (var element in elements)
                {
                    if (!String.IsNullOrEmpty(element))
                    {
                        if (element.StartsWith("\""))
                        {
                            parameters.Add(element.Trim('"'));
                        }
                        else
                        {
                            parameters.Add(element);
                        }

                    }
                }

                if (parameters.Count == 1)
                {
                    currentTime = Single.Parse(parameters[0], CultureInfo.InvariantCulture);
                }
                else if (parameters.Count < 2)
                {
                    throw new Exception("Syntax error in motion picture script.");
                }
                else
                {
                    String[] ids = parameters[0].Split('+');

                    foreach (var id in ids)
                    {
                        MotionPictureObject obj = Find(id);

                        String[] arguments = new String[parameters.Count - 2];

                        for (Int32 idx = 2; idx < parameters.Count; ++idx)
                        {
                            arguments[idx - 2] = parameters[idx];
                        }

                        obj.AddAction(currentTime, parameters[1], arguments);
                    }
                }
            }

            Length = currentTime;
        }

        private MotionPictureObject Find(String name)
        {
            name = name.ToLowerInvariant();

            for (Int32 idx = 0; idx < _objects.Count; ++idx)
            {
                if (_objects[idx].Name == name)
                {
                    return _objects[idx];
                }
            }

            throw new Exception("Unknown object in motion picture script.");
        }

        public void AddObject(MotionPictureObject obj)
        {
            _objects.Add(obj);
        }

        public void InitPlayback()
        {
            foreach (var obj in _objects)
            {
                obj.Reset();
                obj.Process(0, 0);
            }
            _time = 0;
        }

        public Boolean Playback(Single time)
        {
            _time += time;
            for (Int32 idx = 0; idx < _objects.Count; ++idx)
            {
                _objects[idx].Process(_time, time);
            }

            return _time >= Length;
        }

        public Color BgColor
        {
            get
            {
                return _playback.BgColor;
            }
        }

        public void Draw(SpriteBatch spriteBatch, Rectangle target, Single transition)
        {
            _objects.Sort(
                  delegate(MotionPictureObject p1, MotionPictureObject p2)
                  {
                      return p2.SortOrder - p1.SortOrder;
                  }
               );

            Vector2 areaSize = new Vector2(target.Width, target.Height);

            Single scale = 1;
            Vector2 offset = new Vector2(target.X, target.Y);

            Single scaleX = areaSize.X / _playback.View.X;
            Single scaleY = areaSize.Y / _playback.View.Y;

            scale = Math.Min(scaleX, scaleY);

            Single sizeX = scale * _playback.View.X;
            Single sizeY = scale * _playback.View.Y;

            offset += new Vector2((areaSize.X - sizeX) / 2, (areaSize.Y - sizeY) / 2);

            spriteBatch.End();
            spriteBatch.Begin();

            Boolean additive = false;
            Boolean point = false;

            for (Int32 idx = 0; idx < _objects.Count; ++idx)
            {
                MotionPictureObject obj = _objects[idx];

                if (obj.Visible)
                {
                    if (obj.AdditiveBlendState != additive || obj.PointSamplerState != point)
                    {
                        additive = obj.AdditiveBlendState;
                        point = obj.PointSamplerState;

                        spriteBatch.End();
                        spriteBatch.Begin(SpriteSortMode.Deferred, additive ? BlendState.Additive : BlendState.AlphaBlend, point ? SamplerState.PointClamp : SamplerState.LinearClamp, DepthStencilState.None, RasterizerState.CullNone);
                    }

                    obj.Draw(spriteBatch, scale, offset, transition);
                }
            }

            spriteBatch.End();

            spriteBatch.Begin();
        }
    }
}
