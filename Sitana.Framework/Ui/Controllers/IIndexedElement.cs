using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sitana.Framework.Cs;

namespace Sitana.Framework.Ui.Controllers
{
    public interface IIndexedElement
    {
        int SelectedIndex { get; set; }
        int Count { get; }
        void GetText(SharedString text, int index);
    }
}
