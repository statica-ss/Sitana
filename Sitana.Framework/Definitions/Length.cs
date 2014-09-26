using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sitana.Framework
{
    public struct Length
    {
        int _length;
        bool _percent;

        bool _auto;

        public bool IsAuto
        {
            get
            {
                return _auto;
            }
        }

        public Length(bool auto)
        {
            _auto = auto;
            _length = 0;
            _percent = false;
        }

        public Length(int length, bool percent)
        {
            _auto = false;
            _length = length;
            _percent = percent;
        }

        public Length(int length)
        {
            _auto = false;
            _length = length;
            _percent = false;
        }

        public int Compute(int size)
        {
            if ( _percent )
            {
                return _length * size / 100;
            }

            return _length;
        }
    }
}
