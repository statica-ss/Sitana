// SITANA - Copyright (C) The Sitana Team.
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sitana.Framework.Cs
{
    public interface IIndexedContainer<T>
    {
        T this[Int32 index] { get; }
        Int32 Count { get; }
    }
}
