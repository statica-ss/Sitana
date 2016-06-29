using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sitana.Framework.Content;
using System.IO;
using Microsoft.Xna.Framework.Graphics;

namespace Sitana.Framework.Graphics
{
    class FontLoader : ContentLoader.AdditionalType
    {
        /// <summary>
        /// Registers additional type in ContentLoader
        /// </summary>
        public static void Register()
        {
            RegisterType(typeof(Font), Load, true);
        }

        public static object Load(string name)
        {
            Font font = new Font();

            using (Stream stream = ContentLoader.Current.Open(name + ".sft"))
            {
                using (BinaryReader reader = new BinaryReader(stream))
                {
                    font.Load(reader);
                }
            }

            string sheetPath = name;

            if (!font.FontSheetPath.IsNullOrWhiteSpace())
            {
                string directory = Path.GetDirectoryName(name);
                sheetPath = Path.Combine(directory, font.FontSheetPath);
            }

            font.FontSheet = ContentLoader.Current.Load<Texture2D>(sheetPath);

            return font;
        }
    }
}
