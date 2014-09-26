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
using Microsoft.Xna.Framework.Input.Touch;
using Sitana.Framework.Input;

namespace Sitana.Framework.Gui
{
    public class ScreenPopup : Screen
    {
        public Boolean ConsumeInput { get; protected set;}

        public ScreenPopup()
        {
            ConsumeInput = true;
			IsPopupScreen = true;
        }

		public override void LoadContent(Object[] arguments)
		{
			base.LoadContent(arguments);
		}

        public override void Back(Object sender, String argument)
        {
            ThrowNavigationException();
        }

        public override void Back(Object sender)
        {
            ThrowNavigationException();
        }

        public override void Open(Object sender, String argument)
        {
            ThrowNavigationException();
        }

        public virtual void Close(Object sender)
        {
            Remove();
        }

        private void ThrowNavigationException()
        {
            String name = GetType().FullName;
            throw new Exception(String.Format("{0}: Navigation isn't allowed on popup screens.", name));
        }
    }
}

