// SITANA - Copyright (C) The Sitana Team.
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using Sitana.Framework.Ui.DefinitionFiles;

namespace Sitana.Framework.Ui.Views
{
    public class UiBorder : UiContainer
    {
        public new static void Parse(XNode node, DefinitionFile file)
        {
            UiContainer.Parse(node, file);

            foreach(var cn in node.Nodes)
            {
                switch ( cn.Tag )
                {
                    case "UiBorder.Children":
                        ParseChildren(cn, file);
                        break;
                }
            }
        }
    }
}
