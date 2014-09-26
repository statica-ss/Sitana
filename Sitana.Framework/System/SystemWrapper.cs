using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.IsolatedStorage;
using System.Reflection;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Sitana.Framework
{
    public static class SystemWrapper
    {
        public static event EventHandler<OrientationChangedEventArgs> OrientationChanged;
    
        private static Action _onKeyboardClose;
        private static KeyboardHandler _keyboardHook;

        public static IsolatedStorageFile GetUserStoreForApplication()
        {
            return IsolatedStorageFile.GetStore(IsolatedStorageScope.User | IsolatedStorageScope.Assembly | IsolatedStorageScope.Domain, typeof(System.Security.Policy.Url), typeof(System.Security.Policy.Url));
        }
        
        public static Boolean CloseApp()
        {
            return false;
        }

        public static void OpenWebsite(String url)
        {
            if (url.StartsWith(Uri.UriSchemeHttp) || url.StartsWith(Uri.UriSchemeHttps))
            {
                Process.Start(url);
            }
        }

        public static void OpenMail(String name, String address, String subject, String text, Action completedAction)
        {
            name = Uri.EscapeDataString(name);
            subject = Uri.EscapeDataString(subject);
            text = Uri.EscapeDataString(text);
            String command = String.Format("mailto:{0}<{1}>?subject={2}&body={3}", name, address, subject, text);

            Process.Start(command);

            if (completedAction != null)
            {
                completedAction.Invoke();
            }
        }

        public static void OpenRatingPage()
        {
            OpenWebsite("http://ebatianos.com");
        }

        public static String CurrentVersion
        {
            get
            {
                Version version = Assembly.GetEntryAssembly().GetName().Version;
                return version.ToString();
            }
        }

        public static Single ShowKeyboard()
        {
            return 0;
        }

        public static Single ShowKeyboard(Rectangle rect, KeyboardContext context, Action<Char> onCharacterInput, Action onKeyboardClose, Action<Single> onSizeChanged)
        {
            HideKeyboard();

            _onKeyboardClose = onKeyboardClose;
            _keyboardHook.OnCharacter = onCharacterInput;
            return 0;
        }

        public static void HideKeyboard()
        {
            _keyboardHook.OnCharacter = null;

            if (_onKeyboardClose != null)
            {
                _onKeyboardClose.Invoke();
                _onKeyboardClose = null;
            }
        }

        public static void InitKeyboard(IntPtr windowId)
        {
            _keyboardHook = new KeyboardHandler(windowId);
        }

        public static void DisableLock(Boolean disable)
        {

        }
    }
}
