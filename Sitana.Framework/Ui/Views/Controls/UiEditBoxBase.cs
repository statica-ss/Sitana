﻿using System;
using Sitana.Framework.Input;
using Sitana.Framework.Ui.Views.ButtonDrawables;
using Sitana.Framework.Ui.Core;
using Sitana.Framework.Input.TouchPad;
using Microsoft.Xna.Framework;
using Sitana.Framework.Xml;
using Sitana.Framework.Ui.DefinitionFiles;
using Sitana.Framework.Cs;

namespace Sitana.Framework.Ui.Views
{
    public abstract class UiEditBoxBase : UiButton
    {
        public new static void Parse(XNode node, DefinitionFile file)
        {
            UiButton.Parse(node, file);

            var parser = new DefinitionParser(node);

            file["Hint"] = parser.ParseString("Hint");

            file["InputType"] = parser.ParseEnum<TextInputType>("InputType");
            file["CancelOnLostFocus"] = parser.ParseBoolean("CancelOnLostFocus");

            file["TextApply"] = parser.ParseDelegate("TextApply");
            file["TextCancel"] = parser.ParseDelegate("TextCancel");
            file["TextChanged"] = parser.ParseDelegate("TextChanged");

			file["LostFocus"] = parser.ParseDelegate("LostFocus");
			file["Return"] = parser.ParseDelegate("Return");
        }

        public override ButtonState ButtonState
        {
            get
            {
                return base.ButtonState | (_focused ? ButtonState.Checked : ButtonState.None);
            }
        }

        protected bool _lostFocusCancels = false;
        protected bool _focused = false;

        public SharedString Hint {get;set;}

        protected SharedString _password;
        protected TextInputType _inputType;

        protected override bool Init(object controller, object binding, DefinitionFile definition)
        {
            if (!base.Init(controller, binding, definition))
            {
                return false;
            }

            DefinitionFileWithStyle file = new DefinitionFileWithStyle(definition, typeof(UiEditBoxBase));

            Hint = DefinitionResolver.GetSharedString(Controller, Binding, file["Hint"]) ?? new SharedString();

            _inputType = DefinitionResolver.Get<TextInputType>(Controller, Binding, file["InputType"], TextInputType.NormalText);
            _lostFocusCancels = DefinitionResolver.Get<bool>(Controller, Binding, file["CancelOnLostFocus"], false);

            if (_inputType == TextInputType.Password)
            {
                _password = new SharedString();
            }

            RegisterDelegate("TextApply", file["TextApply"]);
            RegisterDelegate("TextChanged", file["TextChanged"]);
            RegisterDelegate("TextCancel", file["TextCancel"]);

			RegisterDelegate("LostFocus", file["LostFocus"]);
			RegisterDelegate("Return", file["Return"]);

            return true;
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

            bool hint = _text.Length == 0;
            bool password = _inputType == TextInputType.Password;

            if (password)
            {
                if (_password.Length != _text.Length)
                {
                    _password.Clear();
                    for (int idx = 0; idx < _text.Length; ++idx)
                    {
						_password.Append("●");
                    }
                }
            }

            drawInfo.Text = hint ? Hint : (password ? _password : _text);
            drawInfo.ButtonState = ButtonState;

            if (hint)
            {
                drawInfo.ButtonState |= ButtonDrawables.ButtonState.Special;
            }

            drawInfo.Target = ScreenBounds;
            drawInfo.Opacity = opacity;
            drawInfo.EllapsedTime = parameters.EllapsedTime;
            drawInfo.Icon = Icon.Value;

            for (int idx = 0; idx < _drawables.Count; ++idx)
            {
                var drawable = _drawables[idx];
                drawable.Draw(batch, drawInfo);
            }
        }

        public virtual void Focus()
        {
            
        }

        protected override void DoAction()
        {
            Focus();
        }

        
    }
}

