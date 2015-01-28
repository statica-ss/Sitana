﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sitana.Framework.Ui.Views;
using Sitana.Framework.Xml;
using Sitana.Framework.Ui.DefinitionFiles;
using Sitana.Framework.Cs;
using Sitana.Framework.Ui;
using Sitana.Framework.Input;
using Sitana.Framework.Ui.Views.ButtonDrawables;
using Sitana.Framework;
using Microsoft.Xna.Framework;
using Sitana.Framework.Input.TouchPad;

namespace Sitana.Framework.Ui.Views
{
    public class UiEditBox: UiEditBoxBase, NativeInput.ITextEdit
    {
        public new static void Parse(XNode node, DefinitionFile file)
        {
			UiEditBoxBase.Parse(node, file);

			var parser = new DefinitionParser(node);

			file["NativeInputMargin"] = parser.ParseMargin("NativeInputMargin");
			file["NativeInputFontSize"] = parser.ParseInt("NativeInputFontSize");
        }

		void NativeInput.ITextEdit.LostFocus()
		{
			_focused = false;

			if ( _lostFocusCancels )
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

			UiTask.BeginInvoke(() => CallDelegate("LostFocus"));
		}

		string NativeInput.ITextEdit.TextChanged(string text)
		{
			object ret = CallDelegate("TextChanged", new InvokeParam("text", text));

			if (ret != null && ret is string)
			{
				text = (string)ret;
			}

			Text.StringValue = text;
			return text;
		}

		void NativeInput.ITextEdit.Return()
		{
			UiTask.BeginInvoke(() =>
			{
				object ret = CallDelegate("Return");

				if( !(ret is bool && (bool)ret))
				{
					OnApply();
					Unfocus();
				}
			});
		}

		bool NativeInput.ITextEdit.WaitsForReturn
		{
			get
			{
				return _inputType == TextInputType.MultilineText;
			}
		}

		int NativeInput.ITextEdit.MaxLength 
		{
			get
			{
				return int.MaxValue;
			}
		}

		int NativeInput.ITextEdit.MaxLines
		{
			get
			{
				return _inputType == TextInputType.MultilineText ? int.MaxValue : 1;
			}
		}

		Margin _nativeInputMargin;
		NativeInput _nativeInput = null;

		bool _applied = false;

		private string _original = null;
		private int _fontSize = 0;

		public override ButtonState ButtonState
		{
			get
			{
				return base.ButtonState | (_focused ? ButtonState.Checked : ButtonState.None);
			}
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

		void OnTouchDown(int id, Vector2 pos)
		{
			if ( _focused && !IsPointInsideView(pos))
			{
				Unfocus();
			}
		}

		protected override void Init(object controller, object binding, Sitana.Framework.Ui.DefinitionFiles.DefinitionFile definition)
		{
			base.Init(controller, binding, definition);

			DefinitionFileWithStyle file = new DefinitionFileWithStyle(definition, typeof(UiEditBox));

			_fontSize = DefinitionResolver.Get<int>(Controller, Binding, file["NativeInputFontSize"], 20);
			_nativeInputMargin = DefinitionResolver.Get<Margin>(Controller, Binding, file["NativeInputMargin"], Margin.None);
		}

		private void CreateInput()
		{
			if (_nativeInput != null)
			{
				_nativeInput.Unfocus();
			}

			_focused = true;
			_original = Text.StringValue;

			Rectangle rect = ScreenBounds;

			rect = _nativeInputMargin.ComputeRect(rect);

			_nativeInput = new NativeInput(rect, _inputType, _original, _fontSize, Align.Left, this);
		}

		public override void Focus()
		{
			if (!_focused)
			{
				CreateInput();
			}
		}

		protected override void DoAction()
		{
			Focus();
		}

		void Unfocus()
		{
			_focused = false;
			if (_nativeInput != null)
			{
				_nativeInput.Unfocus();
			}
		}


		void OnCancel()
		{
			if ( !_applied)
			{
				_applied = true;

				CallDelegate("TextCancel");
			}
		}

		void OnApply()
		{
			if ( !_applied)
			{
				_applied = true;

				string text = Text.StringValue;

				object ret = CallDelegate("TextApply", new InvokeParam("text", text));

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
    }
}