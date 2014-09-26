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
using Sitana.Framework.Gui;

namespace Sitana.Framework.Gui
{
	public class ScreenMessageBox : ScreenPopup
	{
		private String _message;
        private String _caption;

        private Action _hideAction;

		protected override void Init (ScreenManager screenManager, Object[] arguments)
		{
			base.Init (screenManager, arguments);

			_message = (String)arguments[0];
            _caption = (String)arguments[1];

            try
            {
                _hideAction = (Action)arguments[2];
            }
            catch
            {
            }
		}

		public String GetMessage()
		{
			return _message;
		}

        public String GetCaption()
        {
            return _caption;
        }

        public override void OnRemoved()
        {
            base.OnRemoved();

            if (_hideAction != null)
            {
                _hideAction.Invoke();
            }
        }
	}
}

