using Sitana.Framework.Content;
using Sitana.Framework.Diagnostics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;

namespace Sitana.Framework.Xml
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
        public static Object Load(string name)
        {
            return FromPath(name);
        }

        /// <summary>
        /// Initializes new BitmapFont object from font files.
        /// </summary>
        /// <param name="path">Path of font file.</param>
        /// <param name="contentLoader">Content loader used to load assets.</param>
        private XFile(string name)
        {
            Name = name;
        }

        public static XFile FromPath(string name)
        {
            name = name + ".xml";

            try
            {
                // Open font definition file and load info.
                using (Stream stream = ContentLoader.Current.Open(name))
                {
                    return FromStream(stream, name);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(String.Format("{0}: error: The XML file has invalid elements.", name), ex);
            }
        }

        public static XFile FromStream(Stream stream, string name)
        {
            XmlReader reader = XmlReader.Create(stream);

            var xfile = new XFile(name);
            var node = XNode.ReadXml(reader, xfile);

            xfile._node = node;

            return xfile;
        }

        public static implicit operator XNode(XFile file)
        {
            return file._node;
        }
    }
}
