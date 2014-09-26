using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sitana.Framework.Ui.DefinitionFiles
{
    public interface IDefinitionClass
    {
        void Init(object context, DefinitionFile file);
    }
}
