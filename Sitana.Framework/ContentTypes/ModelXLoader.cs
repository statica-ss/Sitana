// /// This file is a part of the EBATIANOS.ESSENTIALS class library.
// /// (c)2013-2014 EBATIANO'S a.k.a. Sebastian Sejud. All rights reserved.
// ///
// /// THIS SOURCE FILE IS THE PROPERTY OF EBATIANO'S A.K.A. SEBASTIAN SEJUD 
// /// AND IS NOT TO BE RE-DISTRIBUTED BY ANY MEANS WHATSOEVER WITHOUT 
// /// THE EXPRESSED WRITTEN CONSENT OF EBATIANO'S A.K.A. SEBASTIAN SEJUD.
// ///
// /// THIS SOURCE CODE CAN ONLY BE USED UNDER THE TERMS AND CONDITIONS OUTLINED
// /// IN THE EBATIANOS.ESSENTIALS LICENSE AGREEMENT. 
// /// EBATIANO'S A.K.A. SEBASTIAN SEJUD GRANTS TO YOU (ONE SOFTWARE DEVELOPER) 
// /// THE LIMITED RIGHT TO USE THIS SOFTWARE ON A SINGLE COMPUTER.
// ///
// /// CONTACT INFORMATION:
// /// contact@ebatianos.com
// /// www.ebatianos.com/essentials-library
// /// 
// ///---------------------------------------------------------------------------
//
using System;
using Sitana.Framework.Graphics.Model;
using Sitana.Framework.Content;
using System.IO;
using Sitana.Framework.Graphics.Model.Importers;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Sitana.Framework.Content
{
    public class ModelXLoader: ContentLoader.AdditionalType
    {
        /// <summary>
        /// Registers additional type in ContentLoader
        /// </summary>
        public static void Register()
        {
            RegisterType(typeof(ModelX), Load, true);
        }

        public static Object Load(String name)
        {
            String directory = Path.GetDirectoryName(name);

            EmxImporter importer = new EmxImporter();
            ModelX model = null;

            using( Stream stream = ContentLoader.Current.Open(name + ".emx") ) 
            {
                model = importer.Import(stream, (fn)=>
                    {
                        return ContentLoader.Current.Open(Path.Combine(directory,fn));
                    });
            }

            foreach(var set in model.MaterialsSets)
            {
                foreach(var material in set)
                {
                    if ( !String.IsNullOrWhiteSpace(material.Texture))
                    {
                        Texture2D texture = ContentLoader.Current.Load<Texture2D>(Path.Combine(directory, material.Texture));
                        material.Textures = new MaterialTextures(texture);
                    }
                }
            }

            IGraphicsDeviceService deviceService = ContentLoader.Current.GetService<IGraphicsDeviceService>();
            model.PrepareForRender(deviceService.GraphicsDevice);

            return model;
        }
    }
}

