/// This file is a part of the EBATIANOS.ESSENTIALS class library.
/// (C)2013-2014 Sebastian Sejud. All rights reserved.
///
/// THIS SOURCE FILE IS THE PROPERTY OF SEBASTIAN SEJUD AND IS NOT TO BE 
/// RE-DISTRIBUTED BY ANY MEANS WHATSOEVER WITHOUT THE EXPRESSED WRITTEN 
/// CONSENT OF SEBASTIAN SEJUD.
/// 
/// THIS SOURCE CODE CAN ONLY BE USED UNDER THE TERMS AND CONDITIONS OUTLINED
/// IN THE EBATIANOS.ESSENTIALS LICENSE AGREEMENT. SEBASTIAN SEJUD GRANTS
/// TO YOU (ONE SOFTWARE DEVELOPER) THE LIMITED RIGHT TO USE THIS SOFTWARE 
/// ON A SINGLE COMPUTER.
///
/// CONTACT INFORMATION:
/// essentials@sejud.com
/// sejud.com/essentials-library
/// 
///---------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
