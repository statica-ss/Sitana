using Sitana.Framework.Ui.DefinitionFiles;
using Sitana.Framework.Xml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sitana.Framework.Content;

namespace Sitana.Framework.Ui.Core
{
    public class StylesContainer
    {
        public static void Parse(XNode node, DefinitionFile file)
        {
            Dictionary<string, DefinitionFile> definitions = new Dictionary<string, DefinitionFile>();

            foreach(var cn in node.Nodes)
            {
                if (cn.Tag == "Include")
                {
                    string filename = cn.Attribute("Path");
                    StylesManager.Instance.LoadStyles(filename, true);
                }
                else
                {
                    if (cn.Tag != "Style")
                    {
                        throw new Exception("Invalid node. Expected: Style");
                    }

                    string name = cn.Attribute("Name");

                    string src = cn.Attribute("Source");

                    if (src.IsNullOrWhiteSpace())
                    {
                        if (cn.Nodes.Count != 1)
                        {
                            throw new Exception("Invalid number of child nodes. Style can have only one child node.");
                        }

                        definitions.Add(name, DefinitionFile.LoadFile(cn.Nodes[0]));
                    }
                    else
                    {
                        definitions.Add(name, definitions[src]);
                    }
                }
            }

            file["Styles"] = definitions;
        }

        private StylesContainer()
        {
        }
    }
}
