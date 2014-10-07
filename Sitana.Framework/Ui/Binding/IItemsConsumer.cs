using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sitana.Framework.Ui.Binding
{
    public interface IItemsConsumer
    {
        void Added(object item, int index);
        void Removed(object item);

        void Recalculate();
    }
}
