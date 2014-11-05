﻿using System;
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

        public void Unfocus()
        {
            _consumer.OnLostFocus();
        }

        public void OnKey(Keys key)
        {

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
                if (CanAddCharacter(character))
                {
                    _text += character;
                }
            }

            string text = _consumer.OnTextChanged(_text);

            if ( text != _text )
            {
                SetText(text);
            }
        }

        bool CanAddCharacter(char ch)
        {
            switch(_inputType)
            {
            case TextInputType.AlphaNumeric:
                return char.IsLetterOrDigit(ch);

            case TextInputType.Numeric:
                return char.IsDigit(ch);
            }

            return true;
        }
    }
}
