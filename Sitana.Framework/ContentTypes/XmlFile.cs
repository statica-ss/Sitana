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

using Sitana.Framework;
using System;
using System.IO;
using System.Xml;

namespace Sitana.Framework.Content
{
    public class XmlFile : ContentLoader.AdditionalType
    {
        private XmlFileNode _node;

        /// <summary>
        /// Registers additional type in ContentLoader
        /// </summary>
        public static void Register()
        {
            RegisterType(typeof(XmlFile), Load, true);
        }

        // <summary>
        /// Loads content object
        /// </summary>
        /// <param name="name">name of resource</param>
        /// <param name="contentLoader">content loader to load additional resources and files</param>
        /// <returns></returns>
        public static Object Load(String name)
        {
            return new XmlFile(name);
        }

        /// <summary>
        /// Initializes new BitmapFont object from font files.
        /// </summary>
        /// <param name="path">Path of font file.</param>
        /// <param name="contentLoader">Content loader used to load assets.</param>
        private XmlFile(String path)
        {
            // Open font definition file and load info.
            using (Stream stream = ContentLoader.Current.Open(path + ".xml"))
            {
                XmlReader reader = XmlReader.Create(stream);

                _node = XmlFileNode.ReadXml(reader);
            }
        }

        public static implicit operator XmlFileNode(XmlFile file)
        {
            return file._node;
        }
    }
}
