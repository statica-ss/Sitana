using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sitana.Framework.Ui.Binding
{
    public interface IItemsProvider
    {
        int Count { get; }
        object ElementAt(int index);

        void Subscribe(IItemsConsumer consumer);
        void Unsubscribe(IItemsConsumer consumer);

        T Find<T>(Predicate<T> predicate);

        void ForEach<T>(Action<T> element);
    }
}
