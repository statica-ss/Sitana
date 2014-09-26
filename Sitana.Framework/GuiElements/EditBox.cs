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

using Sitana.Framework.Content;
using Sitana.Framework;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Input.Touch;
using Sitana.Framework.Input;

namespace Sitana.Framework.Gui
{
    public class TextSourceEventArgs : EventArgs
    {
        public String Text;
        public Boolean Cancel = false;
        public Boolean Update = false;
    }

    public class EditBox : Button
    {
        private static EditBox _focusedElement = null;

        private static Object _lockFocus = new Object();

        public delegate void TextSourceHandler(Object sender, TextSourceEventArgs args);
        public event TextSourceHandler TextChanged;

        public event EventHandler LostFocus;

        private const Single _waitForCursorToHide = 0.4f;
        private const Single _waitForCursorToShow = 0.2f;

        private Char? _hiddenCharacter;

        private Boolean _showCursor = true;
        private Single _wait = 0;

        public String Text {get; private set;}

        private StringBuilder _stringBuilder = new StringBuilder();

        private Single _hideLastCharacter = 0;

        private String _hint;
        private ColorWrapper _hintColor;
        private ColorWrapper _invalidColor;

        private Action _onTextAccept;
        private Action _onReturn;

        private Boolean _isInvalid = false;

        private String _filter = null;

        private Texture2D _eraseButton;
        private Single _eraseButtonMargin;
        private NinePatchImage _invalidBackground;

        private Color _eraseButtonColor;

        private Vector2 _cursorOffset = Vector2.Zero;

        private KeyboardContext _keyboardContext = KeyboardContext.NormalText;

        private Boolean _isCentered = false;

        private Vector2 _desiredScreenOffset = Vector2.Zero;

        private String[] _forceVisibleIds = null;

        public Boolean IsValid
        {
            get
            {
                return !_isInvalid;
            }
        }

        public override Boolean IsPushed
        {
            get
            {
                return base.IsPushed || IsFocused;
            }
        }

        private Boolean IsFocused
        {
            get
            {
                lock(_lockFocus)
                {
                    return _focusedElement == this;
                }
            }

            set
            {
                lock (_lockFocus)
                {
                    if (value)
                    {
                        if (_focusedElement != this && _focusedElement != null)
                        {
                            _focusedElement.Unfocus();
                        }

                        _focusedElement = this;
                    }
                    else if (_focusedElement == this)
                    {
                        _focusedElement = null;

                        DelayedActionInvoker.Instance.AddAction(0.1f, (t) =>
                            {
                                lock (_lockFocus)
                                {
                                    if (_focusedElement == null)
                                    {
                                        Owner.ScreenOffset = Vector2.Zero;
                                        ScreenManager.Current.Redraw();
                                    }
                                }
                            });

                        if (LostFocus != null)
                        {
                            LostFocus(this, EventArgs.Empty);
                        }

                        SystemWrapper.HideKeyboard();
                    }
                }
            }
        }

        enum CharTransform
        {
            None,
            Upper,
            Lower
        }

        private CharTransform _charTransform;
        private Int32 _maxLength = 0;
        private List<Char> _charactersFromInput = new List<Char>();

        public void Validate(Boolean valid)
        {
            _isInvalid = !valid;
        }

        protected override Boolean Initialize(InitializeParams initParams)
        {
            ParametersCollection parameters = initParams.Parameters;
            String directory = initParams.Directory;
            Vector2 scale = initParams.Scale;
            
            parameters = parameters.Clone();

            String text = parameters.AsString("Text");

            if (text == null)
            {
                text = String.Empty;
            }

            parameters.Update("Text", "");

            if ( !base.Initialize(initParams) )
            {
                return false;
            }

            _isCentered = parameters.AsAlign("ContentAlign", "") == Align.Center;

            _charTransform = parameters.AsEnum<CharTransform>("CharTransform", CharTransform.None);
            _maxLength = parameters.AsInt32("MaxLength");

            _hint = parameters.AsString("Hint");
            _hintColor = parameters.FindColorWrapper("HintColor");

            _invalidColor = parameters.FindColorWrapper("BackgroundColorInvalid");

            _forceVisibleIds = parameters.AsString("ForceVisibleIds").Split(',');

            if (parameters.HasKey("BackgroundInvalid"))
            {
                _invalidBackground = ContentLoader.Current.Load<NinePatchImage>(parameters.AsString("BackgroundInvalid"));
            }
            else
            {
                _invalidBackground = _backgroundTexture;
            }

            _filter = parameters.AsString("Filter");

            _keyboardContext = parameters.AsEnum<KeyboardContext>("KeyboardContext", KeyboardContext.NormalText);

            if (parameters.HasKey(("EraseButton")))
            { 
                _eraseButton = ContentLoader.Current.Load<Texture2D>(parameters.AsString("EraseButton"));

                _eraseButtonMargin = parameters.AsInt32("EraseButtonMargin");
                _additionalRightMargin = _eraseButton.Width * scale.X + _eraseButtonMargin * scale.X;
            }

            _eraseButtonColor = parameters.AsColor("EraseButtonColor");

            String hiddenChar = parameters.AsString("HideCharacter");

            if ( hiddenChar != "" )
            {
                _hiddenCharacter = hiddenChar[0];
            }

            _onTextAccept = parameters.AsAction("OnTextAccept", this);
            _onReturn = parameters.AsAction("OnReturn", this);

            _smallTextPresenter = _textPresenter.Clone();
            _smallTextPresenter.PrepareRender("");

            ButtonAction = null;

            _cursorOffset = _smallTextOffset;
            

            UpdateText(text);

            InstallGestureHandler(GestureAdditionalType.Native, GestureType.Tap, OnTap);

            return true;
        }

        protected override void OnAction(Vector2 position)
        {
            if ( IsFocused && _eraseButton != null )
            {
                Rectangle eraseRect = new Rectangle(ElementRectangle.Width - (Int32)(_margin * 2 + (_eraseButtonMargin+ _eraseButton.Width) * Scale.X), 0, (Int32)(_margin * 2 + (_eraseButtonMargin+_eraseButton.Width)*Scale.X), ElementRectangle.Height);

                if (eraseRect.Contains(GraphicsHelper.PointFromVector2(position)))
                {
                    if (TextChanged != null)
                    {
                        TextSourceEventArgs args = new TextSourceEventArgs()
                        {
                            Text = "",
                            Cancel = false
                        };

                        TextChanged(this, args);
                    }

                    UpdateText("");
                }
            }

            Focus();
        }

        private void OnTap(Object sender, EventArgs e)
        {
            if (IsFocused)
            {
                GestureEventArgs args = e as GestureEventArgs;

                Vector2 position = args.Sample.Position - Owner.ScreenOffset;

                if (!ElementRectangle.Contains(GraphicsHelper.PointFromVector2(position)))
                {
                    Owner.ScreenManager.UiAction (() =>
                    {
                        Unfocus();
                    });
                }
            }
        }

        public override void OnRemoved()
        {
            base.OnRemoved();

            if (IsFocused)
            {
                Unfocus();
            }
        }

        public override void Draw(Int32 level, SpriteBatch spriteBatch, Vector2 topLeft, Single transition)
        {
            if (!DrawLevel(level))
            {
                return;
            }

            ColorWrapper bgColorNormal = _backgroundColor;
            NinePatchImage bgNormal = _backgroundTexture;

            if (_isInvalid)
            {
                _backgroundColor = _invalidColor;
                _backgroundTexture = _invalidBackground;
            }

            if ( String.IsNullOrEmpty(_text) && !String.IsNullOrEmpty(_hint))
            {
                ColorWrapper color = _textColor;
                ColorWrapper pushedColor = _textPushedColor;

                String text = _text;

                _smallTextPresenter.PrepareRender(String.Empty);
                base.UpdateText(_hint);

                _textColor = _hintColor;
                _textPushedColor = _hintColor;

                base.Draw(level, spriteBatch, topLeft, transition);

                base.UpdateText(text);
                _textColor = color;
                _textPushedColor = pushedColor;

                Color color1 = _backgroundColor.Value;
                Color pushedColor1 = _backgroundPushedColor.Value;

                _backgroundColor.Value = Color.Transparent;
                _backgroundPushedColor.Value = Color.Transparent;

                _smallTextPresenter.PrepareRender(_showCursor ? "|" : "");

                base.Draw(level, spriteBatch, topLeft, transition);

                _backgroundColor.Value = color1;
                _backgroundPushedColor.Value = pushedColor1;
            }
            else
            {
                base.Draw(level, spriteBatch, topLeft, transition);
            }

            if (IsFocused && _eraseButton != null)
            {
                Vector2 position = new Vector2(ElementRectangle.X+ElementRectangle.Width-(_margin+_eraseButtonMargin) * Scale.X, ElementRectangle.Y + ElementRectangle.Height / 2) + topLeft;
                Vector2 origin = new Vector2(_eraseButton.Width, _eraseButton.Height/2);
                Single opacity = transition < 1 ? 0 : 1;

                spriteBatch.Draw(_eraseButton, position, null, _eraseButtonColor * opacity, 0, origin, Scale, SpriteEffects.None, 0 );
            }

            _backgroundColor = bgColorNormal;
            _backgroundTexture = bgNormal;
        }

        public void SetHint(String hint)
        {
            _hint = hint;
        }

        public void Focus()
        {
            Single height = 0;

            if (IsFocused)
            {
                height = SystemWrapper.ShowKeyboard();
            }
            else
            {
                height = SystemWrapper.ShowKeyboard(ElementRectangle, _keyboardContext, (c) => OnCharacter(c), () => OnLostFocus(), (s)=>OnKeyboardResized(s));
            }

            height *= ScreenManager.Current.Scale;

            IsFocused = true;

            Single bottomY = Owner.AreaSize.Y - height;

            Int32 visibleBottom = CalculateVisibleElementsBottom();

            _desiredScreenOffset = new Vector2 (0, Math.Min(0, bottomY - visibleBottom));
            Console.WriteLine("ScreenOffset: {0},{1}", _desiredScreenOffset.X, _desiredScreenOffset.Y);

            if (!_hiddenCharacter.HasValue)
            {
                base.UpdateText(Text);
            }
        }

        private Int32 CalculateVisibleElementsBottom()
        {
            Int32 bottom = ElementRectangle.Bottom + ElementRectangle.Height;

            foreach (var id in _forceVisibleIds)
            {
                var el = Owner.Find(id);

                if (el != null)
                {
                    bottom = Math.Max(bottom, el.ElementRectangle.Bottom);
                }
            }

            return bottom;
        }

        private void OnKeyboardResized(Single height)
        {
            UiTask.BeginInvoke(() =>
            {
                height *= ScreenManager.Current.Scale;

                Single bottomY = Owner.AreaSize.Y - height;

                Int32 visibleBottom = CalculateVisibleElementsBottom();

                _desiredScreenOffset = new Vector2(0, Math.Min(0, bottomY - visibleBottom));

                Console.WriteLine("ScreenOffset: {0},{1}", _desiredScreenOffset.X, _desiredScreenOffset.Y);
            });
        }

        public void Unfocus()
        {
            IsFocused = false;
        }

        private void OnLostFocus()
        {
            IsFocused = false;

            if ( _onTextAccept != null )
            {
                _onTextAccept.Invoke();
            }
        }

        public void SetText(String text)
        {
            UpdateText(text);
        }

        private new void UpdateText(String text)
        {
            Text = String.Copy(text);

            String displayText = text;

            if ( _hiddenCharacter.HasValue)
            {
                _stringBuilder.Clear();

                for ( Int32 idx = 0; idx < displayText.Length-1; ++idx )
                {
                    _stringBuilder.Append(_hiddenCharacter.Value);
                }

                if (displayText.Length > 0)
                {
                    if (_hideLastCharacter > 0)
                    {
                        _stringBuilder.Append(displayText[displayText.Length-1]);
                    }
                    else
                    {
                        _stringBuilder.Append(_hiddenCharacter.Value);
                    }
                }

                displayText = _stringBuilder.ToString();
            }

            if (String.IsNullOrEmpty(Text))
            {
                _smallTextOffset.X = 0;
            } 
            else
            {
                _smallTextOffset.X = _cursorOffset.X;
            }

            base.UpdateText(displayText);

            if (_isCentered)
            {
                _iconAndTextSize.X = _textPresenter.Size.X * Scale.X;
            }
        }

        private void OnCharacter(Char character)
        {
            if (character != '\0')
            {
                lock (_charactersFromInput)
                {
                    _charactersFromInput.Add(character);
                }
            }
        }

        public override bool Update(TimeSpan gameTime, Screen.ScreenState screenState)
        {
            Boolean redraw = false;

            if (!Enabled && IsFocused)
            {
                Unfocus();
                return true;
            }

            if ( _hideLastCharacter > 0)
            {
                _hideLastCharacter -= (Single)gameTime.TotalSeconds;
                if ( _hideLastCharacter <= 0 )
                {
                    _hideLastCharacter = 0;
                    UpdateText(Text);
                }
            }

            lock (_charactersFromInput)
            {
                if ( _charactersFromInput.Count > 0 )
                {
                    String text = Text;
                    for (Int32 idx = 0; idx < _charactersFromInput.Count; ++idx)
                    {
                        Char character = _charactersFromInput[idx];
                        text = AnalyzeInput(character, text);
                    }

                    text = ProcessCurrentString(text);

                    Boolean cancel = false;

                    if (TextChanged != null)
                    {
                        TextSourceEventArgs args = new TextSourceEventArgs()
                        {
                            Text = text,
                            Cancel = false
                        };

                        TextChanged(this, args);

                        if ( args.Cancel )
                        {
                            cancel = true;
                        }
                        else if (args.Update)
                        {
                            text = args.Text;
                        }
                    }

                    if (!cancel)
                    {
                        UpdateText(text);
                    }

                    _charactersFromInput.Clear();
                    redraw = true;
                }
            }


            if (IsFocused)
            {
                Owner.ScreenOffset = _desiredScreenOffset;

                _wait += (Single)gameTime.TotalSeconds;
                Single waitTime = _showCursor ? _waitForCursorToHide : _waitForCursorToShow;

                if (_wait > waitTime)
                {
                    _showCursor = !_showCursor;
                    _wait -= waitTime;

                    _smallTextPresenter.PrepareRender(_showCursor ? "|" : "");
                    redraw = true;
                }
            }
            else
            {
                if ( _showCursor )
                {
                    _smallTextPresenter.PrepareRender("");
                    redraw = true;
                    _showCursor = false;
                }
            }

            return redraw || base.Update(gameTime, screenState);
        }

        private String ProcessCurrentString(String text)
        {
            switch(_charTransform)
            {
                case CharTransform.Lower:
                    text = text.ToLower();
                    break;

                case CharTransform.Upper:
                    text = text.ToUpper();
                    break;
            }

            if ( _maxLength > 0 && text.Length > _maxLength)
            {
                text = text.Substring(0, _maxLength);
            }

            return text;
        }

        private String AnalyzeInput(Char input, String text)
        {
            switch(input)
            {
            case '\n':
            case '\r':

                if (_onReturn != null)
                {
                        Owner.ScreenManager.UiAction(() =>
                            _onReturn.Invoke());

                }
                else
                {
                    SystemWrapper.HideKeyboard();
                }
                
                break;

            case '\b':
                if (text != "")
                {
                    text = text.Substring(0, text.Length - 1);
                }
                break;

            default:

                if (String.IsNullOrEmpty(_filter) || _filter.Contains(input.ToString()))
                {
                    text = AppendChar(input, text);
                    _hideLastCharacter = 0.5f;
                }
                
                break;
            }

            return text;
        }

        private String AppendChar(Char character, String text)
        {
            switch(_keyboardContext)
            {
                case KeyboardContext.Uppercase:
                    text += (new String(character, 1)).ToUpper();
                    break;

                case KeyboardContext.FirstLetterUppercase:

                    if (text.Length == 0 || Char.IsWhiteSpace(text[text.Length - 1]))
                    {
                        text += (new String(character, 1)).ToUpper();
                    }
                    else
                    {
                        text += character;
                    }
                    break;

                default:
                    text += character;
                    break;
            }

            return text;
        }
    }
}
