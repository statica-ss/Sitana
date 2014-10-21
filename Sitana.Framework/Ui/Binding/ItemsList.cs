using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sitana.Framework.Ui.Binding
{
    public class ItemsList<T> : IItemsProvider
    {
        List<T> _elements = new List<T>();

        List<IItemsConsumer> _consumers = new List<IItemsConsumer>();

        public void Subscribe(IItemsConsumer consumer)
        {
            lock (this)
            {
                _consumers.Add(consumer);
            }
        }

        public void Unsubscribe(IItemsConsumer consumer)
        {
            lock (this)
            {
                _consumers.Remove(consumer);
            }
        }

        public void Add(T element)
        {
            lock (this)
            {
                _elements.Add(element);

                for (int idx = 0; idx < _consumers.Count; ++idx)
                {
                    _consumers[idx].Added(element, _elements.Count-1);
                }
            }
        }

        public void Insert(int index, T element)
        {
            lock (this)
            {
                _elements.Insert(index, element);

                for (int idx = 0; idx < _consumers.Count; ++idx)
                {
                    _consumers[idx].Added(element, index);
                }
            }
        }

        public void Remove(T element)
        {
            lock (this)
            {
                _elements.Remove(element);

                for (int idx = 0; idx < _consumers.Count; ++idx)
                {
                    _consumers[idx].Removed(element);
                }
            }
        }

        public void Clear()
        {
            lock (this)
            {
                for (int idx = 0; idx < _consumers.Count; ++idx)
                {
                    for (int el = 0; el < _elements.Count; ++idx)
                    {
                        var element = _elements[el];
                        _consumers[idx].Removed(element);
                    }
                }
            }
        }

        public void RemoveAt(int index)
        {
            lock (this)
            {
                T element = _elements[index];
                _elements.RemoveAt(index);

                for (int idx = 0; idx < _consumers.Count; ++idx)
                {
                    _consumers[idx].Removed(element);
                }
            }
        }

        public int Count
        {
            get
            {
                lock (this)
                {
                    return _elements.Count;
                }
            }
        }

        public T this[int index]
        {
            get
            {
                return _elements[index];
            }
        }

        object IItemsProvider.ElementAt(int index)
        {
            return _elements[index];
        }
    }
}
