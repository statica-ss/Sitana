using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ebatianos.Cs
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
