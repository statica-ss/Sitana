using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sitana.Framework.Ui.RichText
{
    public interface IRichProcessor
    {
        void Process(string text);
        List<Line> Lines { get; }
    }
}
