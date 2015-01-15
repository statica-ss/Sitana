using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sitana.Framework.Ui.RichText
{
    public interface IRichProcessor
    {
        void Process(List<Line> lines, string text);
    }
}
