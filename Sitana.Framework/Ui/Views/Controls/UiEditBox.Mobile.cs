using System;
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
using Sitana.Framework.Ui.Core;

namespace Sitana.Framework.Ui.Views
{
    public class UiEditBox: UiEditBoxBase, NativeInput.ITextEdit
    {
        public new static void Parse(XNode node, DefinitionFile file)
        {
			UiEditBoxBase.Parse(node, file);

			var parser = new DefinitionParser(node);

			file["NativeInputMargin"] = parser.ParseMargin("NativeInputMargin");
			file["NativeInputAlign"] = parser.ParseEnum<Align>("NativeInputAlign");

			#if __IOS__
				file["NativeInputFontSize"] = parser.ParseInt("iOS.NativeInputFontSize");
			#elif __ANDROID__
				file["NativeInputFontSize"] = parser.ParseInt("Android.NativeInputFontSize");
			#elif __WINDOWSPHONE__
				file["NativeInputFontSize"] = parser.ParseInt("WindowsPhone.NativeInputFontSize");
			#elif __WINRT__
				file["NativeInputFontSize"] = parser.ParseInt("WindowsRT.NativeInputFontSize");
			#endif
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
			if (text.Length > _maxLength)
			{
				text = text.Substring(0, _maxLength);
			} 

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
		Align _nativeInputAlign;

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

		protected override bool Init(object controller, object binding, Sitana.Framework.Ui.DefinitionFiles.DefinitionFile definition)
		{
			if (!base.Init(controller, binding, definition))
			{
				return false;
			}

			DefinitionFileWithStyle file = new DefinitionFileWithStyle(definition, typeof(UiEditBox));

			_fontSize = DefinitionResolver.Get<int>(Controller, Binding, file["NativeInputFontSize"], 20);
			_nativeInputMargin = DefinitionResolver.Get<Margin>(Controller, Binding, file["NativeInputMargin"], Margin.None);
			_nativeInputAlign = DefinitionResolver.Get<Align>(Controller, Binding, file["NativeInputAlign"], Align.Left);
			return true;
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
			rect.Y -= AppMain.Current.MainView.OffsetBoundsVertical;

			rect = _nativeInputMargin.ComputeRect(rect);

			_nativeInput = new NativeInput(rect, _inputType, _original, _fontSize, _nativeInputAlign, this);

			AppMain.Redraw();
		}

		public override void Focus()
		{
			if (!_focused)
			{
				_applied = false;
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
