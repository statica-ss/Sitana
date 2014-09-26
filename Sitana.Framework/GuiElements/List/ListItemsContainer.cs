/// This file is a part of the EBATIANOS.ESSENTIALS class library.
/// (c)2013-2014 EBATIANO'S a.k.a. Sebastian Sejud. All rights reserved.
///
/// THIS SOURCE FILE IS THE PROPERTY OF EBATIANO'S A.K.A. SEBASTIAN SEJUD 
/// AND IS NOT TO BE RE-DISTRIBUTED BY ANY MEANS WHATSOEVER WITHOUT 
/// THE EXPRESSED WRITTEN CONSENT OF EBATIANO'S A.K.A. SEBASTIAN SEJUD.
///
/// THIS SOURCE CODE CAN ONLY BE USED UNDER THE TERMS AND CONDITIONS OUTLINED
/// IN THE EBATIANOS.ESSENTIALS LICENSE AGREEMENT. 
/// EBATIANO'S A.K.A. SEBASTIAN SEJUD GRANTS TO YOU (ONE SOFTWARE DEVELOPER) 
/// THE LIMITED RIGHT TO USE THIS SOFTWARE ON A SINGLE COMPUTER.
///
/// CONTACT INFORMATION:
/// contact@ebatianos.com
/// www.ebatianos.com/essentials-library
/// 
///---------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ebatianos.Gui.List
{
    public class ListItemsContainer
    {
        private List<ListItemData> _list = new List<ListItemData>();
        internal readonly Object Lock = new Object();

        private List<ListItemData> _removedElements = new List<ListItemData>();

        private Boolean _update = false;
        private Boolean _lockRedraw = false;

        internal List<ListItemData> RemovedElements
        {
            get
            {
                return _removedElements;
            }
        }

        public Boolean LockRedraw
        {
            set
            {
                lock (Lock)
                {
                    _lockRedraw = value;
                }
            }
        }

        public Boolean ShouldUpdate
        {
            get
            {
                lock (Lock)
                {
                    if (_lockRedraw)
                    {
                        return false;
                    }

                    Boolean update = _update;
                    _update = false;
                    return update;
                }
            }

            internal set
            {
                lock (Lock)
                {
                    _update = value;
                }
            }
        }

        public Int32 Count
        {
            get
            {
                lock (Lock)
                {
                    return _list.Count;
                }
            }
        }

        public ListItemData this[Int32 index]
        {
            get
            {
                return _list[index];
            }
        }

        public void Add(ListItemData data)
        {
            lock (Lock)
            {
                _list.Add(data);
                _removedElements.Remove(data);
                _update = true;
            }

            data.OnAdded();
        }

        public void Insert(Int32 index, ListItemData data)
        {
            lock (Lock)
            {
                _list.Insert(index, data);
                _removedElements.Remove(data);
                _update = true;
            }

            data.OnAdded();
        }

        public void RemoveAt(Int32 index)
        {
            ListItemData data;

            lock (Lock)
            {
                data = _list[index];
                _removedElements.Add(data);
                _list.RemoveAt(index);
                _update = true;
            }

            data.OnRemoved();
        }

        public void Remove(ListItemData data)
        {
            lock (Lock)
            {
                _list.Remove(data);
                _removedElements.Add(data);
                _update = true;
            }

            data.OnRemoved();
        }

        public void Sort( Comparison<ListItemData> comparer )
        {
            lock (Lock)
            {
                _list.Sort( comparer );
                _update = true;
            }
        }

        public void Clear()
        {
            List<ListItemData> elements = new List<ListItemData>();

            lock (Lock)
            {
                for (Int32 idx = 0; idx < _list.Count; ++idx)
                {
                    var data = _list[idx];
                    elements.Add(data);
                    _removedElements.Add(data);
                }

                _list.Clear();
                _update = true;
            }

            foreach (var element in elements)
            {
                element.OnRemoved();
            }
        }

        public Int32 IndexOf(ListItemData data)
        {
            lock (Lock)
            {
                return _list.IndexOf(data);
            }
        }

        public T Find<T>(Predicate<ListItemData> match) where T: ListItemData
        {
            lock (Lock)
            {
                return _list.Find(match) as T;
            }
        }

        public Boolean Contains(ListItemData data)
        {
            lock (Lock)
            {
                return _list.Contains(data);
            }
        }
    }
}
