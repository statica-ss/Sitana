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
using System.IO.IsolatedStorage;
using Microsoft.Phone.Tasks;
using System.Reflection;
using Microsoft.Phone.Controls;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Xna.Framework;
using System.Windows.Media;
using Windows.UI.ViewManagement;
using Windows.Graphics.Display;

namespace Ebatianos
{
    public static class SystemWrapper
    {
        private static PhoneApplicationPage _page;

        public static Assembly MainAssembly { set; private get; }

        public static EventHandler<OrientationChangedEventArgs> OrientationChanged;

        public static PhoneApplicationPage Page 
        {
            set
            {
                _page = value;
                InitializePage();
            }
            
            private get
            {
                return _page;
            }
        }

        private static TextBox _textBox;
        private static Action<Char> _onCharacterAction;
        private static Action _onKeyboardHidden;
        private static Int32 _keyboardHeight;

		public static Boolean CloseApp()
        {
			return false;
        }
		
        private static void InitializePage()
        {
            _textBox = Page.FindName("InputBox") as TextBox;

            _textBox.GotFocus += InputBox_GotFocus;
            _textBox.LostFocus += InputBox_LostFocus;
            _textBox.KeyUp += InputBox_KeyUp;

            _keyboardHeight = (Int32)(340.0f * DisplayProperties.LogicalDpi / 96.0f);
        }

        static void InputBox_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            Char character = '\0';
            
            switch(e.Key)
            {
                case System.Windows.Input.Key.Back:
                    character = '\b';
                    break;

                case System.Windows.Input.Key.Enter:
                    character = '\n';
                    break;

                default:
                    if (_textBox.Text.Length > 0)
                    {
                        character = _textBox.Text[0];
                        _textBox.Text = "";
                    }
                    break;
            }

            if (_onCharacterAction != null)
            {
                _onCharacterAction.Invoke(character);
            }

            e.Handled = true;
        }

        public static IsolatedStorageFile GetUserStoreForApplication()
        {
            return IsolatedStorageFile.GetUserStoreForApplication();
        }

        public static void OpenWebsite(String url)
        {
            WebBrowserTask task = new WebBrowserTask();
            task.Uri = new Uri(url, UriKind.RelativeOrAbsolute);
            task.Show();
        }

        public static void OpenMail(String name, String address, String subject, String text, Action actionAfterMail)
        {
            EmailComposeTask emailComposeTask = new EmailComposeTask();

            emailComposeTask.Subject = subject;
            emailComposeTask.Body = text;
            emailComposeTask.To = name + "<" + address + ">";

            emailComposeTask.Show();

            if (actionAfterMail != null)
            {
                actionAfterMail.Invoke();
            }
        }

        public static void OpenRatingPage()
        {
            MarketplaceReviewTask marketplaceReviewTask = new MarketplaceReviewTask();
            marketplaceReviewTask.Show();
        }

        public static String CurrentVersion
        {
            get
            {
                var nameHelper = new AssemblyName(MainAssembly.FullName);
                return nameHelper.Version.ToString();
            }
        }
        
        public static Single ShowKeyboard()
        {
            Deployment.Current.Dispatcher.BeginInvoke(() =>
            {
                _textBox.Visibility = Visibility.Visible;
                _textBox.Text = "";

                _textBox.Focus();
                Application.Current.RootVisual.RenderTransform = new ScaleTransform();
            });

            return _keyboardHeight;
        }

        public static Single ShowKeyboard(Rectangle rect, KeyboardContext context, Action<Char> onCharacterAction, Action onKeyboardHidden, Action<Single> unused)
        {
            if ( _onKeyboardHidden != null )
            {
                _onKeyboardHidden.Invoke();
                _onKeyboardHidden = null;
            }

            Deployment.Current.Dispatcher.BeginInvoke(() =>
            {
                Canvas.SetLeft(_textBox, rect.Left);
                Canvas.SetTop(_textBox, rect.Top);

                _textBox.Width = rect.Width;
                _textBox.Height = rect.Height;

                _textBox.Visibility = Visibility.Visible;
                _textBox.Text = "";

                _textBox.Focus();

                Application.Current.RootVisual.RenderTransform = new ScaleTransform();

                _onCharacterAction = onCharacterAction;
                _onKeyboardHidden = onKeyboardHidden;
            });

            return _keyboardHeight;
        }

        public static void HideKeyboard()
        {
            Deployment.Current.Dispatcher.BeginInvoke(new Action(() =>
            {
                _textBox.Visibility = Visibility.Collapsed;
            }));

            if (_onKeyboardHidden != null)
            {
                _onKeyboardHidden.Invoke();
                _onKeyboardHidden = null;
            }
        }

        private static void InputBox_GotFocus(object sender, RoutedEventArgs e)
        {
            Page.RenderTransform = new CompositeTransform();
        }

        private static void InputBox_LostFocus(object sender, RoutedEventArgs e)
        {
            TextBox box = Page.FindName("InputBox") as TextBox;
            box.Visibility = Visibility.Collapsed;

            if ( _onKeyboardHidden != null )
            {
                _onKeyboardHidden.Invoke();
            }
        }

        public static void DisableLock(Boolean disable)
        {

        }

    }
}
