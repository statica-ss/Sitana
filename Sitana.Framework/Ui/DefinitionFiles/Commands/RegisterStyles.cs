using Sitana.Framework.Content;
using Sitana.Framework.Ui.Controllers;
using Sitana.Framework.Ui.Core;
using Sitana.Framework.Xml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sitana.Framework.Ui.DefinitionFiles.Commands
{
    public class RegisterStyles: IDefinitionClass
    {
        public static void Parse(XNode node, DefinitionFile file)
        {
            StylesContainer.Parse(node, file);

            bool overwrite = false;
            bool.TryParse(node.Attribute("OverwriteExisting"), out overwrite);

            StylesManager.Instance.RegisterStyles(file, overwrite);
        }

        bool IDefinitionClass.Init(UiController controller, object binding, DefinitionFile file)
        {
            return false;
        }
    }
}
