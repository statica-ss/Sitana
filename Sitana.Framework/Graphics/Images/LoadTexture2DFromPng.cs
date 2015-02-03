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
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Sitana.Framework.Content
{
    /// <summary>
    /// Loader for texture from png file instead of xnb.
    /// </summary>
    public class LoadTexture2DFromPng : ContentLoader.AdditionalType
    {
        /// <summary>
        /// Registers type in ContentLoader.
        /// </summary>
        public static void Register()
        {
            RegisterType(typeof(Texture2D), Load, true);
        }

        /// <summary>
        /// Loads content object.
        /// </summary>
        /// <param name="name">name of resource</param>
        /// <param name="contentLoader">content loader to load additional resources and files</param>
        /// <returns></returns>
        public static Object Load(string name)
        {
            try
            {
                return FromName(name);
            }
            catch (Exception)
            {
                // If file wasn't found, try load asset using native method.
                return ContentLoader.Current.LoadNative<Texture2D>(name);
            }
        }

        public static Texture2D FromName(string name)
        {
            IGraphicsDeviceService deviceService = ContentLoader.Current.GetService<IGraphicsDeviceService>();
            GraphicsDevice device = deviceService.GraphicsDevice;

            // Open png file.
            using (Stream stream = ContentLoader.Current.Open(name + ".png"))
            {
                // Load texture from png stream.
                Texture2D texture = Texture2D.FromStream(device, stream);

                // If texture wasn't loades throw an exception.
                if (texture == null)
                {
                    throw new TypeLoadException();
                }

                SpriteBatch spriteBatch = new SpriteBatch(device);
                spriteBatch.Begin();
                spriteBatch.Draw(texture, Vector2.Zero, Color.White);
                spriteBatch.End();

                // Return loaded texture.
                return texture;
            }
        }
    }
}
