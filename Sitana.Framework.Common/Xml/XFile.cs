using System.IO;
using System.Text;
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

        public static XFile Create(XmlReader reader, string name)
        {
            var xfile = new XFile(name);
            var node = XNode.ReadXml(reader, xfile);

            xfile._node = node;

            return xfile;
        }

        public static XFile Create(Stream stream, string name)
        {
            XmlReader reader = XmlReader.Create(stream);
            return Create(reader, name);
        }

        public static XFile Create(string name)
        {
            var xfile = new XFile(name);
            xfile._node = new XNode(xfile, string.Empty);

            return xfile;
        }

        public static implicit operator XNode(XFile file)
        {
            return file._node;
        }

        public void Write(Stream stream)
        {
            XmlWriterSettings settings = new XmlWriterSettings();
            
            settings.Indent = true;
            settings.OmitXmlDeclaration = false;
            settings.NewLineOnAttributes = false;
            settings.Encoding = Encoding.UTF8;

            XmlWriter writer = XmlWriter.Create(stream, settings);

            foreach (var node in _node.Nodes)
            {
                node.Write(writer);
            }

            writer.Flush();
        }
    }
}
