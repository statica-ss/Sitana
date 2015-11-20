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
using Microsoft.Xna.Framework;
using Windows.Graphics.Display;
using Windows.UI.Xaml.Controls;

namespace Ebatianos
{
    public partial class TextInput
    {
        static TextBox _textField = null;

        static PasswordBox _passwordField = null;

        bool _internalTextChange = false;
        ITextEdit _controller;

        static TextInput CurrentFocus = null;
        static PhoneApplicationFrame RootFrame = null;

        static InputScope _inputScope = new InputScope();

        static InputScopeName _inputScopeNumber = new InputScopeName() { NameValue = InputScopeNameValue.NumberFullWidth };
        static InputScopeName _inputScopePassword = new InputScopeName() { NameValue = InputScopeNameValue.Password };
        static InputScopeName _inputScopeName = new InputScopeName() { NameValue = InputScopeNameValue.PersonalFullName };
        static InputScopeName _inputScopeText = new InputScopeName() { NameValue = InputScopeNameValue.Text };
        static InputScopeName _inputScopeUpper = new InputScopeName() { NameValue = InputScopeNameValue.Text };
        static InputScopeName _inputScopeEmail = new InputScopeName() { NameValue = InputScopeNameValue.EmailNameOrAddress };

        bool _uppercase = false;
        bool _password = false;
        bool _multiline = false;

        public static void Init(Page page, PhoneApplicationFrame rootFrame)
        {
            RootFrame = rootFrame;

            Grid grid = page.FindName("LayoutRoot") as Grid;
            Canvas canvas = new Canvas();

            grid.Children.Add(canvas);

            _textField = new TextBox();
            _passwordField = new PasswordBox();
            
            canvas.Children.Add(_textField);
            canvas.Children.Add(_passwordField);

            canvas.HorizontalAlignment = HorizontalAlignment.Stretch;
            canvas.VerticalAlignment = VerticalAlignment.Stretch;

            _textField.Background = new SolidColorBrush(System.Windows.Media.Colors.White);
            _textField.Visibility = Visibility.Collapsed;
            _textField.HorizontalAlignment = HorizontalAlignment.Left;
            _textField.VerticalAlignment = VerticalAlignment.Top;
            _textField.Margin = new Thickness(0);
            _textField.Padding = new Thickness(0);

            _textField.InputScope = _inputScope;

            _passwordField.Background = new SolidColorBrush(System.Windows.Media.Colors.White);
            _passwordField.Visibility = Visibility.Collapsed;
            _passwordField.HorizontalAlignment = HorizontalAlignment.Left;
            _passwordField.VerticalAlignment = VerticalAlignment.Top;
            _passwordField.Margin = new Thickness(0);
            _passwordField.Padding = new Thickness(0);
        }

        public TextInput(Rectangle position, KeyboardContext keyboardContext, string text, int textSize, Align contentAlign, ITextEdit controller)
        {
            
            if (CurrentFocus != null)
            {
                CurrentFocus.UnfocusInternal();
            }

            _controller = controller;
            CurrentFocus = this;

            float dipFactor = DisplayProperties.LogicalDpi / 96.0f;

            if (keyboardContext == KeyboardContext.Password)
            {
                _password = true;

                Canvas.SetLeft(_passwordField, (position.X + 1) / dipFactor);
                Canvas.SetTop(_passwordField, (position.Y + 2) / dipFactor);

                _passwordField.Width = (position.Width - 2) / dipFactor;
                _passwordField.Height = (position.Height - 4) / dipFactor;

                SetText(text);

                _passwordField.PasswordChanged += HandleEditingChangedPassword;
                _passwordField.LostFocus += HandleEditingDidEnd;
                _passwordField.KeyUp += HandleKeyUp;

                Deployment.Current.Dispatcher.BeginInvoke(() =>
                {
                    _passwordField.Visibility = Visibility.Visible;
                    _passwordField.Focus();
                    RootFrame.RenderTransform = new CompositeTransform();
                });
            }
            else
            {
                Canvas.SetLeft(_textField, (position.X + 1) / dipFactor);
                Canvas.SetTop(_textField, (position.Y + 2) / dipFactor);

                _textField.Width = (position.Width - 2) / dipFactor;
                _textField.Height = (position.Height - 4) / dipFactor;

                switch (contentAlign & Align.Horz)
                {
                    case Align.Left:
                        _textField.TextAlignment = TextAlignment.Left;
                        break;

                    case Align.Right:
                        _textField.TextAlignment = TextAlignment.Right;
                        break;

                    case Align.Center:
                        _textField.TextAlignment = TextAlignment.Center;
                        break;
                }

                _inputScope.Names.Clear();
                _inputScope.Names.Add(NameFromContext(keyboardContext));

                _uppercase = keyboardContext == KeyboardContext.Uppercase;

                if (keyboardContext == KeyboardContext.MultilineText)
                {
                    _textField.VerticalContentAlignment = VerticalAlignment.Top;
                    _textField.AcceptsReturn = true;
                    _textField.TextWrapping = TextWrapping.Wrap;
                    _multiline = true;
                }

                else
                {
                    _textField.VerticalContentAlignment = VerticalAlignment.Center;
                    _textField.AcceptsReturn = false;
                    _textField.TextWrapping = TextWrapping.NoWrap;
                    _multiline = false;
                }

                SetText(text);

                _textField.TextChanged += HandleEditingChanged;
                _textField.LostFocus += HandleEditingDidEnd;
                _textField.KeyUp += HandleKeyUp;

                Deployment.Current.Dispatcher.BeginInvoke(() =>
                {
                    _textField.Visibility = Visibility.Visible;
                    _textField.Focus();
                    RootFrame.RenderTransform = new CompositeTransform();
                });
            };
        }

        void HandleKeyUp(object sender, KeyEventArgs e)
        {
            if ( e.Key == Key.Enter && !_multiline)
            {
                _controller.Return();
            }
        }

        object NameFromContext(KeyboardContext context)
        {
            switch(context)
            {
                case KeyboardContext.FirstLetterUppercase:
                    return _inputScopeName;

                case KeyboardContext.Number:
                    return _inputScopeNumber;

                case KeyboardContext.Uppercase:
                    return _inputScopeName;

                case KeyboardContext.Email:
                    return _inputScopeEmail;
            }

            return _inputScopeText;
        }

        void HandleEditingDidEnd(object sender, EventArgs e)
        {
            Unfocus();
            _controller.LostFocus();
        }

        void HandleEditingChangedPassword(object sender, RoutedEventArgs e)
        {
            if (_internalTextChange)
                return;

            String newText = _passwordField.Password;

            string text = _controller.TextChanged(newText);

            if (text != newText)
            {
                SetText(text);
            }
        }

        void HandleEditingChanged(object sender, EventArgs e)
        {
            if (_internalTextChange)
                return;

            String newText = _textField.Text;

            if ( _uppercase )
            {
                newText = newText.ToUpper();

                int selStart = _textField.SelectionStart;
                int selLength = _textField.SelectionLength;

                _textField.Text = newText;
                _textField.Select(selStart, selLength);
            }

            string text = _controller.TextChanged(newText);

            if (text != newText)
            {
                SetText(text);
            }
        }

        public void SetText(string text)
        {
            Deployment.Current.Dispatcher.BeginInvoke(() =>
            {
                _internalTextChange = true;
                if (_password)
                {
                    _passwordField.Password = text;
                    _passwordField.SelectAll();
                }
                else
                {
                    _textField.Text = text;
                    _textField.Select(text.Length, 0);
                }
                _internalTextChange = false;
            });
        }

        private void UnfocusInternal()
        {
            _textField.Visibility = Visibility.Collapsed;
            _passwordField.Visibility = Visibility.Collapsed;

            _textField.TextChanged -= HandleEditingChanged;
            _textField.LostFocus -= HandleEditingDidEnd;

            _passwordField.PasswordChanged -= HandleEditingChanged;
            _passwordField.LostFocus -= HandleEditingDidEnd;

            _passwordField.KeyUp -= HandleKeyUp;
            _textField.KeyUp -= HandleKeyUp;

            _controller.LostFocus();

            CurrentFocus = null;
        }

        public void Unfocus()
        {
            Deployment.Current.Dispatcher.BeginInvoke(() =>
            {
                UnfocusInternal();
            });
        }
    }
}


