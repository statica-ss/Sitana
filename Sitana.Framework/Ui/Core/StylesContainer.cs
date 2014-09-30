using Sitana.Framework.Ui.DefinitionFiles;
using Sitana.Framework.Xml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sitana.Framework.Ui.Core
{
    public class StylesContainer
    {
        public static void Parse(XNode node, DefinitionFile file)
        {
            Dictionary<string, DefinitionFile> definitions = new Dictionary<string, DefinitionFile>();

            foreach(var cn in node.Nodes)
            {
                if ( cn.Tag != "Style" )
                {
                    throw new Exception("Invalid node. Expected: Style");
                }

                if (cn.Nodes.Count != 1 )
                {
                    throw new Exception("Invalid number of child nodes. Style can have only one child node.");
                }

                string name = cn.Attribute("Name");

                definitions.Add(name, DefinitionFile.LoadFile(cn.Nodes[0]));
            }

            file["Styles"] = definitions;
        }

        private StylesContainer()
        {
        }
    }
}
