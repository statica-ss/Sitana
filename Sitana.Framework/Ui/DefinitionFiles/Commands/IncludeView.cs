using System;
using Sitana.Framework.Ui.DefinitionFiles;
using Sitana.Framework.Xml;
using Sitana.Framework.Content;
using System.Collections.Generic;

namespace Sitana.Framework.Ui.DefinitionFiles.Commands
{
    public class IncludeView
    {
        public static DefinitionFile Parse(XNode node, DefinitionFile file)
        {
            var path = node.Attribute("Path");

            return ContentLoader.Current.Load<DefinitionFile>(path);
        }
    }
}

