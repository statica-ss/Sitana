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

using Microsoft.Xna.Framework.Input.Touch;
using System;

namespace Ebatianos.Input
{
	internal enum GestureReceiverType
	{
		Menu,
		Screen,
		Popup
	}

    internal class RegisteredGesture
    {
        public GestureAdditionalType AdditionalType {get; private set;}
        public GestureType Type { get; private set; }
        public EventHandler<GestureEventArgs> Handler {get; private set;}
        public Boolean Enabled { get; set; }

		public GestureReceiverType ReceiverType { get; private set; }

		public RegisteredGesture(GestureAdditionalType additionalType, GestureType type, GestureReceiverType receiverType, EventHandler<GestureEventArgs> handler)
        {
            AdditionalType = additionalType;
            Type = type;
            Handler = handler;
            Enabled = true;
			ReceiverType = receiverType;
        }
    }
}
