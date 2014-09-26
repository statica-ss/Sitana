// SITANA - Copyright (C) The Sitana Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO.IsolatedStorage;
using MonoTouch.UIKit;
using MonoTouch.MessageUI;
using MonoTouch.Foundation;
using Microsoft.Xna.Framework;
using System.Reflection;
using System;

namespace Sitana.Framework
{
    public static class SystemWrapper
    {
        internal class CustomMailComposeDelegate : MFMailComposeViewControllerDelegate
        {
            private Action _onCompleted;


            public CustomMailComposeDelegate(Action onCompleted)
            {
                _onCompleted = onCompleted;
            }

            public override void Finished(MFMailComposeViewController controller,
                                        MFMailComposeResult result, NSError error)
            {
                controller.DismissViewController(true, new NSAction(DoCompletedAction));
            }

            public void DoCompletedAction()
            {
                if (_onCompleted != null)
                {
                    _onCompleted.Invoke();
                }
            }
        }

        public static EventHandler<OrientationChangedEventArgs> OrientationChanged;

        public static Game Game{ private get; set; }

        public static String AppId { private get; set; }

        public static UIApplication App { private get; set; }

        private static Action<Char> _onCharacterInput;
        private static Action _onKeyboardHidden;

        private static UITextField _textField = null;

        private static Object _keyboardLock = new Object();

        private static Boolean _noTextChange = false;

        public static Boolean CloseApp()
        {
			return true;
        }
		
        public static IsolatedStorageFile GetUserStoreForApplication()
        {
            return IsolatedStorageFile.GetUserStoreForApplication();
        }

        public static void InitKeyboard()
        {
            _textField = new UITextField(new System.Drawing.RectangleF(-10,-10,5,5));
            _textField.BackgroundColor = new UIColor(1,1,1,0);
            _textField.TextColor = new UIColor(1,1,1,0);
            _textField.Opaque = false;
            _textField.KeyboardAppearance = UIKeyboardAppearance.Default;
            _textField.AutocorrectionType = UITextAutocorrectionType.No;
            _textField.AutocapitalizationType = UITextAutocapitalizationType.None;
            _textField.ReturnKeyType = UIReturnKeyType.Default;
            _textField.SecureTextEntry = false;
            _textField.Ended += TextFieldEditingDidEnd;
            _textField.EditingChanged += TextFieldEditingChanged;
            _textField.Text = " ";

            _textField.ShouldReturn = delegate
            {
                lock(_keyboardLock)
                {
                    if ( _onCharacterInput != null)
                    {
                        _onCharacterInput('\n');
                    }
                }
                return true;
            };

            UIViewController controller = Game.Services.GetService(typeof(UIViewController)) as UIViewController;
            controller.View.AddSubview(_textField);
        }

        private static void TextFieldEditingChanged(Object sender, EventArgs e)
        {
            lock (_keyboardLock)
            {
                if (_noTextChange)
                {
                    return;
                }

                if (_textField != null)
                {
                    String t = String.Copy(_textField.Text);
                    _noTextChange = true;
                    _textField.Text = " ";
                    _noTextChange = false;

                    if (t.Length == 0)
                    {
                        if (_onCharacterInput != null)
                        {
                            _onCharacterInput('\b');
                        }
                    }
                    else
                    {
                        for (Int32 idx = 1; idx < t.Length; ++idx)
                        {
                            if (_onCharacterInput != null)
                            {
                                _onCharacterInput(t.ElementAt(idx));
                            }
                        }
                    }
                }
            }
        }

        private static void TextFieldEditingDidEnd(Object sender, EventArgs e)
        {
            lock (_keyboardLock)
            {
                if (_onKeyboardHidden != null)
                {
                    _onKeyboardHidden.Invoke();
                    _onKeyboardHidden = null;
                }
            }
        }

        public static void OpenWebsite(String url)
        {
            UIApplication.SharedApplication.OpenUrl(new MonoTouch.Foundation.NSUrl(url));
        }

        public static void OpenMail(String name, String address, String subject, String text, Action onCompleted)
        {
            MFMailComposeViewController mail = new MFMailComposeViewController();

            mail.MailComposeDelegate = new CustomMailComposeDelegate(onCompleted);
            mail.SetToRecipients(new string[] { name + "<" + address + ">" });
            mail.SetSubject(subject);
            mail.SetMessageBody(text, true);

            UIViewController controller = Game.Services.GetService(typeof(UIViewController)) as UIViewController;
            controller.PresentViewController(mail, true, null);
        }

        public static void OpenRatingPage()
        {
            String url = String.Format("itms-apps://ax.itunes.apple.com/WebObjects/MZStore.woa/wa/viewContentsUserReviews?type=Purple+Software&id={0}", AppId);
            UIApplication.SharedApplication.OpenUrl(new MonoTouch.Foundation.NSUrl(url));
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
            _textField.BecomeFirstResponder();

            // TODO: Determine if in landscape or in portrait mode.
            return CalculateKeyboardHeight(false);
        }

        public static Single ShowKeyboard(Rectangle rectangle, KeyboardContext context, Action<Char> onCharacterInput, Action onKeyboardHidden, Action<Single> onKeyboardLayoutChanged)
        {
            lock (_keyboardLock)
            {
                if (_onKeyboardHidden != null)
                {
                    _onKeyboardHidden.Invoke();
                    _onKeyboardHidden = null;
                }

                _onKeyboardHidden = onKeyboardHidden;
                _onCharacterInput = onCharacterInput;
            }

            UIKeyboardType type = UIKeyboardType.Default;

            switch (context)
            {
                case KeyboardContext.Email:
                    type = UIKeyboardType.EmailAddress;
                    break;

                case KeyboardContext.FirstLetterUppercase:
                    type = UIKeyboardType.NamePhonePad;
                    break;

                case KeyboardContext.Number:
                    type = UIKeyboardType.NumbersAndPunctuation;
                    break;

                case KeyboardContext.NormalText:
                    type = UIKeyboardType.Default;
                    break;
            }

            Single height = ShowKeyboard(type);

            //UiTask.BeginInvoke(() => onKeyboardLayoutChanged(height));

            return height;
        }

        private static Single ShowKeyboard(UIKeyboardType type)
        {
            if (type != _textField.KeyboardType && _textField.IsFirstResponder)
            {
                Action old = null;

                lock (_keyboardLock)
                {
                    old = _onKeyboardHidden;
                    _onKeyboardHidden = null;
                }

                _textField.ResignFirstResponder();

                _textField.KeyboardType = type;
                ShowKeyboard();

                lock (_keyboardLock)
                {
                    _onKeyboardHidden = old;
                }

                return CalculateKeyboardHeight(false);
            }

            _textField.KeyboardType = type;
            return ShowKeyboard();
        }

        public static void HideKeyboard()
        {
            lock(_keyboardLock)
            {
                _textField.ResignFirstResponder();
            }
        }

        /// <summary>
        /// Calculates the height of the keyboard.
        /// </summary>
        /// <returns>The keyboard height.</returns>
        /// <param name="landscape">If set to <c>true</c> return keyboard height for landscape mode.</param>
        public static Single CalculateKeyboardHeight(Boolean landscape)
        {
            Int32 defWidth = Game.GraphicsDevice.Viewport.Width;
            Console.WriteLine("Default Width: {0}", defWidth);

            if ( landscape )
            {
                switch(defWidth)
                {
                    case 320:
                        return 162;

                    case 640:
                        return 324;

                    case 768:
                        return 352;

                    case 1536:
                        return 704;
                }
            }
            else
            {
                switch(defWidth)
                {
                    case 320:
                        return 216;

                    case 640:
                        return 432;

                    case 768:
                        return 264;

                    case 1536:
                        return 528;
                }
            }

            return 0;
        }

        internal static void OnOrientationChanged(Orientation orientation)
        {
            if (OrientationChanged != null)
            {
                OrientationChanged(null, new OrientationChangedEventArgs(orientation));
            }
        }

        public static void InitRotation()
        {
            UIDevice.CurrentDevice.BeginGeneratingDeviceOrientationNotifications();
            NSNotificationCenter.DefaultCenter.AddObserver("UIDeviceOrientationDidChangeNotification", new Action<NSNotification>(OrientationChangedInternal), UIDevice.CurrentDevice);
        }

        private static void OrientationChangedInternal(NSNotification notify)
        {
            UIDevice device = notify.Object as UIDevice;

            if (device != null)
            {
                switch (device.Orientation)
                {
                case UIDeviceOrientation.LandscapeLeft:
                    OnOrientationChanged(Orientation.LandscapeLeft);
                    break;

                case UIDeviceOrientation.LandscapeRight:
                    OnOrientationChanged(Orientation.LandscapeRight);
                    break;

                case UIDeviceOrientation.Portrait:
                    OnOrientationChanged(Orientation.Portrait);
                    break;
                }
            }
        }

        public static void DisableLock(Boolean disable)
        {
            App.IdleTimerDisabled = disable;
        }
    }
}
