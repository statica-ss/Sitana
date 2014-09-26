/// This file is a part of the EBATIANOS.ESSENTIALS class library.
/// (c)2013-2014 EBATIANO'S a.k.a. Sebastian Sejud. All rights reserved.
///
/// THIS SOURCE FILE IS THE PROPERTY OF EBATIANO'S A.K.A. SEBASTIAN SEJUD 
/// AND IS NOT TO BE RE-DISTRIBUTED BY ANY MEANS WHATSOEVER WITHOUT 
/// THE EXPRESSED WRITTEN CONSENT OF EBATIANO'S A.K.A. SEBASTIAN SEJUD.
///
/// THIS SOURCE CODE CAN ONLY BE USED UNDER THE TERMS AND CONDITIONS OUTLINED
/// IN THE EBATIANOS.ESSENTIALS LICENSE AGREEMENT. 
/// EBATIANO'S A.K.A. SEBASTIAN SEJUD GRANTS TO YOU (ONE SOFTWARE DEVELOPER) 
/// THE LIMITED RIGHT TO USE THIS SOFTWARE ON A SINGLE COMPUTER.
///
/// CONTACT INFORMATION:
/// contact@ebatianos.com
/// www.ebatianos.com/essentials-library
/// 
///---------------------------------------------------------------------------

namespace Ebatianos.Gui
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