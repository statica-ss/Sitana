// SITANA - Copyright (C) The Sitana Team.
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using Android.Views.InputMethods;
using Microsoft.Xna.Framework;
using Android.Content;
using Android.Views;
using Ebatianos.Gui;
using Ebatianos.Input;
using System.Threading;
using Android.Widget;
using System.Collections.Generic;

namespace Ebatianos
{
    public class ActivityBase : Microsoft.Xna.Framework.AndroidGameActivity, ViewTreeObserver.IOnGlobalLayoutListener
	{
		View _view;
        Action<Single> _onLayoutChanged;

        void ViewTreeObserver.IOnGlobalLayoutListener.OnGlobalLayout()
        {
            Android.Graphics.Rect rect = new Android.Graphics.Rect();
            Window.DecorView.GetWindowVisibleDisplayFrame(rect);

            Single size = _view.Height - rect.Height();

            if (_onLayoutChanged != null)
            {
                _onLayoutChanged.Invoke(size);
            }
        }

        public void ToggleKeyboard(Action<Single> onLayoutChanged) 
		{
            _onLayoutChanged = onLayoutChanged;

            InputMethodManager imm = (InputMethodManager)GetSystemService(Context.InputMethodService);
            imm.ToggleSoftInputFromWindow(_view.WindowToken, ShowSoftInputFlags.Forced, HideSoftInputFlags.ImplicitOnly);
		}

		public void HideKeyboard() 
		{
            InputMethodManager imm = (InputMethodManager)GetSystemService(Context.InputMethodService);
            imm.HideSoftInputFromWindow(CurrentFocus.WindowToken, 0);
		}

		void Activity_KeyPress(Object sender, View.KeyEventArgs e) 
		{
			if (e.Event.Action == KeyEventActions.Multiple)
			{
				if (e.Event.KeyCode == Keycode.Unknown)
				{
					for (Int32 idx = 0; idx < e.Event.Characters.Length; ++idx)
					{
						SystemWrapper.OnKey (e.Event.Characters [idx]);
					}
				} 
				else
				{
					Char c = (Char)e.Event.UnicodeChar;
					SystemWrapper.OnKey (c);
				}
				return;
			}

			if (e.Event.Action == KeyEventActions.Up && e.Event.KeyCode == Keycode.Back) 
			{
				InputHandler.Current.EscapePressed = true;
				return;
			}

			if (e.Event.Action == KeyEventActions.Up && e.Event.KeyCode == Keycode.Menu) 
			{
				InputHandler.Current.MenuPressed = true;
				return;
			}

			if (e.Event.Action == KeyEventActions.Up && e.Event.KeyCode == Keycode.Del) 
			{
				SystemWrapper.OnKey('\b');
				return;
			}

			if (e.Event.Action == KeyEventActions.Up)
			{
				Char c = (Char)e.Event.UnicodeChar;
				SystemWrapper.OnKey(c);
			}
		}

        public void InitRotation()
        {
            OrientationEventListener rotationListener = new AndroidOrientationEventListener(this);
            rotationListener.Enable();
        }

		public void Initialize(View view)
		{
            _view = view;
            _view.KeyPress += Activity_KeyPress;

            Window.DecorView.ViewTreeObserver.AddOnGlobalLayoutListener(this);
		}

        public void DoExit()
        {
            //var intent = new Intent(Intent.ActionMain);

            //StartActivity(intent);

              //base.OnBackPressed();

            MoveTaskToBack(true);
        }

        public void DisableLock(Boolean disable)
        {
            if (_view != null)
            {
                _view.KeepScreenOn = disable;
            }
        }
	}
}

