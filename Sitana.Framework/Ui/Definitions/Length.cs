﻿// SITANA - Copyright (C) The Sitana Team.
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sitana.Framework.Ui.Core;

namespace Sitana.Framework
{
    public struct Length
    {
        public readonly static Length Default = new Length(true);
        public readonly static Length Zero = new Length(0);

        int _length;
        int _percent;

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
            _percent = 0;
        }

        public Length(int length, int percent)
        {
            _auto = false;
            _length = length;
            _percent = percent;
        }

        public Length(int length)
        {
            _auto = false;
            _length = length;
            _percent = 0;
        }

        public int Compute()
        {
            return Compute(0);
        }

        public int Compute(int size)
        {
            return (int)(UiUnit.Unit * (_percent * size / 100 + _length));
        }
    }
}
