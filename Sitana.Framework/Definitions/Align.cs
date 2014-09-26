// SITANA - Copyright (C) The Sitana Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
namespace Sitana.Framework
{
    [Flags]
    public enum Align : int
    {
        Left = 0,
        Top = Left,
        Center = 1,
        Middle = 2,
        Right = 4,
        Bottom = 8,
        Justify = 16,
        StretchHorz = 32,
        StretchVert = 64,
        CutHorz = 128,
        CutVert = 256,
        Horz = Left | Center | Right | Justify | StretchHorz | CutHorz,
        Vert = Top | Middle | Bottom | StretchVert | CutVert
    }
}