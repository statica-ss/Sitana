using System;
using Sitana.Framework.Ui.Controllers;
using Sitana.Framework.Cs;
using Sitana.Framework.Input;
using Microsoft.Xna.Framework.Input;
using Sitana.Framework.Ui.Core;

namespace GameEditor
{
    public class MessageBoxController: UiController, IFocusable
    {
        public static MessageBoxController Current {get; private set;}

        EmptyArgsVoidDelegate _onMessageBoxYes;
        EmptyArgsVoidDelegate _onMessageBoxNo;
        EmptyArgsVoidDelegate _onMessageBoxClose;

        bool _cancelVisible;
        bool _yesNoVisible;

        public SharedString MessageBoxText { get; private set; }

        void IFocusable.Unfocus(){}

        void IFocusable.OnKey(Keys key)
        {
            if (key == Keys.Escape)
            {
                OnMessageBoxCancel();
            }
        }

        int IFocusable.Bottom { get { return 0; } }

        void IFocusable.OnCharacter(char character)
        {
            if (_yesNoVisible)
            {
                switch(character)
                {
                case 'Y':
                case 'y':
                    OnMessageBoxYes();
                    break;

                case 'N':
                case 'n':
                    OnMessageBoxNo();
                    break;

                case 'C':
                case 'c':
                case (char)27:
                    if (_cancelVisible)
                    {
                        OnMessageBoxCancel();
                    }
                    else
                    {
                        OnMessageBoxNo();
                    }
                    break;
                }
            }
            else if ( character == 27 || character == '\n' || character == 'o' || character=='O')
            {
                OnMessageBoxCancel();
            }
        }

        void IFocusable.SetText(string text){}

        public MessageBoxController()
        {
            Current = this;
            MessageBoxText = new SharedString();
        }

        public void MessageBox(string text)
        {
            _yesNoVisible = false;
            _cancelVisible = false;

            MessageBoxText.StringValue = text;
            ShowElement("MessageBox");
            HideElement("MessageBoxNo");
            HideElement("MessageBoxYes");
            HideElement("MessageBoxCancel");

            HideElement("MessageBoxNo2");
            HideElement("MessageBoxYes2");

            ShowElement("MessageBoxOk");

            AppMain.Current.SetFocus(this);
        }

        public void MessageBoxYesNo(string text, EmptyArgsVoidDelegate onYes, EmptyArgsVoidDelegate onNo = null, EmptyArgsVoidDelegate alwaysCall = null)
        {
            _yesNoVisible = true;
            _cancelVisible = false;

            _onMessageBoxYes = onYes;
            _onMessageBoxNo = onNo;
            _onMessageBoxClose = alwaysCall;

            MessageBoxText.StringValue = text;
            ShowElement("MessageBox");
            ShowElement("MessageBoxNo2");
            ShowElement("MessageBoxYes2");

            HideElement("MessageBoxCancel");
            HideElement("MessageBoxNo");
            HideElement("MessageBoxYes");

            HideElement("MessageBoxOk");

            AppMain.Current.SetFocus(this);
        }

        public void MessageBoxYesNoCancel(string text, EmptyArgsVoidDelegate onYes, EmptyArgsVoidDelegate onNo = null, EmptyArgsVoidDelegate alwaysCall = null)
        {
            _yesNoVisible = true;
            _cancelVisible = true;

            _onMessageBoxYes = onYes;
            _onMessageBoxNo = onNo;
            _onMessageBoxClose = alwaysCall;

            MessageBoxText.StringValue = text;
            ShowElement("MessageBox");
            ShowElement("MessageBoxNo");
            ShowElement("MessageBoxYes");
            ShowElement("MessageBoxCancel");
            HideElement("MessageBoxOk");

            HideElement("MessageBoxNo2");
            HideElement("MessageBoxYes2");

            AppMain.Current.SetFocus(this);
        }

        public void OnMessageBoxCancel()
        {
            CloseMessageBox();
        }

        public void OnMessageBoxYes()
        {
            if (_onMessageBoxYes != null)
            {
                _onMessageBoxYes();
            }

            CloseMessageBox();
        }

        public void OnMessageBoxNo()
        {
            if (_onMessageBoxNo != null)
            {
                _onMessageBoxNo();
            }
            CloseMessageBox();
        }

        void CloseMessageBox()
        {
            HideElement("MessageBox");

            if ( _onMessageBoxClose != null )
            {
                _onMessageBoxClose();
                _onMessageBoxClose = null;
            }

            AppMain.Current.ReleaseFocus(this);
        }
    }

    public static class MessageBox
    {
        public static void Info(string text)
        {
            MessageBoxController.Current.MessageBox(text);
        }

        public static void YesNo(string text, EmptyArgsVoidDelegate onYes, EmptyArgsVoidDelegate onNo = null, EmptyArgsVoidDelegate alwaysCall = null)
        {
            MessageBoxController.Current.MessageBoxYesNo(text, onYes, onNo, alwaysCall);
        }

        public static void YesNoCancel(string text, EmptyArgsVoidDelegate onYes, EmptyArgsVoidDelegate onNo = null, EmptyArgsVoidDelegate alwaysCall = null)
        {
            MessageBoxController.Current.MessageBoxYesNoCancel(text, onYes, onNo, alwaysCall);
        }
    }
}

