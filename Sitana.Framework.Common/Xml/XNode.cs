// SITANA - Copyright (C) The Sitana Team.
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.using Sitana.Framework.Content;

using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace Sitana.Framework.Xml
{
    public class XNode
    {
        const byte ContentTypeString = 0;
        const byte ContentTypeByteArray = 1;

        public readonly string Tag;
        public string Value;
        public byte[] BinaryValue;
        public readonly string Namespace;
        public readonly int LineNumber;
        public readonly XFile Owner;

        public List<XNode> Nodes { get; private set; }

        Dictionary<string, string> _attributes = new Dictionary<string,string>();

        XNode _parent = null;

        internal XNode(XFile owner, string tag)
        {
            _parent = null;
            Owner = owner;
            Tag = tag;
            Nodes = new List<XNode>();
        }

        public XNode(XNode parent, string tag)
        {
            _parent = parent;
            Owner = parent.Owner;
            Tag = tag;
            Nodes = new List<XNode>();
        }

        private XNode(XNode parent, XFile file, int lineNumber)
        {
            _parent = parent;
            Owner = file;
            LineNumber = lineNumber;
            Nodes = new List<XNode>();
        }

        internal XNode(XNode parent, XFile owner, BinaryReader reader)
        {
            _parent = parent;
            Owner = owner;

            Nodes = new List<XNode>();

            Tag = reader.ReadString();

            int attrCount = reader.ReadInt32();

            for (int idx = 0; idx < attrCount; ++idx)
            {
                string key = reader.ReadString();
                string value = reader.ReadString();

                _attributes.Add(key, value);
            }

            int nodesCount = reader.ReadInt32();

            for (int idx = 0; idx < nodesCount; ++idx)
            {
                var node = new XNode(this, owner, reader);
                Nodes.Add(node);
            }

            if (nodesCount == 0)
            {
                byte contentType = reader.ReadByte();

                if (contentType == ContentTypeString)
                {
                    Value = reader.ReadString();
                }
                else if(contentType == ContentTypeByteArray)
                {
                    int count = reader.ReadInt32();
                    BinaryValue = reader.ReadBytes(count);
                }
            }
        }

        private XNode(XmlReader reader, XFile owner, XNode parent, Dictionary<string, string> namespaces)
        {
            namespaces = new Dictionary<string, string>(namespaces);

            Owner = owner;
            _parent = parent;

            while (reader.NodeType != XmlNodeType.Element)
            {
                if (reader.EOF || reader.NodeType == XmlNodeType.EndElement)
                {
                    if (reader.NodeType == XmlNodeType.EndElement)
                    {
                        reader.Read();
                    }

                    Tag = null;
                    return;
                }
                reader.Read();
            }

            Tag = reader.LocalName;
            Namespace = reader.NamespaceURI;

            IXmlLineInfo xmlInfo = reader as IXmlLineInfo;
            if (xmlInfo != null)
            {
                LineNumber = xmlInfo.LineNumber;
            }
            else
            {
                LineNumber = 0;
            }

            Boolean hasChildren = !reader.IsEmptyElement;
            bool hasBinaryContent = false;
            if (reader.HasAttributes)
            {
                // Read all attributes and add them to dictionary.
                while (reader.MoveToNextAttribute())
                {
                    string key = reader.Name;

                    if (key.StartsWith("xmlns:"))
                    {
                        key = key.Substring(6);
                        namespaces.Add(key, reader.Value);
                    }
                    else if ( key != "xmlns")
                    {
                        if ( key.Contains(":"))
                        {
                            string[] vals = key.Split(':');

                            key = String.Format("{0}:{1}", namespaces[vals[0]], vals[1]);
                        }

                        if (key == "___XNodeExtensionAttribute_HasBinaryContent")
                        {
                            hasBinaryContent = reader.Value == "true";
                        }
                        else
                        {
                            _attributes.Add(key, reader.Value);
                        }
                    }
                }
            }
            else
            {
                reader.Read();
            }

            Nodes = new List<XNode>();

            while (reader.NodeType != XmlNodeType.Element && reader.NodeType != XmlNodeType.Text)
            {
                if (reader.EOF || reader.NodeType == XmlNodeType.EndElement)
                {
                    return;
                }
                reader.Read();
            }

            if (reader.NodeType == XmlNodeType.Text)
            {
                if (hasBinaryContent)
                {
                    BinaryValue = Convert.FromBase64String(reader.Value);
                }
                else
                {
                    Value = reader.Value;
                }
            }

            if (hasChildren)
            {
                while (true)
                {
                    XNode node = new XNode(reader, owner, this, namespaces);

                    if (node.Tag != null)
                    {
                        Nodes.Add(node);
                    }
                    else
                    {
                        break;
                    }

                    if (reader.EOF)
                    {
                        return;
                    }
                }
            }
        }

        public void AddAttribute(string name, string value)
        {
            _attributes.Add(name, value);
        }

        public XNode EnucleateAttributes(string attribPrefix)
        {
            var node = new XNode(_parent, Owner, LineNumber);
            int prefixLength = attribPrefix.Length;

            foreach(var attrib in _attributes)
            {
                if ( attrib.Key.StartsWith(attribPrefix))
                {
                    node._attributes.Add(attrib.Key.Substring(prefixLength), attrib.Value);
                }
            }

            return node;
        }

        internal static XNode ReadXml(XmlReader reader, XFile owner)
        {
            return new XNode(reader, owner, null, new Dictionary<string,string>());
        }

        public XNode this[String nodeName]
        {
            get
            {
                for (int idx = 0; idx < Nodes.Count; ++idx)
                {
                    if (Nodes[idx].Tag == nodeName)
                    {
                        return Nodes[idx];
                    }
                }

                throw new Exception("Cannot find xml node: " + nodeName);
            }
        }

        public string Attribute(string name)
        {
            string value = null;
            _attributes.TryGetValue(name, out value);

            return value ?? string.Empty;
        }

        public bool HasAttribute(string name)
        {
            return _attributes.ContainsKey(name);
        }

        public string NodeInfo
        {
            get
            {
                return string.Format("{0}({1})", Owner.Name, LineNumber);
            }
        }

        public string NodeError(string format, params object[] args)
        {
            string message = string.Format(format, args);
            return string.Format("{0}: error: {1}", NodeInfo, message);
        }

        public IEnumerable<string> Attributes
        {
            get
            {
                return _attributes.Keys;
            }
        }

        internal void Write(XmlWriter writer)
        {
            writer.WriteStartElement(Tag, Namespace);

            if (BinaryValue != null)
            {
                writer.WriteAttributeString("___XNodeExtensionAttribute_HasBinaryContent", "true");
            }

            foreach (var attr in _attributes)
            {
                writer.WriteAttributeString(attr.Key, attr.Value);
            }

            if (BinaryValue != null)
            {
                string text = Convert.ToBase64String(BinaryValue);
                writer.WriteString(text);
            }
            else if (!string.IsNullOrWhiteSpace(Value))
            {
                writer.WriteString(Value);
            }

            foreach(var node in Nodes)
            {
                node.Write(writer);
            }

            writer.WriteEndElement();
        }

        internal void Write(BinaryWriter writer)
        {
            if (Namespace.IsNullOrWhiteSpace())
            {
                writer.Write(Tag);
            }
            else
            {
                throw new NotImplementedException("Binary serialization of XNode doesn't support namespaces.");
            }

            writer.Write(_attributes.Count);

            foreach(var attr in _attributes)
            {
                writer.Write(attr.Key);
                writer.Write(attr.Value ?? string.Empty);
            }

            writer.Write(Nodes.Count);

            foreach(var node in Nodes)
            {
                node.Write(writer);
            }

            if (Nodes.Count == 0)
            {
                if (BinaryValue != null)
                {
                    writer.Write(ContentTypeByteArray);
                    writer.Write(BinaryValue.Length);
                    writer.Write(BinaryValue);
                }
                else
                {
                    writer.Write(ContentTypeString);
                    writer.Write(Value ?? string.Empty);
                }
            }
        }
    }
}
