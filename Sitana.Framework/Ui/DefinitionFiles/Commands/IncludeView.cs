using System;
using Sitana.Framework.Ui.DefinitionFiles;
using Sitana.Framework.Xml;
using Sitana.Framework.Content;
using System.Collections.Generic;

namespace Sitana.Framework.Ui.DefinitionFiles.Commands
{
    public class IncludeView
    {
        public new static DefinitionFile Parse(XNode node, DefinitionFile file)
        {
            var path = node.Attribute("Path");

            XNode childNode = ContentLoader.Current.Load<XFile>(path);

            file = DefinitionFile.LoadFile(childNode);
            return file;
        }
    }
}

