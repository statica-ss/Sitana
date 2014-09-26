using Sitana.Framework.Content;
using Sitana.Framework.Diagnostics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;

namespace Sitana.Framework.Content
{
    public class XFile : ContentLoader.AdditionalType
    {
        private XNode _node;

        public readonly string Name;

        /// <summary>
        /// Registers additional type in ContentLoader
        /// </summary>
        public static void Register()
        {
            RegisterType(typeof(XFile), Load, true);
        }

        // <summary>
        /// Loads content object
        /// </summary>
        /// <param name="name">name of resource</param>
        /// <param name="contentLoader">content loader to load additional resources and files</param>
        /// <returns></returns>
        public static Object Load(String name)
        {
            return new XFile(name);
        }

        /// <summary>
        /// Initializes new BitmapFont object from font files.
        /// </summary>
        /// <param name="path">Path of font file.</param>
        /// <param name="contentLoader">Content loader used to load assets.</param>
        private XFile(String path)
        {
            Name = ContentLoader.Current.AbsolutePath(path + ".xml"); 

            try
            {
                // Open font definition file and load info.
                using (Stream stream = ContentLoader.Current.Open(path + ".xml"))
                {
                    XmlReader reader = XmlReader.Create(stream);
                    _node = XNode.ReadXml(reader, this);
                }
            }
            catch(Exception ex)
            {
                throw new Exception(String.Format("{0}: error: The XML file has invalid elements.", Name), ex);
            }
        }

        public static implicit operator XNode(XFile file)
        {
            return file._node;
        }
    }
}
