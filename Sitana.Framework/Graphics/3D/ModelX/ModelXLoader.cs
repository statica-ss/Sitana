// SITANA - Copyright (C) The Sitana Team.
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using Sitana.Framework.Graphics.Model;
using Sitana.Framework.Content;
using System.IO;
using Sitana.Framework.Graphics.Model.Importers;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Sitana.Framework.Graphics
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

