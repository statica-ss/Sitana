// SITANA - Copyright (C) The Sitana Team.
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

namespace Sitana.Framework.Gui
{
    public enum TransitionType : int
    {
        None = 0,   // No fading at all
        Left = 1,   // Fade from/to left
        Right = 2,  // Fade from/to right
        Top = 3,    // Fade from/to top
        Bottom = 4, // Fade from/to bottom
        AlphaBlend = 5,    // Alpha blending when fading
        Fade = 6,
        Always = 7,
        SlideLeft = 8,   // Slide from/to left
        SlideRight = 9,  // Slide from/to right
        SlideTop = 10,    // Slide from/to top
        SlideBottom = 11  // Slide from/to bottom
    }
}