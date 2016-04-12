using System;
using Sitana.Framework.Ui.Interfaces;
using Microsoft.Xna.Framework.Input;

namespace Sitana.Framework.Input
{
    public class TextInput: IFocusable
    {
        ITextConsumer _consumer;

        string _text;
        TextInputType _inputType;

        public TextInput(ITextConsumer consumer, TextInputType inputType)
        {
            _consumer = consumer;
            _inputType = inputType;
        }

        public void SetText(string text)
        {
            _text = text;

            _consumer.SelectionStart = text.Length;
            _consumer.SelectionEnd = text.Length;
        }

        public int Bottom { get { return 0; } }

        public void Unfocus()
        {
            _consumer.OnLostFocus();
        }

        public void OnKey(Keys key)
        {
            switch(key)
            {
                case Keys.Escape:
                    _consumer.Cancel();
                    break;
            }
        }

        public void OnCharacter(char character)
        {
          if ( character == '\b')
            {
                if (_text.Length > 0)
                {
                    _text = _text.Substring(0, _text.Length - 1);
                }
            }
            else if(character == '\n')
            {
                _consumer.Apply();
                return;
            }
            else if(character == 27)
            {
                _consumer.Cancel();
                return;
            }
            else
            {
                if (_inputType == TextInputType.Uppercase)
                {
                    character = Char.ToUpperInvariant(character);
                }

                if (ProcessCharacter(ref character))
                {
                    _text += character;
                }
            }

            string text = _consumer.OnTextChanged(_text);

            if ( text != _text )
            {
                SetText(text);
            }
            else
            {
                _consumer.SelectionStart = text.Length;
                _consumer.SelectionEnd = text.Length;
            }
        }

        bool ProcessCharacter(ref char ch)
        {
            switch(_inputType)
            {
            case TextInputType.Digits:
                return char.IsDigit(ch);

            case TextInputType.Numeric:
                return char.IsDigit(ch) || ch == '.' || ch == ',' || ch=='-' || ch=='+';

            case TextInputType.Uppercase:
                ch = char.ToUpperInvariant(ch);
                break;
            }

            return true;
        }
    }
}

