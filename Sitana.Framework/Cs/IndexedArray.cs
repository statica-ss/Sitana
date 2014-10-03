// SITANA - Copyright (C) The Sitana Team.
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sitana.Framework.Cs
{
    public class IndexedArray<T>: IIndexedContainer<T>
    {
        T[] _array;
        public IndexedArray(T[] array)
        {
            _array = array;
        }

        public T this[Int32 index]
        {
            get
            {
                return _array[index];
            }
        }

        public Int32 Count
        {
            get
            {
                return _array.Length;
            }
        }
    }
}
