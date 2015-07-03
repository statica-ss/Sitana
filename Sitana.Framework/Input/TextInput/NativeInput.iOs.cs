// /// This file is a part of the EBATIANOS.ESSENTIALS class library.
// /// (c)2013-2014 EBATIANO'S a.k.a. Sebastian Sejud. All rights reserved.
// ///
// /// THIS SOURCE FILE IS THE PROPERTY OF EBATIANO'S A.K.A. SEBASTIAN SEJUD 
// /// AND IS NOT TO BE RE-DISTRIBUTED BY ANY MEANS WHATSOEVER WITHOUT 
// /// THE EXPRESSED WRITTEN CONSENT OF EBATIANO'S A.K.A. SEBASTIAN SEJUD.
// ///
// /// THIS SOURCE CODE CAN ONLY BE USED UNDER THE TERMS AND CONDITIONS OUTLINED
// /// IN THE EBATIANOS.ESSENTIALS LICENSE AGREEMENT. 
// /// EBATIANO'S A.K.A. SEBASTIAN SEJUD GRANTS TO YOU (ONE SOFTWARE DEVELOPER) 
// /// THE LIMITED RIGHT TO USE THIS SOFTWARE ON A SINGLE COMPUTER.
// ///
// /// CONTACT INFORMATION:
// /// contact@ebatianos.com
// /// www.ebatianos.com/essentials-library
// /// 
// ///---------------------------------------------------------------------------
//
using System;
using Sitana.Framework.Cs;
using Microsoft.Xna.Framework;
using UIKit;
using MessageUI;
using Foundation;
using Sitana.Framework.Ui.Core;
using Sitana.Framework;
using Sitana.Framework.Input;

namespace Sitana.Framework.Input
{
	public partial class NativeInput
    {
        static UITextField _textField = null;
        static UITextView _textView = null;

        bool _internalTextChange = false;

        ITextEdit _controller;

        internal static NativeInput CurrentFocus = null;

        bool _useTextView = false;

        public bool Visible
        {
            get
            {
                return true;
            }
        }

        private void InitTextField()
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


            _textView = new UITextView(new System.Drawing.RectangleF(-10, -10, 5, 5));
            _textView.BackgroundColor = new UIColor(1,1,1,0);
            _textView.TextColor = new UIColor(1,1,1,0);
            _textView.Opaque = false;
            _textView.KeyboardAppearance = UIKeyboardAppearance.Default;
            _textView.AutocorrectionType = UITextAutocorrectionType.No;
            _textView.AutocapitalizationType = UITextAutocapitalizationType.None;
            _textView.ReturnKeyType = UIReturnKeyType.Default;
            _textView.SecureTextEntry = false;

            UIViewController viewController = AppMain.Current.Services.GetService(typeof(UIViewController)) as UIViewController;
            viewController.View.AddSubview(_textField);
            viewController.View.AddSubview(_textView);
        }

		public NativeInput(Rectangle position, TextInputType keyboardContext, string text, int textSize, Align align, ITextEdit controller)
        {
            if (CurrentFocus != null)
            {
                CurrentFocus.Unfocus();
            }

            _controller = controller;
            CurrentFocus = this;

            if (_textField == null)
            {
                InitTextField();
            }

			text = text.Replace('\n', '\r');

			Bottom = position.Bottom + (int)Platform.PointsToPixels(10);
			position.Y += AppMain.Current.SetFocus(this);

            float x = Platform.PixelsToPoints(position.X) + 1;
			float y = Platform.PixelsToPoints(position.Y) + 2;
			float width = Platform.PixelsToPoints(position.Width) - 2;
			float height = Platform.PixelsToPoints(position.Height) - 4;

			if (keyboardContext.HasFlag(TextInputType.PasswordClass))
			{
				keyboardContext &= ~(TextInputType.NoSuggestions);
			}

			if (keyboardContext.HasFlag(TextInputType.MultilineText))
            {
                _useTextView = true;

                _textView.Frame = new System.Drawing.RectangleF(x, y, width, height);
                _textView.BackgroundColor = new UIColor(255, 255, 255, 255);
                _textView.TextColor = new UIColor(0, 0, 0, 255);
                _textView.Opaque = true;
                _textView.KeyboardType = TypeFromContext(keyboardContext);
                _textView.AutocapitalizationType = AutoCapitalizationFromContext(keyboardContext);

                _textView.KeyboardType = UIKeyboardType.Default;

				_textView.AutocorrectionType = keyboardContext.HasFlag(TextInputType.NoSuggestions) ?  UITextAutocorrectionType.No :  UITextAutocorrectionType.Default;

				_textView.SecureTextEntry = keyboardContext.HasFlag(TextInputType.PasswordClass);

				_textView.Font = UIFont.FromName("Helvetica", textSize);
                SetText(text);

                _textView.Ended += HandleEnded;
                _textView.BecomeFirstResponder();
            }
            else
            {
                _textField.Frame = new System.Drawing.RectangleF(x, y, width, height);
                _textField.BackgroundColor = new UIColor(255, 255, 255, 255);
                _textField.TextColor = new UIColor(0, 0, 0, 255);
                _textField.Opaque = true;
                _textField.KeyboardType = TypeFromContext(keyboardContext);
				_textField.AutocapitalizationType = AutoCapitalizationFromContext(keyboardContext);

				_textField.ResignFirstResponder();

				_textField.AutocorrectionType = keyboardContext.HasFlag(TextInputType.NoSuggestions) ?  UITextAutocorrectionType.No :  UITextAutocorrectionType.Default;
				_textField.SecureTextEntry = keyboardContext.HasFlag(TextInputType.PasswordClass);

				_textField.ClearsOnBeginEditing = false;

                _textField.Font = UIFont.FromName("Helvetica", textSize);

                switch (align & Align.Horz)
                {
                    case Align.Center:
                        _textField.TextAlignment = UITextAlignment.Center;
                        break;

                    case Align.Left:
                        _textField.TextAlignment = UITextAlignment.Left;
                        break;

                    case Align.Right:
                        _textField.TextAlignment = UITextAlignment.Right;
                        break;
                }

                SetText(text);
                _textField.EditingChanged += HandleEditingChanged;
                _textField.EditingDidEnd += HandleEditingDidEnd;


                _textField.ShouldReturn = delegate
                {
					if ((keyboardContext & TextInputType.TypeFilter) != TextInputType.MultilineText)
                    {
                        _controller.Return();
                        return true;
                    }

                    return false;
                };

                _textField.BecomeFirstResponder();
            }



        }

        void HandleEnded(object sender, EventArgs e)
        {
            _textView.Ended += HandleEnded;

			_controller.TextChanged(_textView.Text.Replace('\r', '\n'));

            Unfocus();
            _controller.LostFocus();
        }

		UITextAutocapitalizationType AutoCapitalizationFromContext(TextInputType context)
        {
			switch (context & TextInputType.TypeFilter)
            {
			case TextInputType.FirstLetterUppercase:
                    return UITextAutocapitalizationType.Words;

			case TextInputType.Uppercase:
                    return UITextAutocapitalizationType.AllCharacters;

			case TextInputType.MultilineText:
                    return UITextAutocapitalizationType.Sentences;
            }

            return UITextAutocapitalizationType.None;
        }

		UIKeyboardType TypeFromContext(TextInputType context)
        {
			switch (context & TextInputType.TypeFilter)
            {
			case TextInputType.Email:
                    return UIKeyboardType.EmailAddress;

			case TextInputType.Digits:
				return UIKeyboardType.NumberPad;

			case TextInputType.Numeric:
                    return UIKeyboardType.NumbersAndPunctuation;
            }

            return UIKeyboardType.Default;
        }

        void HandleEditingDidEnd(object sender, EventArgs e)
        {
            Unfocus();
            _controller.LostFocus();
        }

        void HandleEditingChanged(object sender, EventArgs e)
        {
            if (_internalTextChange)
                return;

			string newText = _textField.Text.Replace('\r', '\n');

            string text = _controller.TextChanged(newText);

            if (text != newText)
            {
                SetText(text);
            }
        }

        public void SetText(string text)
        {
            _internalTextChange = true;

            if (_useTextView)
            {
                _textView.Text = text;
            }
            else
            {   
                _textField.Text = text;
            }
            _internalTextChange = false;
        }

        public void Unfocus()
        {
            if (_useTextView)
            {
                _textView.EndEditing(true);
                _textView.ResignFirstResponder();
                _textView.Opaque = false;

                _textView.BackgroundColor = new UIColor(1, 1, 1, 0);
                _textView.TextColor = new UIColor(1, 1, 1, 0);

                _textView.Frame = new System.Drawing.RectangleF(-10, -10, 5, 5);

                _textView.Ended -= HandleEnded;
            }
            else
            {
                _textField.EndEditing(true);
                _textField.ResignFirstResponder();
                _textField.Opaque = false;

                _textField.BackgroundColor = new UIColor(1, 1, 1, 0);
                _textField.TextColor = new UIColor(1, 1, 1, 0);

                _textField.Frame = new System.Drawing.RectangleF(-10, -10, 5, 5);

                _textField.EditingChanged -= HandleEditingChanged;
                _textField.EditingDidEnd -= HandleEditingDidEnd;

                _textField.ShouldReturn = delegate
                {
                    return false;
                };
            }

            CurrentFocus = null;
			DelayedActionInvoker.Instance.AddAction(0.2f, (v)=>AppMain.Current.ReleaseFocus(this));
        }
    }
}


