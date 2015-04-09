// SITANA - Copyright (C) The Sitana Team.
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

namespace Sitana.Framework.Cs
{
    public struct Pair<Type1, Type2>
    {
        public Type1 First { get; private set; }
        public Type2 Second { get; private set; }

        public Pair(Type1 o1, Type2 o2) : this()
        {
            First = o1;
            Second = o2;
        }
    }

    public struct Trio<Type1, Type2, Type3>
    {
        public Type1 First { get; private set; }
        public Type2 Second { get; private set; }
        public Type3 Third { get; private set; }

        public Trio(Type1 o1, Type2 o2, Type3 o3) : this()
        {
            First = o1;
            Second = o2;
            Third = o3;
        }
    }
}
