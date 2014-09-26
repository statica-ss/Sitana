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

using System;
using Microsoft.Xna.Framework;

namespace Sitana.Framework.Input
{
    /// <summary>
    /// Struct holding information about pointer.
    /// </summary>
    public struct PointerInfo
    {
        // Value representing id of invalid pointer.
        public const int InvalidPointerId = 0;

        /// <summary>
        /// States for pointers.
        /// </summary>
        public enum PressState
        {
            Invalid,
            Pressed,
            Moved,
            Released
        }


        // Id of pointer.
        public Int32 PointerId { get; internal set; }


        // Pointer position.
        public Vector2 Position { get; internal set; }


        // State of pointer.
        public PressState State { get; internal set; }

        /// <summary>
        /// Constructs new pointer info object.
        /// </summary>
        /// <param name="pointerId">Id of pointer.</param>
        /// <param name="position">Position.</param>
        /// <param name="state">Pointer state.</param>
        public PointerInfo(Int32 pointerId, Vector2 position, PressState state)
            : this()
        {
            PointerId = pointerId;
            Position = position;
            State = state;
        }
    }
}
