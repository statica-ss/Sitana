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

using System;
using System.Collections.Generic;
using System.Xml;

namespace Ebatianos
{
    public class XmlFileNode
    {
        public String Tag { get; private set; }
        public String Value { get; private set; }
        public List<XmlFileNode> Nodes { get; private set; }
        public ParametersCollection Attributes { get; private set; }

        private XmlFileNode(XmlReader reader)
        {
            while (reader.NodeType != XmlNodeType.Element)
            {
                if ( reader.EOF || reader.NodeType == XmlNodeType.EndElement)
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

            Boolean hasChildren = !reader.IsEmptyElement;

            Attributes = new ParametersCollection(false);

            if (reader.HasAttributes)
            {
                // Read all attributes and add them to dictionary.
                while (reader.MoveToNextAttribute())
                {
                    Attributes.Add(reader.Name, reader.Value);
                }
            }
            else
            {
                reader.Read();
            }


            Nodes = new List<XmlFileNode>();

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
                    XmlFileNode node = new XmlFileNode(reader);

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

        public static XmlFileNode ReadXml(XmlReader reader)
        {
            return new XmlFileNode(reader);
        }

        public Object ValueSource
        {
            set
            {
                Attributes.ValueSource = value;

                for ( Int32 idx = 0; idx < Nodes.Count; ++idx )
                {
                    Nodes[idx].ValueSource = value;
                }
            }
        }

        public ColorsManager ColorsManager
        {
            set
            {
                Attributes.ColorsManager = value;

                for (Int32 idx = 0; idx < Nodes.Count; ++idx)
                {
                    Nodes[idx].ColorsManager = value;
                }
            }
        }

        public XmlFileNode this[String nodeName]
        {
            get
            {
                for ( Int32 idx = 0; idx < Nodes.Count; ++idx )
                {
                    if ( Nodes[idx].Tag == nodeName )
                    {
                        return Nodes[idx];
                    }
                }

                throw new Exception("Cannot find xml node: " + nodeName);
            }
        }
    }
}
