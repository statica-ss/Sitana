// SITANA - Copyright (C) The Sitana Team.
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
namespace Sitana.Framework
{
    [Flags]
    public enum TextAlign : int
    {
        None = 0,
        Left = None,
        Right = 1,
        Center = 2,
        //Justify = 4,
        Middle = 8,
        VCenter = Middle,
        Top = None,
        Bottom = 16,
        Horz = Left | Center | Right,// | Justify,
        Vert = Top | Middle | Bottom
    }
}