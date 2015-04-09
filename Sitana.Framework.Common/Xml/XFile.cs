using System.IO;
using System.Xml;

namespace Sitana.Framework.Xml
{
    public class XFile
    {
        private XNode _node;

        public readonly string Name;

        /// <summary>
        /// Initializes new BitmapFont object from font files.
        /// </summary>
        /// <param name="path">Path of font file.</param>
        /// <param name="contentLoader">Content loader used to load assets.</param>
        private XFile(string name)
        {
            Name = name;
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
