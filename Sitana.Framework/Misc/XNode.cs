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

using Ebatianos.Content;
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace Ebatianos
{
    public class XNode
    {
        public readonly string Tag;
        public readonly string Value;
        public readonly string Namespace;
        public readonly int LineNumber;
        public readonly XFile Owner;

        public List<XNode> Nodes { get; private set; }

        Dictionary<string, string> _attributes = new Dictionary<string,string>();

        XNode _parent = null;

        private XNode(XNode parent, XFile file, int lineNumber)
        {
            _parent = parent;
            Owner = file;
            LineNumber = lineNumber;
            Nodes = new List<XNode>();
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

                        _attributes.Add(key, reader.Value);
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
                Value = reader.Value;
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
                for (Int32 idx = 0; idx < Nodes.Count; ++idx)
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

            return value ?? String.Empty;
        }

        public bool HasAttribute(string name)
        {
            return _attributes.ContainsKey(name);
        }

        public string NodeInfo
        {
            get
            {
                return String.Format("{0}({1})", Owner.Name, LineNumber);
            }
        }

        public string NodeError(string message)
        {
            return String.Format("{0}: error: {1}", NodeInfo, message);
        }

        public IEnumerable<string> Attributes
        {
            get
            {
                return _attributes.Keys;
            }
        }
    }
}
