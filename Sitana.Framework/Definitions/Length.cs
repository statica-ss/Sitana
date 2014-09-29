// SITANA - Copyright (C) The Sitana Team.
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sitana.Framework
{
    public struct Length
    {
        public readonly static Length Default = new Length(true);

        int _length;
        bool _percent;

        bool _auto;
        bool _add100;

        public bool IsAuto
        {
            get
            {
                return _auto;
            }
        }

        public Length(bool auto)
        {
            _add100 = false;
            _auto = auto;
            _length = 0;
            _percent = false;
        }

        public Length(int length, bool percent, bool add100)
        {
            _add100 = add100;
            _auto = false;
            _length = length;
            _percent = percent;
        }

        public Length(int length)
        {
            _auto = false;
            _length = length;
            _percent = false;
            _add100 = false;
        }

        public int Compute(int size)
        {
            int add = _add100 ? size : 0;

            if ( _percent )
            {
                return add + _length * size / 100;
            }

            return add + _length;
        }
    }
}
