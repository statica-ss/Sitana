using System;
using System.IO;
using System.Text;
using System.Xml;

namespace Sitana.Framework.Xml
{
    public class XFile
    {
        XNode _node;

        public readonly string Name;

        /// <summary>
        /// Initializes new BitmapFont object from font files.
        /// </summary>
        /// <param name="path">Path of font file.</param>
        /// <param name="contentLoader">Content loader used to load assets.</param>
        XFile(string name)
        {
            Name = name;
        }

        public static XFile Create(Stream stream, string name)
        {
            XmlReader reader = XmlReader.Create(stream);

            var xfile = new XFile(name);
            var node = XNode.ReadXml(reader, xfile);

            xfile._node = node;

            return xfile;
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

        public static XFile LoadBinary(Stream stream, string name)
        {
            var reader = new BinaryReader(stream);
            var xfile = new XFile(name);

            var node = new XNode(null, xfile, reader);
            xfile._node = node;

            return xfile;
        }

        public void Write(Stream stream)
        {
            XmlWriterSettings settings = new XmlWriterSettings();
            
            settings.Indent = true;
            settings.OmitXmlDeclaration = false;
            settings.NewLineOnAttributes = false;
            settings.Encoding = Encoding.UTF8;

            XmlWriter writer = XmlWriter.Create(stream, settings);

            if (_node.Tag.IsNullOrEmpty())
            {
                foreach (var node in _node.Nodes)
                {
                    node.Write(writer);
                }
            }
            else
            {
                _node.Write(writer);
            }

            writer.Flush();
        }

        public void WriteBinary(Stream stream)
        {
            var writer = new BinaryWriter(stream);

            if (_node.Tag.IsNullOrEmpty())
            {
                foreach (var node in _node.Nodes)
                {
                    node.Write(writer);
                }
            }
            else
            {
                _node.Write(writer);
            }

            writer.Flush();
        }
    }
}
