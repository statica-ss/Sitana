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

                    if (cn.Nodes.Count > 1)
                    {
                        throw new Exception("Invalid number of child nodes. Style can have only one child node.");
                    }

                    DefinitionFile styleFile = null;

                    if (cn.Nodes.Count == 1)
                    {
                        styleFile = DefinitionFile.LoadFile(cn.Nodes[0]);
                    }

                    DefinitionFile otherStyle = null;

                    if (!src.IsNullOrWhiteSpace())
                    {
                        if (definitions.ContainsKey(src))
                        {
                            otherStyle = definitions[src];
                        }
                        else
                        {
                            otherStyle = StylesManager.Instance.FindStyle(src);
                        }
                    }

                    if(otherStyle != null)
                    {
                        if (styleFile == null)
                        {
                            styleFile = otherStyle;
                        }
                        else
                        {
                            foreach (var key in otherStyle.Keys)
                            {
                                if (!styleFile.HasKey(key))
                                {
                                    styleFile[key] = otherStyle[key];
                                }
                            }
                        }
                    }

                    definitions.Add(name, styleFile);
                }
            }

            file["Styles"] = definitions;
        }

        private StylesContainer()
        {
        }
    }
}
