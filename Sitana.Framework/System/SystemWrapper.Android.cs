using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO.IsolatedStorage;
using Microsoft.Xna.Framework;
using System.Reflection;
using System.Threading;
using Android.Content;

namespace Ebatianos
{
	public static class SystemWrapper
	{
		public static Game Game{ private get; set; }

		public static String AppId { private get; set; }

		public static ActivityBase Activity { get; set; }

		private static Action<Char> _onCharacterAction;
		private static Action _onKeyboardHide;

		private static Object _keyboardLock = new Object();

        private static Single _keyboardSize = 0;

        public static EventHandler<OrientationChangedEventArgs> OrientationChanged;

		public static IsolatedStorageFile GetUserStoreForApplication ()
		{
			return IsolatedStorageFile.GetUserStoreForApplication ();
		}

		public static void OpenWebsite(String url)
		{
           var uri = Android.Net.Uri.Parse(url);
           var intent = new Intent(Intent.ActionView, uri);
           Activity.StartActivity(intent);
		}

        public static Boolean CloseApp()
        {
            Activity.DoExit();
            return true;
        }

		public static void OpenMail (String name, String address, String subject, String text, Action onCompleted)
		{
		}

		public static void OpenRatingPage()
		{
		}

		public static String CurrentVersion 
		{
			get 
			{
				return "1.0.1.2345";
			}
		}

        public static Single ShowKeyboard()
		{
            Activity.ToggleKeyboard(null);
            return _keyboardSize;
		}
			
        public static Single ShowKeyboard(Rectangle elementRect, KeyboardContext context, Action<Char> onCharacterInput, Action onKeyboardHide, Action<Single> onKeyboardResized)
		{
            lock (_keyboardLock)
            {
                if (onKeyboardHide != null)
                {
                    if (_onKeyboardHide != null)
                    {
                        _onKeyboardHide.Invoke();
                        _onKeyboardHide = null;
                    }

                    _onKeyboardHide = onKeyboardHide;
                }

                _onCharacterAction = onCharacterInput;
            }

            Activity.ToggleKeyboard( (s)=>
            {
                _keyboardSize = s;
                if ( onKeyboardResized != null )
                {
                    onKeyboardResized.Invoke(s);
                }
            });

            return _keyboardSize;
		}

		public static void HideKeyboard()
		{
			Activity.HideKeyboard();

			if (_onCharacterAction != null)
			{
				_onCharacterAction = null;
			}

			if (_onKeyboardHide != null)
			{
				_onKeyboardHide.Invoke();
				_onKeyboardHide = null;
			}
		}

		public static void OnKey(Char character)
		{
			if (_onCharacterAction != null) 
			{
				_onCharacterAction.Invoke(character);
			}
		}

		internal static void KeyboardHidden()
		{
			lock (_keyboardLock)
			{
				if (_onKeyboardHide != null)
				{
					_onKeyboardHide.Invoke();
					_onKeyboardHide = null;
				}
			}
		}

        internal static void OnOrientationChanged(Orientation orientation)
        {
            if (OrientationChanged != null)
            {
                OrientationChanged(Activity, new OrientationChangedEventArgs(orientation));
            }
        }

        public static void DisableLock(Boolean disable)
        {
            if (Activity != null)
            {
                Activity.DisableLock(disable);
            }
        }
	}
}
