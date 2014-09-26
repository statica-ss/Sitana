using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ebatianos.Gui
{
    public interface ISelectorContext
    {
        void GetData(Int32 offset, StringBuilder caption, StringBuilder annotation, out Boolean enabled);
        void SetCurrent(Int32 offset);

        Boolean ShouldUpdateSelection { get; }
    }
}
