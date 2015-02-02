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

        object _consumersLock = new object();

        public void Subscribe(IItemsConsumer consumer)
        {
            lock (_consumersLock)
            {
                _consumers.Add(consumer);
            }
        }

        public void Unsubscribe(IItemsConsumer consumer)
        {
            lock (_consumersLock)
            {
                _consumers.Remove(consumer);
            }
        }

        public void Add(T element)
        {
            int index = 0;
            lock(this)
            {
                _elements.Add(element);
                index = _elements.Count - 1;
            }

            lock(_consumersLock)
            {
                for (int idx = 0; idx < _consumers.Count; ++idx)
                {
                    _consumers[idx].Added(element, index);
                }
            }
        }

        public void Insert(int index, T element)
        {
            lock(this)
            {
                _elements.Insert(index, element);
            }

            lock(_consumersLock)
            {
                for (int idx = 0; idx < _consumers.Count; ++idx)
                {
                    _consumers[idx].Added(element, index);
                }
            }
        }

        public int IndexOf(T element)
        {
            lock (this)
            {
                return _elements.IndexOf(element);
            }
        }

        public void Sort(Comparison<T> comparer)
        {
            lock (this)
            {
                _elements.Sort(comparer);
            }

            lock (_consumersLock)
            {
                for (int idx = 0; idx < _consumers.Count; ++idx)
                {
                    _consumers[idx].Recalculate();
                }
            }
        }

        public void MoveElementToIndex(T element, int index)
        {
            lock (this)
            {
                _elements.Remove(element);
                _elements.Insert(index, element);
            }

            lock (_consumersLock)
            {
                for (int idx = 0; idx < _consumers.Count; ++idx)
                {
                    _consumers[idx].Recalculate();
                }
            }
        }

        public void Remove(T element)
        {
            lock(this)
            {
                _elements.Remove(element);
            }

            lock(_consumersLock)
            {
                for (int idx = 0; idx < _consumers.Count; ++idx)
                {
                    _consumers[idx].Removed(element);
                }
            }
        }

        public void Clear()
        {
            lock(_consumersLock)
            {
                for (int idx = 0; idx < _consumers.Count; ++idx)
                {
                    _consumers[idx].RemovedAll();
                }
            }

            lock(this)
            {
                _elements.Clear();
            }
        }

        public void RemoveAt(int index)
        {
            T element;

            lock(this)
            {
                element = _elements[index];
                _elements.RemoveAt(index);
            }

            lock(_consumersLock)
            {
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
