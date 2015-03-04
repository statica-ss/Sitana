using System;
using System.Text;

namespace Sitana.Framework.Ui.Views
{
    public interface ISelectorContext
    {
        void GetData(int offset, StringBuilder caption, out bool enabled);
        void SetCurrent(int offset);

        bool ShouldUpdateSelection { get; }
    }
}
