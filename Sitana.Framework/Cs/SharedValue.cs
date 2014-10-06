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
        object _sharedLock = new object();

        T _value = default(T);

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
                lock(_sharedLock)
                {
                    _value = value;
                }
            }
        }
    }
}
