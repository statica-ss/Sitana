using System;
using Sitana.Framework.Cs;
using Microsoft.Xna.Framework;
using Sitana.Framework.Ui.Core;
using Sitana.Framework;
using Sitana.Framework.Input;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml;
using Windows.UI.Core;
using Windows.System;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media;
using Windows.Graphics.Display;

namespace Sitana.Framework.Input
{
	public partial class NativeInput
    {
        static TextBox _textBox = null;
        static PasswordBox _passwordBox = null;

        bool _usePasswordBox = false;
        bool _internalTextChange = false;
        bool _uppercase = false;
        bool _multiline = false;

        ITextEdit _controller;

        internal static NativeInput CurrentFocus = null;

        static InputScope _inputScope = new InputScope();

        static InputScopeName _inputScopeDigits = new InputScopeName() { NameValue = InputScopeNameValue.Number };
        static InputScopeName _inputScopeNumber = new InputScopeName() { NameValue = InputScopeNameValue.NumberFullWidth };
        static InputScopeName _inputScopeName = new InputScopeName() { NameValue = InputScopeNameValue.NameOrPhoneNumber };
        static InputScopeName _inputScopeText = new InputScopeName() { NameValue = InputScopeNameValue.Default };
        static InputScopeName _inputScopeUpper = new InputScopeName() { NameValue = InputScopeNameValue.Default };
        static InputScopeName _inputScopeEmail = new InputScopeName() { NameValue = InputScopeNameValue.EmailSmtpAddress };

        public bool Visible
        {
            get
            {
                return true;
            }
        }

        public static void Init(Grid layoutRoot)
        {
            Canvas canvas = new Canvas();

            layoutRoot.Children.Add(canvas);

            _textBox = new TextBox();
            _passwordBox = new PasswordBox();

            canvas.Children.Add(_textBox);
            canvas.Children.Add(_passwordBox);

            canvas.HorizontalAlignment = HorizontalAlignment.Stretch;
            canvas.VerticalAlignment = VerticalAlignment.Stretch;

            _textBox.Background = new SolidColorBrush(Windows.UI.Colors.White);
            _textBox.Visibility = Visibility.Collapsed;
            _textBox.HorizontalAlignment = HorizontalAlignment.Left;
            _textBox.VerticalAlignment = VerticalAlignment.Top;
            _textBox.VerticalContentAlignment = VerticalAlignment.Center;
            _textBox.Margin = new Thickness(0);
            _textBox.Padding = new Thickness(0);


            _passwordBox.Background = new SolidColorBrush(Windows.UI.Colors.White);
            _passwordBox.Visibility = Visibility.Collapsed;
            _passwordBox.HorizontalAlignment = HorizontalAlignment.Left;
            _passwordBox.VerticalAlignment = VerticalAlignment.Top;
            _passwordBox.VerticalContentAlignment = VerticalAlignment.Center;
            _passwordBox.Margin = new Thickness(0);
            _passwordBox.Padding = new Thickness(0);
        }

		public NativeInput(Rectangle position, TextInputType keyboardContext, string text, int textSize, Align align, ITextEdit controller)
        {
            if (CurrentFocus != null)
            {
                CurrentFocus.Unfocus();
            }

            _controller = controller;
            CurrentFocus = this;

			text = text.Replace('\n', '\r');

            var info = DisplayInformation.GetForCurrentView();
            double scale = info.RawPixelsPerViewPixel;

            if (keyboardContext == TextInputType.Password)
            {
                _usePasswordBox = true;
                _passwordBox.PasswordChanged += HandleEditingChangedPassword;
                _passwordBox.LostFocus += HandleEditingDidEnd;
                _passwordBox.KeyUp += HandleKeyUp;

                Canvas.SetLeft(_passwordBox, (position.X+2) / scale);

                _passwordBox.Width = (position.Width-4) / scale;
                _passwordBox.Height = _passwordBox.MinHeight;
                Canvas.SetTop(_passwordBox, (position.Center.Y / scale - _passwordBox.Height / 2));
                
                _passwordBox.Visibility = Visibility.Visible;
                _passwordBox.Focus(FocusState.Pointer);
            }
            else
            {
                _usePasswordBox = false;

                _textBox.TextChanged += HandleEditingChanged;
                _textBox.LostFocus += HandleEditingDidEnd;
                _textBox.KeyUp += HandleKeyUp;

                Canvas.SetLeft(_textBox, (position.X+2) / scale);
                

                _textBox.Width = (position.Width-2) / scale;

                switch (align & Align.Horz)
                {
                    case Align.Left:
                        _textBox.TextAlignment = TextAlignment.Left;
                        break;

                    case Align.Right:
                        _textBox.TextAlignment = TextAlignment.Right;
                        break;

                    case Align.Center:
                        _textBox.TextAlignment = TextAlignment.Center;
                        break;
                }

                _inputScope.Names.Clear();
                _inputScope.Names.Add(NameFromContext(keyboardContext));
                _textBox.InputScope = _inputScope;

                _textBox.IsTextPredictionEnabled = !keyboardContext.HasFlag(TextInputType.NoSuggestions);
                _uppercase = keyboardContext.HasFlag(TextInputType.Uppercase);

                if (keyboardContext == TextInputType.MultilineText)
                {
                    _textBox.VerticalContentAlignment = VerticalAlignment.Top;
                    _textBox.AcceptsReturn = true;
                    _textBox.TextWrapping = TextWrapping.Wrap;
                    _multiline = true;

                    Canvas.SetTop(_textBox, (position.Y + 2) / scale);
                    _textBox.Height = (position.Height - 4) / scale;
                }
                else
                {
                    _textBox.VerticalContentAlignment = VerticalAlignment.Center;
                    _textBox.AcceptsReturn = false;
                    _textBox.TextWrapping = TextWrapping.NoWrap;
                    _multiline = false;

                    _textBox.Height = _textBox.MinHeight;
                    Canvas.SetTop(_textBox, (position.Center.Y / scale - _textBox.Height / 2));
                    
                }

                SetText(text);

                CoreDispatcher dispatcher = CoreWindow.GetForCurrentThread().Dispatcher;

                _textBox.Visibility = Visibility.Visible;
                _textBox.Focus(FocusState.Pointer);
            }
        }

        
        public void SetText(string text)
        {
            _internalTextChange = true;

            if (_usePasswordBox)
            {
                _passwordBox.Password = text;
            }
            else
            {   
                _textBox.Text = text;
                _textBox.Select(text.Length, 0);
            }
            _internalTextChange = false;
        }

        void HandleEditingChangedPassword(object sender, RoutedEventArgs e)
        {
            if (_internalTextChange)
                return;

            string newText = _passwordBox.Password;

            string text = _controller.TextChanged(newText);

            if (text != newText)
            {
                SetText(text);
            }
        }

        void HandleEditingDidEnd(object sender, RoutedEventArgs e)
        {
            Unfocus();
            _controller.LostFocus();
        }

        void HandleKeyUp(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == VirtualKey.Enter && !_multiline)
            {
                _controller.Return();
            }
        }

        InputScopeName NameFromContext(TextInputType context)
        {
            switch (context)
            {
                case TextInputType.FirstLetterUppercase:
                    return _inputScopeName;

                case TextInputType.Numeric:
                    return _inputScopeNumber;

                case TextInputType.Uppercase:
                    return _inputScopeUpper;

                case TextInputType.Email:
                    return _inputScopeEmail;

                case TextInputType.Digits:
                    return _inputScopeDigits;
            }

            return _inputScopeText;
        }

        void HandleEditingChanged(object sender, TextChangedEventArgs e)
        {
            if (_internalTextChange)
                return;

            string newText = _textBox.Text;

            if (_uppercase)
            {
                newText = newText.ToUpper();

                int selStart = _textBox.SelectionStart;
                int selLength = _textBox.SelectionLength;

                _textBox.Text = newText;
                _textBox.Select(selStart, selLength);
            }

            string text = _controller.TextChanged(newText);

            if (text != newText)
            {
                SetText(text);
            }
        }

        public void Unfocus()
        {
            _textBox.TextChanged -= HandleEditingChanged;
            _textBox.LostFocus -= HandleEditingDidEnd;
            _textBox.KeyUp -= HandleKeyUp;

            _passwordBox.PasswordChanged -= HandleEditingChangedPassword;
            _passwordBox.LostFocus -= HandleEditingDidEnd;
            _passwordBox.KeyUp -= HandleKeyUp;

            if (CurrentFocus == this)
            {
                _textBox.Visibility = Visibility.Collapsed;
                _passwordBox.Visibility = Visibility.Collapsed;

                _controller.LostFocus();

                CurrentFocus = null;
            }
        }
    }
}


