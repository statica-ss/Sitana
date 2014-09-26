// SITANA - Copyright (C) The Sitana Team.
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;

namespace Sitana.Framework
{
    public class OrientationChangedEventArgs: EventArgs
    {
        public Orientation Orientation {get; private set;}

        public OrientationChangedEventArgs(Orientation orientation)
        {
            Orientation = orientation;
        }
    }
}

