using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sitana.Framework.Ui.Views;
using Sitana.Framework.Xml;
using Sitana.Framework.Ui.DefinitionFiles;
using Sitana.Framework.Ui.Core;
using Sitana.Framework.Input;
using Sitana.Framework.Input.TouchPad;
using Microsoft.Xna.Framework;
using Sitana.Framework.Ui.Views.ButtonDrawables;

namespace Sitana.Framework.Ui.Views
{
    public class UiEditBox : UiEditBoxBase, ITextConsumer
    {
        public new static void Parse(XNode node, DefinitionFile file)
        {
            UiEditBoxBase.Parse(node, file);
        }

        TextInput _textInput;
        bool _applied = false;
        int _carretPosition = 0;

        string ITextConsumer.OnTextChanged(string newText)
        {
            newText = OnTextChanged(newText);

            Text.Format("{0}", newText);

            return newText;
        }

        void ITextConsumer.OnLostFocus()
        {
            _focused = false;

            if (_lostFocusCancels)
            {
                if (!_applied)
                {
                    Text.Format("{0}", _original);
                    OnCancel();
                }
            }
            else
            {
                OnApply();
            }

            AppMain.Current.ReleaseFocus(_textInput);
        }

        public void Apply()
        {
            _original = Text.StringValue;

            OnApply();

            AppMain.Current.ReleaseFocus(_textInput);

            CallDelegate("Return");
        }

        public void Cancel()
        {
            Text.Format("{0}", _original);
            OnCancel();

            AppMain.Current.ReleaseFocus(_textInput);
        }

        int ITextConsumer.SelectionStart
        {
            set
            {
                _carretPosition = value;
            }
        }

        int ITextConsumer.SelectionEnd
        {
            set
            {
                 _carretPosition = value;
            }
        }

        
        private string _original = null;

        protected override bool Init(object controller, object binding, Sitana.Framework.Ui.DefinitionFiles.DefinitionFile definition)
        {
            if (!base.Init(controller, binding, definition))
            {
                return false;
            }

            DefinitionFileWithStyle file = new DefinitionFileWithStyle(definition, typeof(UiEditBox));

            _textInput = new TextInput(this, _inputType);

            _lostFocusCancels = DefinitionResolver.Get<bool>(Controller, Binding, file["CancelOnLostFocus"], false);

            return true;
        }

        protected override void OnAdded()
        {
            base.OnAdded();
            TouchPad.Instance.TouchDown += OnTouchDown;
        }

        protected override void OnRemoved()
        {
            base.OnRemoved();
            TouchPad.Instance.TouchDown -= OnTouchDown;
        }

        public override void Focus()
        {
            if (!_focused)
            {
                _focused = true;
                _original = Text.StringValue;
                _textInput.SetText(_original);
                _applied = false;
                _carretPosition = Text.Length;

                AppMain.Current.SetFocus(_textInput);
            }
        
        }

        void OnTouchDown(int id, Vector2 pos)
        {
            if (_focused )
            {
                if(!IsPointInsideView(pos))
                {
                    AppMain.Current.ReleaseFocus(_textInput);
                }
                else
                {
                    var drawInfo = new DrawButtonInfo();

                    drawInfo.Text = _text;
                    drawInfo.ButtonState = ButtonState;

                    drawInfo.Target = ScreenBounds;
                    drawInfo.Opacity = 0;
                    drawInfo.EllapsedTime = 0;
                    drawInfo.Icon = Icon.Value;
                    drawInfo.Additional = 0;

                    int clickPosition = (int)pos.X;

                    for (int idx = 0; idx < _drawables.Count; ++idx)
                    {
                        var drawable = _drawables[idx];
                        object ret = drawable.OnAction(drawInfo, clickPosition);

                        if(ret != null)
                        {
                            _carretPosition = (int)ret;
                        }
                    }
                }
            }
        }

        void OnCancel()
        {
            if (!_applied)
            {
                _applied = true;

                CallDelegate("TextCancel", new InvokeParam("sender", this));
            }
        }

        void OnApply()
        {
            if (!_applied)
            {
                _applied = true;

                string text = Text.StringValue;

                object ret = CallDelegate("TextApply", new InvokeParam("sender", this), new InvokeParam("text", text));

                if (ret is string)
                {
                    string newText = ret as string;

                    if (text != newText)
                    {
                        Text.StringValue = newText;
                    }
                }
            }
        }

        protected override void Draw(ref Parameters.UiViewDrawParameters parameters)
        {
            float opacity = parameters.Opacity;

            if (opacity == 0)
            {
                return;
            }

            var batch = parameters.DrawBatch;

            var drawInfo = new DrawButtonInfo();

            drawInfo.Text = _text;
            drawInfo.ButtonState = ButtonState;

            drawInfo.Target = ScreenBounds;
            drawInfo.Opacity = opacity;
            drawInfo.EllapsedTime = parameters.EllapsedTime;
            drawInfo.Icon = Icon.Value;
            drawInfo.Additional = _carretPosition;

            for (int idx = 0; idx < _drawables.Count; ++idx)
            {
                var drawable = _drawables[idx];
                drawable.Draw(batch, drawInfo);
            }
        }

        string OnTextChanged(string text)
        {
            object ret = CallDelegate("TextChanged", new InvokeParam("sender", this), new InvokeParam("text", text));

            if (ret is string)
            {
                text = ret as string;
            }

            return text;
        }
    }
}
