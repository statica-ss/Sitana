using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sitana.Framework.Ui.Definitions
{
    public interface IMixable
    {
        IMixable Mix(IMixable lessImportant);
        bool IsMixMeaningful { get; }
    }
}
