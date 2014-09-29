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
        public enum Mode
        { 
            Begining,
            Center,
            End
        }

        public readonly static Length Default = new Length(true);

        int _length;
        bool _percent;

        bool _auto;
        Mode _mode;
        

        public bool IsAuto
        {
            get
            {
                return _auto;
            }
        }

        public Length(bool auto)
        {
            _mode = Mode.Begining;
            _auto = auto;
            _length = 0;
            _percent = false;
        }

        public Length(int length, bool percent, Mode mode)
        {
            _mode = mode;
            _auto = false;
            _length = length;
            _percent = percent;
        }

        public Length(int length)
        {
            _auto = false;
            _length = length;
            _percent = false;
            _mode = Mode.Begining;
        }

        public int Compute(int size)
        {
            int add = 0;

            switch (_mode)
            {
                case Mode.Center:
                    add = size / 2;
                    break;

                case Mode.End:
                    add = size;
                    break;
            }

            if ( _percent )
            {
                return add + _length * size / 100;
            }

            return add + _length;
        }
    }
}
