// SITANA - Copyright (C) The Sitana Team.
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sitana.Framework.Cs
{
    public class SharedValue<T>
    {
        public delegate void ValueChangedDelegate(T newValue);

        public event ValueChangedDelegate ValueChanged;

        object _sharedLock = new object();

        T _value = default(T);

        public SharedValue(T value)
        {
            _value = value;
        }

        public SharedValue()
        {
        }

        public T Value
        {
            get
            {
                lock(_sharedLock)
                {
                    return _value;
                }
            }

            set
            {
                bool changed = false;

                lock(_sharedLock)
                {
                    changed = !object.Equals(_value, value);
                    _value = value;
                }

                if (changed && ValueChanged != null)
                {
                    ValueChanged(value);
                }
            }
        }
    }
}
